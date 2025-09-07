using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentation.Behaviors
{
    /// <summary>
    /// Attachable behavior that constrains TextBox input to numeric / currency characters (digits, one dot, optional $ and commas).
    /// Parsing/formatting still handled by CurrencyConverter; this only keeps raw keystrokes reasonable.
    /// </summary>
    public static class NumericTextBoxBehavior
    {
        public static readonly DependencyProperty CurrencyInputProperty =
            DependencyProperty.RegisterAttached(
                "CurrencyInput",
                typeof(bool),
                typeof(NumericTextBoxBehavior),
                new PropertyMetadata(false, OnCurrencyInputChanged));

        public static void SetCurrencyInput(DependencyObject element, bool value) => element.SetValue(CurrencyInputProperty, value);
        public static bool GetCurrencyInput(DependencyObject element) => (bool)element.GetValue(CurrencyInputProperty);

        private static void OnCurrencyInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb) return;

            if ((bool)e.NewValue)
            {
                tb.PreviewTextInput += OnPreviewTextInput;
                DataObject.AddPastingHandler(tb, OnPaste);
            }
            else
            {
                tb.PreviewTextInput -= OnPreviewTextInput;
                DataObject.RemovePastingHandler(tb, OnPaste);
            }
        }

        private static readonly Regex Allowed = new(@"^[0-9\.\,\$\s-]+$", RegexOptions.Compiled);
        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Allowed.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }
            // Prevent more than one dot in the combined text
            if (sender is TextBox tb && e.Text.Contains('.'))
            {
                var selStart = tb.SelectionStart;
                var selLen = tb.SelectionLength;
                var current = tb.Text[..selStart] + e.Text + tb.Text[(selStart + selLen)..];
                if (current.Split('.').Length > 2) e.Handled = true;
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText)) return;
            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string ?? "";
            if (!Allowed.IsMatch(text)) e.CancelCommand();
        }
    }
}
