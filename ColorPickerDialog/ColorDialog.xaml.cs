using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ColorPickerDialog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ColorDialog : Window
    {
        public static DependencyProperty
            RProperty = DependencyProperty.Register("R", typeof(double), typeof(ColorDialog)),
            GProperty = DependencyProperty.Register("G", typeof(double), typeof(ColorDialog)),
            BProperty = DependencyProperty.Register("B", typeof(double), typeof(ColorDialog));

        public double R
        {
            get => (double)GetValue(RProperty);
            set => SetValue(RProperty, value);
        }
        public double G
        {
            get => (double)GetValue(GProperty);
            set => SetValue(GProperty, value);
        }
        public double B
        {
            get => (double)GetValue(BProperty);
            set => SetValue(BProperty, value);
        }

        public ColorDialog(bool isPicker)
        {
            InitializeComponent();
            if (isPicker)
            {
                button_Ok.Visibility = Visibility.Visible;
                button_Cancel.Visibility = Visibility.Visible;
                button_GenerateRgbString.Visibility = Visibility.Hidden;
                button_ReadRgbString.Visibility = Visibility.Hidden;
            }
            else
            {
                button_Ok.Visibility = Visibility.Hidden;
                button_Cancel.Visibility = Visibility.Hidden;
                button_GenerateRgbString.Visibility = Visibility.Visible;
                button_ReadRgbString.Visibility = Visibility.Visible;
            }
            txtR.Text = "255";
            txtG.Text = "255";
            txtB.Text = "255";
            var rBinding = new Binding
            {
                Source = txtR,
                Path = new PropertyPath("Text"),
                Mode = BindingMode.OneWay
            };
            var gBinding = new Binding
            {
                Source = txtG,
                Path = new PropertyPath("Text"),
                Mode = BindingMode.OneWay
            };
            var bBinding = new Binding
            {
                Source = txtB,
                Path = new PropertyPath("Text"),
                Mode = BindingMode.OneWay
            };
            SetBinding(RProperty, rBinding);
            SetBinding(GProperty, gBinding);
            SetBinding(BProperty, bBinding);
            ColourPreview.Background = new SolidColorBrush(Colors.White);
        }

        public event ColorRgbChangedEventHandler? ColorRgbChanged;

        private void Texts_TextChanged(object sender, TextChangedEventArgs e) => ColourPreview.Background = new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ColourPreview.Background = new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));

        private void Button_GenerateRgbString_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject($"{(byte)R:d3},{(byte)G:d3},{(byte)B:d3}");
        }

        private void Button_ReadRgbString_Click(object sender, RoutedEventArgs e)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(DataFormats.Text))
            {
                string rgbString = (string)dataObject.GetData(DataFormats.Text);
                Regex regex = RgbRegex();
                if (rgbString is not null && regex.IsMatch(rgbString))
                {
                    string[] rgbStrings = rgbString.Split(',');
                    if (byte.TryParse(rgbStrings[0], out byte r) && byte.TryParse(rgbStrings[1], out byte g) && byte.TryParse(rgbStrings[2], out byte b))
                    {
                        txtR.Text = r.ToString();
                        txtG.Text = g.ToString();
                        txtB.Text = b.ToString();
                    }
                }
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            ColorRgbChanged?.Invoke(this, new ColorRgbChangedEventArgs((byte)R, (byte)G, (byte)B));
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        [GeneratedRegex("\\d{3}\\s*,\\s*\\d{3}\\s*,\\s*\\d{3}")]
        private static partial Regex RgbRegex();
    }

    public delegate void ColorRgbChangedEventHandler(object sender, ColorRgbChangedEventArgs e);

    public class ColorRgbChangedEventArgs : EventArgs
    {
        public ColorRgbChangedEventArgs(byte r, byte g, byte b)
        {
            (R, G, B) = (r, g, b);
        }

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
    }
}
