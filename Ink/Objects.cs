﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ink
{
    public enum InkPropertyValueType
    {
        Boolean,
        List,
        Input
    }

    public class InkPropertyValueChangedEventArgs : EventArgs
    {
        public string Name { get; }
        public string NewValue { get; }
        public InkPropertyValueType ValueType { get; }

        public InkPropertyValueChangedEventArgs(string name, string newValue, InkPropertyValueType valueType)
        {
            Name = name;
            NewValue = newValue;
            ValueType = valueType;
        }
    }

    public delegate void InkPropertyValueChangedEventHandler(object sender, InkPropertyValueChangedEventArgs e);

    public class InkProperty : INotifyPropertyChanged
    {
        private string value = "";
        private InkProperty? valueSource;

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
        public Stack<string> ValueHistory { get; } = new(64);
        public InkProperty? ValueSource
        {
            get { return valueSource; }
            private set
            {
                valueSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueSource)));
            }
        }
        public InkPropertyValueType ValueType { get; }
        public string DefaultValue { get; }
        public string[]? ValueList { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event InkPropertyValueChangedEventHandler? InkPropertyValueChanged;

        public InkProperty(string name, InkPropertyValueType valueType, string defaultValue)
        {
            Name = name;
            ValueType = valueType;
            Value = defaultValue;
            DefaultValue = defaultValue;
        }

        private void SetValue(string value, bool addToValueHistory = true)
        {
            this.value = value;
            if (addToValueHistory)
            {
                ValueHistory.Push(value);
            }
        }

        public void RestoreValue(int index)
        {
            for (int i = 0; i < index && ValueHistory.Count > 0; i++)
            {
                ValueHistory.Pop();
            }
            if (ValueHistory.Count > 0)
            {
                Value = ValueHistory.Pop();
            }
        }

        public void SyncValueWith(InkProperty property)
        {
            if (property.ValueType == this.ValueType)
            {
                property.InkPropertyValueChanged += Property_InkPropertyValueChanged;
                ValueSource = property;
            }
        }

        public void DisableValueSynchronization()
        {
            if (ValueSource is not null)
            {
                ValueSource.InkPropertyValueChanged -= Property_InkPropertyValueChanged;
                ValueSource = null;
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

    public abstract class InkObject : INotifyPropertyChanged
    {
        protected string name = "";

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public virtual double X
        {
            get { return ShownElement.Margin.Left; }
            set
            {
                ShownElement.Margin = new(value, ShownElement.Margin.Top, ShownElement.Margin.Right, ShownElement.Margin.Bottom);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }
        public virtual double Y
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
        public abstract string Type { get; }
        public abstract Dictionary<string, InkProperty> Properties { get; }

        protected abstract FrameworkElement ShownElement { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

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
    }

    public class InkPage : INotifyPropertyChanged
    {
        private string name = "";

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
        public ObservableCollection<InkObject> Objects { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }

    public class InkTextBox : InkObject
    {
        private TextBox textBox = new();
        private TextBlock textBlock = new();

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

                { "Foreground", new InkProperty("Foreground", InkPropertyValueType.List, "Black")
                { ValueList = new string[] { "Black", "White", "Grey", "Red", "Orange", "Yellow", "Green", "Blue", "Purple", "Customize" } } },

                { "Background", new InkProperty("Background", InkPropertyValueType.List, "None")
                { ValueList = new string[] { "Black", "White", "Grey", "Red", "Orange", "Yellow", "Green", "Blue", "Purple", "Customize" } } }
            };
            foreach (InkProperty property in Properties.Values)
            {
                property.InkPropertyValueChanged += Property_InkPropertyValueChanged;
                property.Value = property.DefaultValue;
            }
            X = 514;
            Y = 114;
            BindTextBoxWithTextBlock();
        }

        public override string Type => "TYPE: TEXT BOX";

        public override Dictionary<string, InkProperty> Properties { get; }

        protected override FrameworkElement ShownElement => textBlock;

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

        private void Property_InkPropertyValueChanged(object sender, InkPropertyValueChangedEventArgs e)
        {
            switch (e.Name)
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
                default:
                    break;
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
            Binding binding = new Binding
            {
                Source = textBlock,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("Text")
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
        }
    }
}