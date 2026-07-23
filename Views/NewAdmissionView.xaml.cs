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

        // Intercepts and allows ONLY Alphabetic Letters and Spaces (Name Field)
        private void AlphabeticTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z ]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Intercepts and allows Letters, Numbers, Spaces, and Punctuation for Real Addresses
        private void AlphanumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9 .,\-/]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Intercepts and allows Integer Digits (0-9) Only (Year, Postal Code)
        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Intercepts and allows valid email characters
        private void EmailTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.@_\-]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Intercepts and allows standard phone number layout characters
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9()\-+ ]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        // Strict Masked GPA Bounds Controller (0.00 - 4.00)
        private void GPATextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        // Automatically cleans up leading/trailing gaps and accidental double spaces
        private void TextClean_LostFocus(object sender, RoutedEventArgs e)
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
