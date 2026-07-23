using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfAppSchoolDataBase.Views
{
    public partial class CourseEntriesView : UserControl
    {
        public CourseEntriesView()
        {
            InitializeComponent();
        }

        // 1. Course Code Mask: Allows ONLY Upper/Lower Case Letters, Numbers, and Hyphens (e.g. CS-101)
        private void CourseCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9\-]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // 2. Alphanumeric Text Mask: Allows any mix of letters, numbers, spaces, and punctuation for titles
        private void AlphaText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9 .,\-/]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // 3. Positive Integer Mask: Allows strictly digits 0-9 for seat counts and financial whole strings
        private void CapacityDigits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // 4. Boundary Range Verification: Validates student seating capacity metrics between 5 and 120
        private void Capacity_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
            {
                if (value < 5 || value > 120)
                {
                    MessageBox.Show("Operational Constraint Violation:\n\nClassroom capacity limitations mandate a registry range between 5 and 120 allocations.",
                                    "Out of Bounds Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = "30"; // Safe industry-standard fallback default value
                    textBox.Focus();
                }
            }
        }

        // 5. Spacing Format Cleaner: Silently strips leading/trailing spaces and internal double spaces
        private void StringClean_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text.Trim();
                text = Regex.Replace(text, @"\s+", " ");
                textBox.Text = text;
            }
        }
    }
}
