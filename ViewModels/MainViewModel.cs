/*
    Chunk C1: Using Declarations and Class Shell Definition:
This chunk imports your required MVVM toolkits, sets up your collections, 
and configures the tracking property for the selected student row.
*/
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WpfAppSchoolDataBase.Models; // Points to your EF Core folder

namespace WpfAppSchoolDataBase.ViewModels
{
    // ObservableObject enables the automatic modern data binding notifications
    public partial class MainViewModel : ObservableObject
    {
        
        // ObservableCollections tell the DataGrid to automatically refresh on updates
        public ObservableCollection<Course> Courses { get; set; } = new();
        public ObservableCollection<StudentAdmission> Students { get; set; } = new();

        // MODERN C# 14 WAY: Delete the private '_selectedStudent' variable completely!
        public StudentAdmission? SelectedStudent
        {
            get => field; // C# 14 compiler auto-synthesizes the internal backing storage path
            set
            {
                if (SetProperty(ref field, value)) // Uses C# 14 field token cleanly
                {
                    ApproveCommand.NotifyCanExecuteChanged();
                    RejectCommand.NotifyCanExecuteChanged();
                    DeleteStudentCommand.NotifyCanExecuteChanged();
                    SaveStudentChangesCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /*
         * Chunk C2: The Constructor and DB Options Configuration:
         * This chunk instantiates the View Model when the app starts and 
         * defines your encrypted database connection parameters targeting 
         * your specific (localdb)\ProjectModels server instance.
         */
        public MainViewModel()
        {
            LoadDataFromDatabase();
        }

        // Shared helper method to safely provide SQL Server connection strings
        private DbContextOptions<SchoolDBContext> GetDbOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SchoolDBContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=SchoolDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
            return optionsBuilder.Options;
        }

        // Returns true if a student record row is highlighted in the grid
        private bool HasSelection() => SelectedStudent != null;

        /*
         * Chunk C3: The Read Method (LoadDataFromDatabase):
         * This method queries your SQL Express engine to pull down 
         * all master course catalogs and existing student records, 
         * feeding them cleanly into your visual grids.
         */
        private void LoadDataFromDatabase()
        {
            try
            {
                using (var db = new SchoolDBContext(GetDbOptions()))
                {
                    Students.Clear();
                    Courses.Clear();

                    // Retrieve rows from server tables
                    foreach (var course in db.Courses.ToList()) Courses.Add(course);
                    foreach (var student in db.StudentAdmissions.ToList()) Students.Add(student);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to local SQL Express Database instance:\n\n{ex.Message}",
                                "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /*
         * Chunk C4: The Create Method (NewStudent)
         * This method instantiates a clean placeholder row profile template with empty fields, 
         * allowing your attached placeholder watermarks to instantly display inside the text boxes.
         */
        [RelayCommand]
        private void NewStudent()
        {
            if (!Courses.Any())
            {
                MessageBox.Show("Please populate at least one course in the database before managing students.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var emptyAdmission = new StudentAdmission
            {
                StudentID = 0, // Explicit tracking flag for completely uncommitted entries
                FullName = "",
                Email = "",
                Gender = "Male",
                DateOfBirth = new DateOnly(2005, 1, 1),
                PhoneNum = "",
                StreetAddress = "",
                City = "",
                StateProvince = "",
                PostalCode = "",
                PrevSchoolName = "",
                GPA = 0.00M,
                GraduationYear = DateTime.Today.Year,
                CourseID = Courses.First().CourseID,
                AdmissionDate = DateOnly.FromDateTime(DateTime.Today),
                ApplicationStatus = "Pending"
            };

            Students.Add(emptyAdmission);
            SelectedStudent = emptyAdmission;
        }

        /*
         * Chunk C5: The Update Method (SaveStudentChanges)
         * This method handles both new record saving and edits. 
         * It contains your custom validation engine that 
         * checks your SQL server for duplicate telephone numbers or 
         * email entries before running the save operation.
         */
        [RelayCommand(CanExecute = nameof(HasSelection))]
        private void SaveStudentChanges()
        {
            if (SelectedStudent == null) return;

            // Enforce filled essential text boxes
            if (string.IsNullOrWhiteSpace(SelectedStudent.FullName) ||
                string.IsNullOrWhiteSpace(SelectedStudent.Email) ||
                string.IsNullOrWhiteSpace(SelectedStudent.PhoneNum))
            {
                MessageBox.Show("Validation Failed: Full Name, Email, and Phone Number fields cannot be left blank.", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new SchoolDBContext(GetDbOptions()))
            {
                try
                {
                    // Scan DB locally to look for unique key constraint duplicates
                    bool isPhoneDuplicate = db.StudentAdmissions.Any(s => s.PhoneNum == SelectedStudent.PhoneNum && s.StudentID != SelectedStudent.StudentID);
                    bool isEmailDuplicate = db.StudentAdmissions.Any(s => s.Email == SelectedStudent.Email && s.StudentID != SelectedStudent.StudentID);

                    if (isPhoneDuplicate)
                    {
                        MessageBox.Show($"Validation Error: The phone number '{SelectedStudent.PhoneNum}' is already assigned to an existing applicant record.\n\nPlease fix the number before saving.",
                                        "Duplicate Information Blocked", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }

                    if (isEmailDuplicate)
                    {
                        MessageBox.Show($"Validation Error: The email address '{SelectedStudent.Email}' is already registered in the system.\n\nPlease modify it.",
                                        "Duplicate Information Blocked", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }

                    // Determine if record is new or updated
                    if (SelectedStudent.StudentID == 0)
                    {
                        db.StudentAdmissions.Add(SelectedStudent);
                        db.SaveChanges();
                        MessageBox.Show("New student record created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        db.Entry(SelectedStudent).State = EntityState.Modified;
                        db.SaveChanges();
                        MessageBox.Show("Student profile updates applied successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Dynamic Reload: Re-syncs grid without dropping current row highlight focus
                    var currentSelectionId = SelectedStudent.StudentID;
                    LoadDataFromDatabase();
                    SelectedStudent = Students.FirstOrDefault(s => s.StudentID == currentSelectionId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database Operation Rejected:\n\n{ex.InnerException?.Message ?? ex.Message}",
                                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /*
         * Chunk C6: The Delete Method (DeleteStudent)
         * This method verifies an entry exists on your SQL local server, 
         * asks for user confirmation via a clean yes/no alert dialog box, and drops the record line cleanly.
         */
        [RelayCommand(CanExecute = nameof(HasSelection))]
        private void DeleteStudent()
        {
            if (SelectedStudent == null) return;

            var result = MessageBox.Show($"Are you sure you want to permanently delete {SelectedStudent.FullName}?", "Confirm Removal Action", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new SchoolDBContext(GetDbOptions()))
                {
                    var target = db.StudentAdmissions.Find(SelectedStudent.StudentID);
                    if (target != null)
                    {
                        db.StudentAdmissions.Remove(target);
                        db.SaveChanges();
                    }
                }
                Students.Remove(SelectedStudent);
                SelectedStudent = null;
            }
        }
        /*
         * Chunk C7: State Actions (Approve / Reject) & Closing Brackets
         * These final methods automatically flip your string columns and 
         * call the central save routing infrastructure, 
         * followed by the final two brackets to close out your entire class file.
         */
        [RelayCommand(CanExecute = nameof(HasSelection))]
        private void Approve()
        {
            if (SelectedStudent != null) { SelectedStudent.ApplicationStatus = "Approved"; SaveStudentChanges(); }
        }

        [RelayCommand(CanExecute = nameof(HasSelection))]
        private void Reject()
        {
            if (SelectedStudent != null) { SelectedStudent.ApplicationStatus = "Rejected"; SaveStudentChanges(); }
        }
    }
}