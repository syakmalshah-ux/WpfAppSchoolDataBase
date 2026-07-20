using CommunityToolkit.Mvvm.ComponentModel; // Enables modern UI notifications
using WpfAppSchoolDataBase.Models; // Points to your newly generated EF Core folder
using System.Collections.ObjectModel;

using System.Linq;

namespace WpfAppSchoolDataBase.ViewModels
{
    // Inheriting from ObservableObject automatically gives this class notification powers
    public class MainViewModel : ObservableObject
    {
        private StudentAdmission? _selectedStudent;

        // Collections that notify the WPF DataGrid when rows load or change
        public ObservableCollection<Course> Courses { get; set; }
        public ObservableCollection<StudentAdmission> Students { get; set; }

        // The active student highlighted in your UI grid
        public StudentAdmission? SelectedStudent
        {
            get => _selectedStudent;
            set => SetProperty(ref _selectedStudent, value); // Auto-notifies your textboxes instantly
        }

        public MainViewModel()
        {
            Courses = new ObservableCollection<Course>();
            Students = new ObservableCollection<StudentAdmission>();
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            // Build the configuration options manually.
            var optionsBuilder = new DbContextOptionsBuilder<SchoolDBContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=SchoolDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

            // Replace 'SchoolDBContext' with the exact name your EF Core tool generated
            // Pass the options directly into the constructor.

            using (var db = new SchoolDBContext(optionsBuilder.Options))
            {
                // Fetch the real live records out of your SQL Express engine
                var liveCourses = db.Courses.ToList();
                var liveStudents = db.StudentAdmissions.ToList();

                foreach (var course in liveCourses) Courses.Add(course);
                foreach (var student in liveStudents) Students.Add(student);
            }
        }
    }
}
