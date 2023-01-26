﻿using ColorPickerDialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Ink
{
    /* 指定属性值的类型 */
    public enum InkPropertyValueType
    {
        Boolean,    // 显示为一个CheckBox
        List,   // 给定值列表，显示为ComboBox
        Input   // 用户自行输入，显示为TextBox
    }

    /* InkProperty类中InkPropertyValueChanged事件的EventArgs */
    public class InkPropertyValueChangedEventArgs : EventArgs
    {
        public string PropertyName { get; } // 值发生改变的属性名称
        public string NewValue { get; } // 改变后的属性值
        public InkPropertyValueType ValueType { get; }

        public InkPropertyValueChangedEventArgs(string name, string newValue, InkPropertyValueType valueType)
        {
            PropertyName = name;
            NewValue = newValue;
            ValueType = valueType;
        }
    }

    public delegate void InkPropertyValueChangedEventHandler(object sender, InkPropertyValueChangedEventArgs e);

    /* InkObject对象的属性 */
    public class InkProperty : INotifyPropertyChanged
    {
        private string value = string.Empty;
        private InkProperty? valueSource;   // 启用ValueSync时属性值的来源，即绑定同步的另一个InkProperty对象
        private InkObject? valueSourceObject;

        public string Name { get; }
        public string Value
        {
            get { return value; }
            set
            {
                SetValue(value, true);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                InkPropertyValueChanged?.Invoke(this, new InkPropertyValueChangedEventArgs(Name, Value, ValueType));
            }
        }
        public ObservableCollection<string> ValueHistory { get; } = new();   // 用于实现回溯属性值
        public InkProperty? ValueSource
        {
            get { return valueSource; }
            private set
            {
                valueSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueSource)));
            }
        }
        public InkObject? ValueSourceObject
        {
            get { return valueSourceObject; }
            set
            {
                valueSourceObject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueSourceObject)));
            }
        }
        public InkPropertyValueType ValueType { get; }
        public string DefaultValue { get; }
        public string[]? ValueList { get; init; }   // 若ValueType是List，该列表存储给定的属性值；否则为null
        public bool ValueSyncEnabled { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;  // 实现INotifyPropertyChanged接口
        public event InkPropertyValueChangedEventHandler? InkPropertyValueChanged;  // 自定义用于同步属性值的事件

        public InkProperty(string name, InkPropertyValueType valueType, string defaultValue)
        {
            Name = name;
            ValueType = valueType;
            Value = defaultValue;
            DefaultValue = defaultValue;
            ValueSyncEnabled = false;
        }

        private void SetValue(string value, bool addToValueHistory = true)
        {
            this.value = value;
            if (addToValueHistory)
            {
                ValueHistory.Insert(0, value);
            }
        }

        public void RestoreValue(int index)
        {
            if (index < ValueHistory.Count)
            {
                SetValue(ValueHistory[index], false);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                InkPropertyValueChanged?.Invoke(this, new InkPropertyValueChangedEventArgs(Name, Value, ValueType));
            }
        }

        public void SyncValueWith(InkProperty property, InkObject sourceObject)
        {
            if (property.ValueType == this.ValueType && property.ValueSource != this)
            {
                property.InkPropertyValueChanged += Property_InkPropertyValueChanged;   // 同步是通过订阅事件完成的
                // 可能出现的问题：O1订阅O2，O2订阅O3，……，O(n)订阅O1，然后StackOverflow
                Value = property.Value;
                ValueSource = property;
                valueSourceObject = sourceObject;
                ValueSyncEnabled = true;
            }
        }

        public void DisableValueSynchronization()
        {
            if (ValueSource is not null)
            {
                ValueSource.InkPropertyValueChanged -= Property_InkPropertyValueChanged;
                ValueSource = null;
                ValueSourceObject = null;
                ValueSyncEnabled = false;
            }
        }

        private void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e)
        {
            Value = e.NewValue;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract partial class InkObject : INotifyPropertyChanged
    {
        protected string name = "InkObject";

        protected static Color GetColourFromString(string rgb)
        {
            Regex regex = RgbRegex();
            if (rgb is not null && regex.IsMatch(rgb))
            {
                string[] rgbStrings=rgb.Split(',');
                if (byte.TryParse(rgbStrings[0], out byte r) && byte.TryParse(rgbStrings[1], out byte g) && byte.TryParse(rgbStrings[2], out byte b))
                {
                    return Color.FromRgb(r, g, b);
                }
                else
                {
                    return Colors.Transparent;
                }
            }
            else
            {
                return Colors.Transparent;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public virtual double X // 在Canvas上的横坐标
        {
            get { return ShownElement.Margin.Left; }
            set
            {
                ShownElement.Margin = new(value, ShownElement.Margin.Top, ShownElement.Margin.Right, ShownElement.Margin.Bottom);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }
        public virtual double Y // 在Canvas上的纵坐标
        {
            get { return ShownElement.Margin.Top; }
            set
            {
                ShownElement.Margin = new(ShownElement.Margin.Left, value, ShownElement.Margin.Right, ShownElement.Margin.Bottom);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }
        public virtual double Width
        {
            get { return ShownElement.Width; }
            set
            {
                ShownElement.Width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }
        public virtual double Height
        {
            get { return ShownElement.Height; }
            set
            {
                ShownElement.Height = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            }
        }
        public virtual bool Visible
        {
            get { return ShownElement.IsVisible; }
            set
            {
                ShownElement.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visible)));
            }
        }
        public abstract string Type { get; }    // 并没有很大的实际意义
        public abstract Dictionary<string, InkProperty> Properties { get; }

        protected abstract FrameworkElement ShownElement { get; }   // 实际显示出来的控件

        public event PropertyChangedEventHandler? PropertyChanged;
        public event MouseButtonEventHandler? Click;    // 控件被点击时触发，通常用于通知前端获取焦点

        protected InkObject(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void AddToPage(Canvas page)
        {
            page.Children.Add(ShownElement);
        }

        public void RemoveFromPage(Canvas page)
        {
            page.Children.Remove(ShownElement);
        }

        protected abstract void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e);

        protected void RaiseClickEvent(object sender, MouseButtonEventArgs e)
        {
            Click?.Invoke(sender, e);
        }

        [GeneratedRegex("^\\d{3}\\s*,\\s*\\d{3}\\s*,\\s*\\d{3}$")]
        private static partial Regex RgbRegex();
    }

    public class InkPage : INotifyPropertyChanged   // 抽象出的Page对象
    {
        private string name = string.Empty;

        public InkPage(string name)
        {
            Name = name;
            Objects = new ObservableCollection<InkObject>();
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public ObservableCollection<InkObject> Objects { get; } // 包含页面上所有InkObject，Page的主体
        public Brush Background { get; set; } = new SolidColorBrush(Colors.White);

        public event PropertyChangedEventHandler? PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }

    public class InkTextBox : InkObject
    {
        private readonly TextBox textBox = new() { AcceptsReturn = true };    // 在InkTextBox获得焦点时显示以直接编辑文本
        private TextBlock textBlock = new();    // 无焦点时显示

        public InkTextBox(string name) : base(name)
        {
            Properties = new Dictionary<string, InkProperty>()
            {
                { "Text", new InkProperty("Text", InkPropertyValueType.Input, "Text here") },

                { "Alignment", new InkProperty("Alignment", InkPropertyValueType.List, "Left")
                { ValueList = new string[] { "Left", "Right", "Center", "Justify" } } },

                { "TextWrapping", new InkProperty("TextWrapping", InkPropertyValueType.Boolean, "True") },
                { "FontSize", new InkProperty("FontSize", InkPropertyValueType.Input, "18") },

                { "FontFamily", new InkProperty("FontFamily", InkPropertyValueType.List, "Times New Roman")
                { ValueList = GetInstalledFonts() } },

                { "FontWeight", new InkProperty("FontWeight", InkPropertyValueType.List, "Regular")
                { ValueList = new string[] { "Light", "Regular", "Bold" } } },

                { "Italic", new InkProperty("Italic", InkPropertyValueType.Boolean, "False") },

                { "Lines", new InkProperty("Lines", InkPropertyValueType.List, "NoLine")
                { ValueList = new string[] { "NoLine", "UnderLine", "OverLine", "Strikethrough" } } },

                { "Foreground", new InkProperty("Foreground", InkPropertyValueType.Input, "000,000,000") },
                { "Background", new InkProperty("Background", InkPropertyValueType.Input, string.Empty) }
            };
            foreach (InkProperty property in Properties.Values)
            {
                property.InkPropertyValueChanged += Property_InkPropertyValueChanged;   // 以便在Property更新时同时更新前端的显示
                property.Value = property.DefaultValue;
            }
            X = 514;
            Y = 114;
            BindTextBoxWithTextBlock();
            textBlock.MouseDown += TextBlock_MouseDown;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.MouseDown += (sender, e) => RaiseClickEvent(this, e);
            Binding binding = new()
            {
                Source = Properties["Text"],
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Value"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            textBox.SetBinding(TextBox.TextProperty, binding);
        }

        private bool IsTextBoxShown { get; set; } = false;

        public override string Type => "Text Box";

        public override Dictionary<string, InkProperty> Properties { get; }

        protected override FrameworkElement ShownElement => IsTextBoxShown ? textBox : textBlock;

        public override void AddToPage(Canvas page)
        {
            page.Children.Add(textBlock);
            page.Children.Add(textBox);
            textBox.Visibility = Visibility.Hidden;
        }

        private static string[] GetInstalledFonts()
        {
            ICollection<FontFamily> installedFonts = Fonts.SystemFontFamilies;
            string[] fonts = new string[installedFonts.Count];
            int i = 0;
            foreach (FontFamily fontFamily in installedFonts)
            {
                fonts[i] = fontFamily.ToString();
                i++;
            }
            return fonts;
        }

        /* 当TextBlock被点击时切换到TextBox */
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (textBlock.IsVisible && !Properties["Text"].ValueSyncEnabled)
            {
                textBlock.Visibility = Visibility.Hidden;
                textBox.Visibility = Visibility.Visible;
                IsTextBoxShown = true;
                textBox.Focus();
            }
            RaiseClickEvent(this, e);
        }

        /* TextBox失去焦点时切回TextBlock */
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlock.Visibility = Visibility.Visible;
            textBox.Visibility = Visibility.Hidden;
            IsTextBoxShown = false;
        }

        /* 响应后台属性值的更改 */
        protected override void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Text":
                    textBlock.Text = e.NewValue; break;
                case "Alignment":
                    SetTextAlignment(e.NewValue);
                    break;
                case "TextWrapping":
                    bool textWrapping = bool.Parse(e.NewValue);
                    textBlock.TextWrapping = textWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;
                    break;
                case "FontSize":
                    if (int.TryParse(e.NewValue, out int fontSize))
                    {
                        textBlock.FontSize = fontSize;
                    }
                    break;
                case "FontFamily":
                    if (e.NewValue is not null)
                    {
                        FontFamily fontFamily = new(e.NewValue);
                        textBlock.FontFamily = fontFamily;
                    }
                    break;
                case "FontWeight":
                    SetFontWeight(e.NewValue);
                    break;
                case "Italic":
                    bool italic = bool.Parse(e.NewValue);
                    textBlock.FontStyle = italic ? FontStyles.Italic : FontStyles.Normal;
                    break;
                case "Lines":
                    SetLine(e.NewValue);
                    break;
                case "Foreground":
                    SetForeground(e.NewValue);
                    break;
                case "Background":
                    SetBackground(e.NewValue);
                    break;
                default:
                    break;
            }
        }

        private void SetForeground(string foreground)
        {
            if (foreground == string.Empty)
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                textBlock.Foreground = new SolidColorBrush(GetColourFromString(foreground));
            }
        }

        private void SetBackground(string background)
        {
            if (background == string.Empty)
            {
                textBlock.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                textBlock.Background = new SolidColorBrush(GetColourFromString(background));
            }
        }

        private static Color GetCustomColour()
        {
            ColorDialog colorDialog = new(true);
            if (colorDialog.ShowDialog() == true)
            {
                return Color.FromRgb((byte)colorDialog.R, (byte)colorDialog.G, (byte)colorDialog.B);
            }
            else
            {
                return Colors.Transparent;
            }
        }

        private void SetTextAlignment(string textAlignment)
        {
            switch (textAlignment)
            {
                case "Left":
                    textBlock.TextAlignment = TextAlignment.Left;
                    break;
                case "Right":
                    textBlock.TextAlignment = TextAlignment.Right;
                    break;
                case "Center":
                    textBlock.TextAlignment = TextAlignment.Center;
                    break;
                case "Justify":
                    textBlock.TextAlignment = TextAlignment.Justify;
                    break;
                default:
                    break;
            }
        }

        private void SetFontWeight(string fontWeight)
        {
            switch (fontWeight)
            {
                case "Light":
                    textBlock.FontWeight = FontWeights.Light; break;
                case "Regular":
                    textBlock.FontWeight = FontWeights.Regular; break;
                case "Bold":
                    textBlock.FontWeight = FontWeights.Bold; break;
                default:
                    break;
            }
        }

        private void SetLine(string line)
        {
            switch (line)
            {
                case "NoLine":
                    textBlock.TextDecorations = null;
                    break;
                case "UnderLine":
                    textBlock.TextDecorations = TextDecorations.Underline;
                    break;
                case "OverLine":
                    textBlock.TextDecorations = TextDecorations.OverLine;
                    break;
                case "Strikethrough":
                    textBlock.TextDecorations = TextDecorations.Strikethrough;
                    break;
                default:
                    break;
            }
        }

        private void BindTextBoxWithTextBlock()
        {
            Binding binding = new()
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Text"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            textBox.SetBinding(TextBox.TextProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("TextAlignment")
            };
            textBox.SetBinding(TextBox.TextAlignmentProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("TextWrapping")
            };
            textBox.SetBinding(TextBox.TextWrappingProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("FontSize")
            };
            textBox.SetBinding(TextBox.FontSizeProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("FontFamily")
            };
            textBox.SetBinding(TextBox.FontFamilyProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("FontWeight")
            };
            textBox.SetBinding(TextBox.FontWeightProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("FontStyle")
            };
            textBox.SetBinding(TextBox.FontStyleProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("TextDecorations")
            };
            textBox.SetBinding(TextBox.TextDecorationsProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Foreground")
            };
            textBox.SetBinding(TextBox.ForegroundProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Background")
            };
            textBox.SetBinding(TextBox.BackgroundProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Margin")
            };
            textBox.SetBinding(TextBox.MarginProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Width")
            };
            textBox.SetBinding(TextBox.WidthProperty, binding);
            binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Height")
            };
            textBox.SetBinding(TextBox.HeightProperty, binding);
        }
    }

    public class InkImageBox : InkObject
    {
        private System.Windows.Controls.Image image = new()
        {
            Source = new BitmapImage(new Uri(@"/PowerInk.png", UriKind.Relative)),
            Stretch = Stretch.Uniform,
            ToolTip = "Right-click to load image"
        };

        public InkImageBox(string name) : base(name)
        {
            Properties = new Dictionary<string, InkProperty>
            {
                { "ImagePath", new InkProperty("ImagePath", InkPropertyValueType.Input, @"/PowerInk.png") },

                { "Stretch", new InkProperty("Stretch", InkPropertyValueType.List, "Uniform")
                { ValueList = new string[] { "Uniform", "Fill", "UniformToFill", "None" } } }
            };
            foreach (InkProperty property in Properties.Values)
            {
                property.InkPropertyValueChanged += Property_InkPropertyValueChanged;
                property.Value = property.DefaultValue;
            }
            X = 514;
            Y = 114;
            Width = 128;
            Height = 128;
            image.MouseDown += (sender, e) => RaiseClickEvent(this, e);
            image.MouseRightButtonUp += (sender, e) => MouseRightButtonUp?.Invoke(this, e);
        }

        protected override void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ImagePath":
                    SetImageSource(e.NewValue);
                    break;
                case "Stretch":
                    Stretch stretch = (Stretch)Enum.Parse(typeof(Stretch), e.NewValue);
                    image.Stretch = stretch;
                    break;
                default:
                    break;
            }
        }

        private void SetImageSource(string uri)
        {
            if (uri == string.Empty)
            {
                image.Source = new BitmapImage(new Uri(Properties["ImagePath"].DefaultValue, UriKind.Relative));
            }
            else if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri? resultUri))
            {
                try
                {
                    image.Source = new BitmapImage(resultUri);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("File is not an image! ", "Oops...", MessageBoxButton.OK, MessageBoxImage.Error);
                    Properties["ImagePath"].Value = string.Empty;
                }
            }
        }

        public override string Type => "Image Box";

        public override Dictionary<string, InkProperty> Properties { get; }

        protected override FrameworkElement ShownElement => image;

        public event MouseButtonEventHandler? MouseRightButtonUp;
    }

    public abstract class InkShape : InkObject
    {
        public InkShape(string name) : base(name)
        {
            Properties = new Dictionary<string, InkProperty>()
            {
                { "Stroke", new InkProperty("Stroke", InkPropertyValueType.Input, "000,000,000") },
                { "StrokeThickness", new InkProperty("StrokeThickness", InkPropertyValueType.Input, "1") },
                { "Fill", new InkProperty("Fill", InkPropertyValueType.Input, string.Empty) }
            };
            if (Properties is not null)
            {
                foreach (InkProperty property in Properties.Values)
                {
                    property.InkPropertyValueChanged += Property_InkPropertyValueChanged;
                    property.Value = property.DefaultValue;
                }
            }
            else
            {
                Properties = new Dictionary<string, InkProperty>();
            }
            X = 514;
            Y = 114;
            Shape.MouseDown += (sender, e) => RaiseClickEvent(this, e);
        }

        public override Dictionary<string, InkProperty> Properties { get; }

        protected abstract Shape Shape { get; }

        protected override void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Stroke":
                    SetStroke(e.NewValue);
                    break;
                case "StrokeThickness":
                    if (double.TryParse(e.NewValue, out double thickness) && thickness >= 0)
                    {
                        Shape.StrokeThickness = thickness;
                    }
                    break;
                case "Fill":
                    SetFill(e.NewValue);
                    break;
                default:
                    break;
            }
        }

        protected void SetStroke(string colour)
        {
            if (colour == string.Empty)
            {
                Shape.Stroke = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                Shape.Stroke = new SolidColorBrush(GetColourFromString(colour));
            }
        }

        protected void SetFill(string colour)
        {
            if (colour == string.Empty)
            {
                Shape.Fill = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                Shape.Fill = new SolidColorBrush(GetColourFromString(colour));
            }
        }
    }

    public class InkEllipse : InkShape
    {
        private readonly Ellipse ellipse = new();

        public InkEllipse(string name) : base(name)
        {
            Width = 514;
            Height = 114;
        }

        public override string Type => "Ellipse";

        protected override FrameworkElement ShownElement => ellipse;

        protected override Shape Shape => ellipse;
    }

    public class InkRectangle : InkShape
    {
        private readonly Rectangle rectangle = new();

        public InkRectangle(string name) : base(name)
        {
            Width = 514;
            Height = 114;
        }

        public override string Type => "Rectangle";

        protected override FrameworkElement ShownElement => rectangle;

        protected override Shape Shape => rectangle;
    }

    public class InkLine : InkShape
    {
        private readonly Line line = new();

        public InkLine(string name) : base(name)
        {
            Properties = new Dictionary<string, InkProperty>()
            {
                { "Stroke", new InkProperty("Stroke", InkPropertyValueType.Input, "000,000,000") },
                { "StrokeThickness", new InkProperty("StrokeThickness", InkPropertyValueType.Input, "1") },
                { "StartPoint", new InkProperty("StartPoint", InkPropertyValueType.Input, "0,114") },
                { "EndPoint", new InkProperty("EndPoint", InkPropertyValueType.Input, "514,0") },
            };
            foreach (InkProperty property in Properties.Values)
            {
                property.InkPropertyValueChanged += Property_InkPropertyValueChanged;
                property.Value = property.DefaultValue;
            }
        }

        public override string Type => "Line";

        public override Dictionary<string, InkProperty> Properties { get; }

        protected override Shape Shape => line;

        protected override FrameworkElement ShownElement => line;

        protected override void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Stroke":
                    SetStroke(e.NewValue);
                    break;
                case "StrokeThickness":
                    if (double.TryParse(e.NewValue, out double thickness) && thickness >= 0)
                    {
                        Shape.StrokeThickness = thickness;
                    }
                    break;
                case "StartPoint":
                    (line.X1, line.Y1) = GetCoordinateFromString(e.NewValue);
                    break;
                case "EndPoint":
                    (line.X2, line.Y2) = GetCoordinateFromString(e.NewValue);
                    break;
                default:
                    break;
            }
        }

        private static (double x, double y) GetCoordinateFromString(string coordinate)
        {
            string[] coordinates = coordinate.Split(',');
            if (coordinates.Length == 2 && double.TryParse(coordinates[0], out double xCoordinate) && double.TryParse(coordinates[1], out double yCoordinate))
            {
                return (xCoordinate, yCoordinate);
            }
            else
            {
                return (514, 114);
            }
        }
    }
}