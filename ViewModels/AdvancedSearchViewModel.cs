using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WpfAppSchoolDataBase.Models;

namespace WpfAppSchoolDataBase.ViewModels
{
    public partial class AdvancedSearchViewModel : ViewModelBase
    {
        private readonly DbContextOptions<SchoolDBContext> _dbOptions;

        // Deep copy cloning layer used for absolute rollback safety
        private StudentAdmission? _clonedStudentBackup;

        // State Trackers bound to View elements
        private bool _isSearchById = true;
        private bool _isSearchByName = false;
        private string _searchIdInput = string.Empty;
        private string _searchNameInput = string.Empty;
        private bool _isFormLocked = true;
        private string _tickerMessage = "Ready. Input query constraints above to look up records.";
        private StudentAdmission? _studentRecord;

        public ObservableCollection<Course> AvailableCourses { get; } = new ObservableCollection<Course>();

        public bool IsSearchById
        {
            get => _isSearchById;
            set
            {
                if (SetProperty(ref _isSearchById, value))
                {
                    if (value) IsSearchByName = false;
                }
            }
        }

        public bool IsSearchByName
        {
            get => _isSearchByName;
            set
            {
                if (SetProperty(ref _isSearchByName, value))
                {
                    if (value) IsSearchById = false;
                }
            }
        }

        public string SearchIdInput
        {
            get => _searchIdInput;
            set => SetProperty(ref _searchIdInput, value);
        }

        public string SearchNameInput
        {
            get => _searchNameInput;
            set => SetProperty(ref _searchNameInput, value);
        }

        public bool IsFormLocked
        {
            get => _isFormLocked;
            set => SetProperty(ref _isFormLocked, value);
        }

        public string TickerMessage
        {
            get => _tickerMessage;
            set => SetProperty(ref _tickerMessage, value);
        }

        public StudentAdmission? StudentRecord
        {
            get => _studentRecord;
            set => SetProperty(ref _studentRecord, value);
        }

        // Dual Cascading Constructors for Dependency Injection or Design-Time
        public AdvancedSearchViewModel(DbContextOptions<SchoolDBContext> dbOptions)
        {
            _dbOptions = dbOptions;
            InitializeSearchModule();
        }

        public AdvancedSearchViewModel() : this(new DbContextOptionsBuilder<SchoolDBContext>()
            .UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=SchoolDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
            .Options)
        {
        }

        private void InitializeSearchModule()
        {
            using var db = new SchoolDBContext(_dbOptions);
            foreach (var course in db.Courses.ToList())
            {
                AvailableCourses.Add(course);
            }

            StudentRecord = new StudentAdmission();
        }

        // ==================== SEARCH ENGINE ====================
        [RelayCommand]
        private void ExecuteSearch()
        {
            IsFormLocked = true;
            using var db = new SchoolDBContext(_dbOptions);

            StudentAdmission? foundStudent = null;

            if (IsSearchById)
            {
                if (int.TryParse(SearchIdInput, out int targetId))
                {
                    foundStudent = db.StudentAdmissions.FirstOrDefault(s => s.StudentID == targetId);
                }
            }
            else // Search by Name
            {
                if (!string.IsNullOrWhiteSpace(SearchNameInput))
                {
                    foundStudent = db.StudentAdmissions.FirstOrDefault(s =>
                        s.FullName.ToLower() == SearchNameInput.Trim().ToLower());
                }
            }

            if (foundStudent != null)
            {
                StudentRecord = foundStudent;
                // ==================== ADD CODE HERE ====================
                NotifyRadioButtonProperties();
                // =======================================================
                TickerMessage = $"Success: Record for ID {StudentRecord.StudentID} loaded.";
            }
            else
            {
                StudentRecord = new StudentAdmission();
                // Optional: Refresh radio buttons for empty state as well
                NotifyRadioButtonProperties();
                TickerMessage = "Record Not Found";
                MessageBox.Show("The database query returned no matching entities.", "Search Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ==================== EDIT STATE TOGGLES ====================
        [RelayCommand]
        private void ChangeToEditState()
        {
            if (StudentRecord is null || StudentRecord.StudentID == 0) return;

            // Deep clone backup
            _clonedStudentBackup = new StudentAdmission
            {
                StudentID = StudentRecord.StudentID,
                FullName = StudentRecord.FullName,
                Email = StudentRecord.Email,
                Gender = StudentRecord.Gender,
                DateOfBirth = StudentRecord.DateOfBirth,
                PhoneNum = StudentRecord.PhoneNum,
                StreetAddress = StudentRecord.StreetAddress,
                City = StudentRecord.City,
                StateProvince = StudentRecord.StateProvince,
                PostalCode = StudentRecord.PostalCode,
                PrevSchoolName = StudentRecord.PrevSchoolName,
                GPA = StudentRecord.GPA,
                GraduationYear = StudentRecord.GraduationYear,
                CourseID = StudentRecord.CourseID,
                AdmissionDate = StudentRecord.AdmissionDate,
                ApplicationStatus = StudentRecord.ApplicationStatus
            };

            IsFormLocked = false;
            TickerMessage = "Modification Engine Active. Edit fields and execute update.";
        }

        [RelayCommand]
        private void CancelEdit()
        {
            if (_clonedStudentBackup != null && StudentRecord != null)
            {
                // Re-instantiate to trigger WPF property updates cleanly
                StudentRecord = new StudentAdmission
                {
                    StudentID = _clonedStudentBackup.StudentID,
                    FullName = _clonedStudentBackup.FullName,
                    Email = _clonedStudentBackup.Email,
                    Gender = _clonedStudentBackup.Gender,
                    DateOfBirth = _clonedStudentBackup.DateOfBirth,
                    PhoneNum = _clonedStudentBackup.PhoneNum,
                    StreetAddress = _clonedStudentBackup.StreetAddress,
                    City = _clonedStudentBackup.City,
                    StateProvince = _clonedStudentBackup.StateProvince,
                    PostalCode = _clonedStudentBackup.PostalCode,
                    PrevSchoolName = _clonedStudentBackup.PrevSchoolName,
                    GPA = _clonedStudentBackup.GPA,
                    GraduationYear = _clonedStudentBackup.GraduationYear,
                    CourseID = _clonedStudentBackup.CourseID,
                    AdmissionDate = _clonedStudentBackup.AdmissionDate,
                    ApplicationStatus = _clonedStudentBackup.ApplicationStatus
                };

                // ==================== ADD CODE HERE ====================
                NotifyRadioButtonProperties();
                // =======================================================
            }

            IsFormLocked = true;
            TickerMessage = "Modification routine aborted. Previous properties restored.";
        }

        // ==================== UPDATE TRANSACTION ====================
        [RelayCommand]
        private void UpdateChanges()
        {
            if (StudentRecord is null || StudentRecord.StudentID == 0) return;

            using var db = new SchoolDBContext(_dbOptions);
            try
            {
                db.StudentAdmissions.Update(StudentRecord);
                db.SaveChanges();

                IsFormLocked = true;
                TickerMessage = "Success: Academic record modified and updated.";
                MessageBox.Show("Modifications written back to database successfully.", "Database Synced", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update Failure:\n\n{ex.Message}", "Transaction Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==================== DELETE TRANSACTION ====================
        [RelayCommand]
        private void ExecuteDelete()
        {
            if (StudentRecord is null || StudentRecord.StudentID == 0) return;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to permanently delete this record?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var db = new SchoolDBContext(_dbOptions);
                try
                {
                    var target = db.StudentAdmissions.Find(StudentRecord.StudentID);
                    if (target != null)
                    {
                        db.StudentAdmissions.Remove(target);
                        db.SaveChanges();
                    }

                    StudentRecord = new StudentAdmission();
                    TickerMessage = "Success: Record permanently purged.";
                    MessageBox.Show("Entity completely erased from database.", "Purge Finished", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Deletion Aborted:\n\n{ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                TickerMessage = "Deletion command dropped. Record untouched.";
            }
        }

        // Inside AdvancedSearchViewModel.cs

        // ==================== GENDER RADIO BUTTON HELPERS ====================
        public bool IsGenderMale
        {
            get => StudentRecord?.Gender?.Equals("Male", StringComparison.OrdinalIgnoreCase) ?? false;
            set
            {
                if (value && StudentRecord != null)
                {
                    StudentRecord.Gender = "Male";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsGenderFemale));
                }
            }
        }

        public bool IsGenderFemale
        {
            get => StudentRecord?.Gender?.Equals("Female", StringComparison.OrdinalIgnoreCase) ?? false;
            set
            {
                if (value && StudentRecord != null)
                {
                    StudentRecord.Gender = "Female";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsGenderMale));
                }
            }
        }

        // ==================== STATUS RADIO BUTTON HELPERS ====================
        public bool IsStatusPending
        {
            get => StudentRecord?.ApplicationStatus?.Equals("Pending", StringComparison.OrdinalIgnoreCase) ?? true;
            set
            {
                if (value && StudentRecord != null)
                {
                    StudentRecord.ApplicationStatus = "Pending";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsStatusApproved));
                }
            }
        }

        public bool IsStatusApproved
        {
            get => StudentRecord?.ApplicationStatus?.Equals("Approved", StringComparison.OrdinalIgnoreCase) ?? false;
            set
            {
                if (value && StudentRecord != null)
                {
                    StudentRecord.ApplicationStatus = "Approved";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsStatusPending));
                }
            }
        }

        // ==================== ADD THIS METHOD ====================
        private void NotifyRadioButtonProperties()
        {
            OnPropertyChanged(nameof(IsGenderMale));
            OnPropertyChanged(nameof(IsGenderFemale));
            OnPropertyChanged(nameof(IsStatusPending));
            OnPropertyChanged(nameof(IsStatusApproved));
        }
        // =========================================================
    }
}