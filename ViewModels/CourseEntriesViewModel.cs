using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WpfAppSchoolDataBase.Models;

namespace WpfAppSchoolDataBase.ViewModels
{
    public partial class CourseEntriesViewModel : ViewModelBase
    {
        private readonly DbContextOptions<SchoolDBContext> _dbOptions;

        // FIXED: Flat tracking properties matching your exact 4 database data fields
        public string CourseCode { get => field; set => SetProperty(ref field, value); } = "";
        public string CourseName { get => field; set => SetProperty(ref field, value); } = "";
        public string SelectedDepartment { get => field; set => SetProperty(ref field, value); } = "Computer Science";
        public int SelectedCredits { get => field; set => SetProperty(ref field, value); } = 3;

        // Auto-calculating dynamic header metadata matching the institution banner
        public string LiveCalendarDate => $"Registration Date: {DateTime.Today:MMMM dd, yyyy}";

        // Fully populated, error-free list collections using .NET 10 expressions
        public ObservableCollection<string> DepartmentCatalog { get; } = ["Computer Science", "Mathematics", "Business Administration", "Natural Sciences", "Humanities"];
        public ObservableCollection<int> CreditValueOptions { get; } =[1, 2, 3, 4, 5];

        // Runtime constructor mirroring your working NewAdmissionViewModel architecture
        public CourseEntriesViewModel(DbContextOptions<SchoolDBContext> dbOptions)
        {
            _dbOptions = dbOptions;
            InitializeNewCourseForm();
        }

        // Visual Studio Designer fallback constructor mapping
        public CourseEntriesViewModel() : this(new DbContextOptionsBuilder<SchoolDBContext>()
            .UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=SchoolDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
            .Options)
        {
        }
        // Chunk M2: Data Reset and Explicit Multi-Column Database Commit Pipeline
        private void InitializeNewCourseForm()
        {
            // Sets clean baseline states on startup to clear old inputs
            CourseCode = "";
            CourseName = "";
            SelectedDepartment = "Computer Science";
            SelectedCredits = 3;
        }

        [RelayCommand]
        private void SaveCourseToCatalog()
        {
            // Strip accidental outer edge padding spaces seamlessly before processing guards
            CourseCode = CourseCode?.Trim() ?? "";
            CourseName = CourseName?.Trim() ?? "";

            // 1. Core Mandatory Presence Grid Screen Guards
            if (string.IsNullOrWhiteSpace(CourseCode) || string.IsNullOrWhiteSpace(CourseName))
            {
                MessageBox.Show("Validation Error: Both Course Code and Course Title are mandatory fields.",
                                "Missing Parameters", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Local Database Unique Constraint Check Block
            using var db = new SchoolDBContext(_dbOptions);
            try
            {
                // Pre-verify uniqueness bounds to avoid unique index SQL collisions
                bool duplicateCode = db.Courses.Any(c => c.CourseCode.ToLower() == CourseCode.ToLower());
                bool duplicateName = db.Courses.Any(c => c.CourseName.ToLower() == CourseName.ToLower());

                if (duplicateCode || duplicateName)
                {
                    MessageBox.Show("Academic Registry Intercept: A module with this specific Course Code or Course Title is already registered.",
                                    "Constraint Collision", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }

                // 3. COMPLETE EXPLICIT OBJECT CONSTRUCT INITIALIZATION (NO PLACEHOLDERS / NO COMMENTS)
                var newRecord = new Course
                {
                    CourseCode = this.CourseCode,
                    CourseName = this.CourseName,
                    Department = this.SelectedDepartment,
                    Credits = this.SelectedCredits
                };

                // Commit row values straight into local DB instance schema tables
                db.Courses.Add(newRecord);
                db.SaveChanges();

                MessageBox.Show("New Academic Program committed securely to SQL Server database catalog!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Seamless route view navigation switches back to home page dashboard layout canvas
                if (Application.Current.MainWindow.DataContext is MainViewModel mainVM)
                {
                    mainVM.CurrentView = new WelcomeViewModel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Execution Dropped:\n\n{ex.InnerException?.Message ?? ex.Message}",
                                "SQL Write Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
