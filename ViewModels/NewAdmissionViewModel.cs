using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WpfAppSchoolDataBase.Models;

namespace WpfAppSchoolDataBase.ViewModels
{
    public partial class NewAdmissionViewModel : ViewModelBase
    {
        private readonly DbContextOptions<SchoolDBContext> _dbOptions;

        public StudentAdmission StudentRecord { get; set; } = null!;
        public ObservableCollection<Course> AvailableCourses { get; } = [];

        // Modern C# expression property auto-computes the current calendar year dynamically
        public string CurrentAcademicYear => $"For Year {DateTime.Today.Year}";

        // Boolean properties to bind cleanly to the WPF View RadioButtons
        public bool IsMale
        {
            get => StudentRecord?.Gender == "Male";
            set
            {
                if (value && StudentRecord != null)
                {
                    StudentRecord.Gender = "Male";
                    OnPropertyChanged(nameof(IsMale));
                }
            }
        }

        public bool IsFemale
        {
            get => StudentRecord?.Gender == "Female";
            set
            {
                if (value && StudentRecord != null)
                {
                    StudentRecord.Gender = "Female";
                    OnPropertyChanged(nameof(IsFemale));
                }
            }
        }

        public NewAdmissionViewModel(DbContextOptions<SchoolDBContext> dbOptions)
        {
            _dbOptions = dbOptions;
            InitializeFormLayout();
        }

        public NewAdmissionViewModel() : this(new DbContextOptionsBuilder<SchoolDBContext>()
            .UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=SchoolDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
            .Options)
        {
        }

        private void InitializeFormLayout()
        {
            using var db = new SchoolDBContext(_dbOptions);
            foreach (var course in db.Courses.ToList()) AvailableCourses.Add(course);

            StudentRecord = new StudentAdmission
            {
                FullName = "",
                Email = "",
                Gender = "Male", // Default safe initialization
                DateOfBirth = new DateOnly(2006, 1, 1),
                PhoneNum = "",
                StreetAddress = "",
                City = "",
                StateProvince = "",
                PostalCode = "",
                PrevSchoolName = "",
                GPA = 0.00M,
                GraduationYear = DateTime.Today.Year,
                CourseID = AvailableCourses.FirstOrDefault()?.CourseID ?? 0,
                AdmissionDate = DateOnly.FromDateTime(DateTime.Today),
                ApplicationStatus = "Pending"
            };

            // Force radio flags to sync with default state values
            OnPropertyChanged(nameof(IsMale));
            OnPropertyChanged(nameof(IsFemale));
        }

        [RelayCommand]
        private void SaveAndExit()
        {
            if (StudentRecord is null) return;

            // 1. Core Mandatory Fields Verification
            if (string.IsNullOrWhiteSpace(StudentRecord.FullName) ||
                string.IsNullOrWhiteSpace(StudentRecord.Email) ||
                string.IsNullOrWhiteSpace(StudentRecord.PhoneNum))
            {
                MessageBox.Show("Validation Error: Full Name, Email, and Phone Number fields are mandatory.",
                                "Validation Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Comprehensive Email Regex Schema Verification Match
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(StudentRecord.Email, emailPattern))
            {
                MessageBox.Show("The email address structure entered is invalid.\n\nPlease ensure it contains a proper domain prefix and suffix layout (e.g., student@school.com).",
                                "Invalid Email Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Structured Segment-Length Phone Number Verification Match
            string rawDigits = System.Text.RegularExpressions.Regex.Replace(StudentRecord.PhoneNum, @"[^\d+]", "");
            if (rawDigits.Length < 10 || rawDigits.Length > 14)
            {
                MessageBox.Show("Telephone validation failed.\n\nPlease follow standard segmentation schemas:\n" +
                                "- Country Identifier Code: +XX or XXXX\n" +
                                "- Local Mobile/Service Provider Group: 3 Digits\n" +
                                "- Primary Endpoint Base Line Number: 7 Digits",
                                "Invalid Phone Structure", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 4. Connect to database context and perform unique constraint screens
            using var db = new SchoolDBContext(_dbOptions);
            try
            {
                bool duplicatePhone = db.StudentAdmissions.Any(s => s.PhoneNum == StudentRecord.PhoneNum);
                bool duplicateEmail = db.StudentAdmissions.Any(s => s.Email == StudentRecord.Email);

                if (duplicatePhone || duplicateEmail)
                {
                    MessageBox.Show("Validation Check Stopped: An applicant with this Phone Number or Email is already registered.",
                                    "Constraint Collision", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }

                db.StudentAdmissions.Add(StudentRecord);
                db.SaveChanges();

                MessageBox.Show("Admission Form filed and committed securely to SQL Server database!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                if (Application.Current.MainWindow.DataContext is MainViewModel mainVM)
                {
                    mainVM.CurrentView = new WelcomeViewModel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Transaction Aborted:\n\n{ex.InnerException?.Message ?? ex.Message}",
                                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
