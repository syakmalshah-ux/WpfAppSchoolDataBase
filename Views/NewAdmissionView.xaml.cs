using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfAppSchoolDataBase.Views
{
    public partial class NewAdmissionView : UserControl
    {
        public NewAdmissionView()
        {
            InitializeComponent();
        }

        // ==========================================
        // LAYER 1: KEYSTROKE PREVIEW INTERCEPTORS (0-9, A-Z, etc)
        // ==========================================

        // Allows ONLY Alphabetic Letters and Spaces (Name Field)
        private void AlphabeticTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-A-Za-z ]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Allows Letters, Numbers, and Spaces (Address, City, Province, School)
        private void AlphanumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9 ]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Allows Integer Digits (0-9) Only (Year, Postal Code)
        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Allows Email Valid Construction Elements
        private void EmailTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.@_\-]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Allows Telephone Valid Construction Elements
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9()\-+ ]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Strict Masked GPA Bounds Controller (0.00 - 4.00)
        private void GpaTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string fullProposedText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

                if (fullProposedText.Length > 4) { e.Handled = true; return; }

                Regex structureRegex = new Regex(@"^[0-9]?\.?[0-9]{0,2}$");
                if (!structureRegex.IsMatch(fullProposedText)) { e.Handled = true; return; }

                if (double.TryParse(fullProposedText, out double parsedValue))
                {
                    if (parsedValue < 0.00 || parsedValue > 4.00) { e.Handled = true; return; }
                }
            }
        }

        // ==========================================
        // LAYER 2: LOSS OF FOCUS STRING FORMAT CLEANERS
        // ==========================================

        // Clean double spaces and edge spaces for alphabetic and alphanumeric text boxes
        private void TextClean_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text.Trim();
                // Replace double spaces with single space using Regex string replace manipulation
                text = Regex.Replace(text, @"\s+", " ");
                textBox.Text = text;
            }
        }
    }
}
