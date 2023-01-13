using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            pages = new ObservableCollection<InkPage>() { new InkPage($"Page{pageCounter++}") };
            comboBox_Pages.ItemsSource = pages;
            comboBox_Pages.SelectedIndex = 0;
            currentPage = pages[0];
            checkBox_Visible.IsEnabled = false;
        }

        private void ComboBox_Pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentPage = pages[comboBox_Pages.SelectedIndex];
            canvas_Page.Children.Clear();
            /* 将选中Page的所有Object添加到显示区，并选中第一个Object（如果有） */
            foreach (InkObject element in currentPage.Objects)
            {
                element.AddToPage(canvas_Page);
            }
            comboBox_Objects.ItemsSource = currentPage.Objects;
            if (comboBox_Objects.Items.Count > 0)
            {
                comboBox_Objects.SelectedIndex = 0;
            }
        }

        private void ComboBox_Objects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Objects.SelectedItem is InkObject)
            {
                currentObject = comboBox_Objects.SelectedItem as InkObject;
                if (currentObject is not null)
                {
                    groupBox_Type.Header = currentObject.Type;
                    Binding binding = new()
                    {
                        Source = currentObject,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath("Name")
                    };
                    textBox_Name.SetBinding(TextBox.TextProperty, binding);
                    binding = new Binding
                    {
                        Source = currentObject,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath("X")
                    };
                    textBox_X.SetBinding(TextBox.TextProperty, binding);
                    slider_X.SetBinding(Slider.ValueProperty, binding);
                    binding = new Binding
                    {
                        Source = currentObject,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath("Y")
                    };
                    textBox_Y.SetBinding(TextBox.TextProperty, binding);
                    slider_Y.SetBinding(Slider.ValueProperty, binding);
                    binding = new Binding
                    {
                        Source = currentObject,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath("Width")
                    };
                    textBox_Width.SetBinding(TextBox.TextProperty, binding);
                    binding = new Binding
                    {
                        Source = currentObject,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath("Height")
                    };
                    textBox_Height.SetBinding(TextBox.TextProperty, binding);
                    binding = new Binding
                    {
                        Source = currentObject,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath("Visible")
                    };
                    checkBox_Visible.SetBinding(CheckBox.IsCheckedProperty, binding);
                    checkBox_Visible.IsEnabled = true;
                    slider_X.Visibility = Visibility.Visible;
                    slider_Y.Visibility = Visibility.Visible;
                    checkBox_SyncProperty.Visibility = Visibility.Visible;
                    comboBox_SyncPropertyWithObject.Visibility = Visibility.Visible;
                    groupBox_Properties.Visibility = Visibility.Visible;
                    list_Properties.ItemsSource = currentObject.Properties.Values;
                    if (list_Properties.Items.Count > 0)
                    {
                        list_Properties.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                currentObject = null;
                checkBox_Visible.IsEnabled = false;
                slider_X.Visibility = Visibility.Hidden;
                slider_Y.Visibility = Visibility.Hidden;
                checkBox_SyncProperty.Visibility = Visibility.Hidden;
                comboBox_SyncPropertyWithObject.Visibility = Visibility.Hidden;
                groupBox_Properties.Visibility = Visibility.Hidden;
            }
        }

        private void MenuItem_NewTextBox_Click(object sender, RoutedEventArgs e)
        {
            InkTextBox inkTextBox = new($"TextBox{textBoxCounter++}");
            currentPage.Objects.Add(inkTextBox);
            inkTextBox.AddToPage(canvas_Page);
            comboBox_Objects.SelectedIndex = comboBox_Objects.Items.Count - 1;
        }

        private void ComboBox_Objects_DropDownOpened(object sender, EventArgs e)
        {
            comboBox_Objects.Items.Refresh();
        }

        private void TextBox_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            comboBox_Objects.Items.Refresh();   // 更新Object组合框里的名称
        }

        private void List_Properties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list_Properties.SelectedItem is InkProperty selectedProperty)
            {
                checkBox_PropertyValue.Visibility = Visibility.Hidden;
                textBox_PropertyValue.Visibility = Visibility.Hidden;
                comboBox_PropertyValue.Visibility = Visibility.Hidden;
                Binding binding = new()
                {
                    Source = selectedProperty,
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                switch (selectedProperty.ValueType)
                {
                    case InkPropertyValueType.Boolean:
                        checkBox_PropertyValue.Visibility = Visibility.Visible;
                        checkBox_PropertyValue.Content = selectedProperty.Name;
                        checkBox_PropertyValue.SetBinding(CheckBox.IsCheckedProperty, binding);
                        break;
                    case InkPropertyValueType.List:
                        comboBox_PropertyValue.Visibility = Visibility.Visible;
                        comboBox_PropertyValue.ItemsSource = selectedProperty.ValueList;
                        binding = new Binding
                        {
                            Source = selectedProperty,
                            Path = new PropertyPath("Value"),
                            Mode = BindingMode.OneWay
                        };
                        comboBox_PropertyValue.SetBinding(ComboBox.SelectedItemProperty, binding);
                        break;
                    case InkPropertyValueType.Input:
                        textBox_PropertyValue.Visibility = Visibility.Visible;
                        textBox_PropertyValue.SetBinding(TextBox.TextProperty, binding);
                        break;
                    default:
                        break;
                }
                binding = new Binding
                {
                    Source = selectedProperty,
                    Path = new PropertyPath("ValueSyncEnabled"),
                    Mode = BindingMode.OneWay
                };
                checkBox_SyncProperty.SetBinding(CheckBox.IsCheckedProperty, binding);
                binding = new Binding
                {
                    Source = selectedProperty,
                    Path = new PropertyPath("ValueSourceObject"),
                    Mode = BindingMode.OneWay
                };
                comboBox_SyncPropertyWithObject.SetBinding(ComboBox.SelectedItemProperty, binding);
            }
        }

        private void ComboBox_PropertyValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Binding会出问题所以采用手动更新的方式
            if (comboBox_PropertyValue.SelectedIndex >= 0 && list_Properties.SelectedIndex >= 0 && currentObject is not null
                && list_Properties.SelectedItem is InkProperty selectedProperty && comboBox_PropertyValue.SelectedItem is string selectedPropertyValue)
            {
                currentObject.Properties[selectedProperty.Name].Value = selectedPropertyValue;
            }
        }

        private void MenuItem_File_New_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void MenuItem_File_NewWindow_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
        }

        private void MenuItem_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_SyncProperty_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox_SyncProperty.IsChecked == true)
            {
                if (comboBox_SyncPropertyWithObject.SelectedItem is InkObject selectedSyncObject)
                {
                    if (list_Properties.SelectedItem is InkProperty selectedProperty)
                    {
                        selectedProperty.DisableValueSynchronization();
                        selectedProperty.SyncValueWith(selectedSyncObject.Properties[selectedProperty.Name], selectedSyncObject);
                        SetValueEditState(false);
                    }
                }
                else
                {
                    MessageBox.Show("Please select which object to sync with first. ", "Select Object First", MessageBoxButton.OK, MessageBoxImage.Information);
                    checkBox_SyncProperty.IsChecked = false;
                }
            }
            else if (checkBox_SyncProperty.IsChecked == false)
            {
                if (comboBox_SyncPropertyWithObject.SelectedItem is InkObject)
                {
                    if (list_Properties.SelectedItem is InkProperty selectedProperty)
                    {
                        selectedProperty.DisableValueSynchronization();
                        SetValueEditState(true);
                    }
                }
            }
        }

        private void ComboBox_SyncPropertyWithObject_DropDownOpened(object sender, EventArgs e)
        {
            if (comboBox_Objects.SelectedIndex >= 0 && currentObject is not null)
            {
                List<InkObject> list_SyncObjects = new(currentPage.Objects);
                list_SyncObjects.Remove(currentObject);
                foreach (InkObject inkObject in list_SyncObjects)
                {
                    if (inkObject.Type != currentObject.Type)
                    {
                        list_SyncObjects.Remove(inkObject);
                    }
                }
                comboBox_SyncPropertyWithObject.ItemsSource = list_SyncObjects;
            }
        }

        private void CheckBox_SyncProperty_Checked(object sender, RoutedEventArgs e)
        {
            SetValueEditState(false);
        }

        private void CheckBox_SyncProperty_Unchecked(object sender, RoutedEventArgs e)
        {
            SetValueEditState(true);
        }

        private void SetValueEditState(bool enabled)
        {
            if (enabled)
            {
                textBox_PropertyValue.IsEnabled = true;
                comboBox_PropertyValue.IsEnabled = true;
                checkBox_PropertyValue.IsEnabled = true;
            }
            else
            {
                textBox_PropertyValue.IsEnabled = false;
                comboBox_PropertyValue.IsEnabled = false;
                checkBox_PropertyValue.IsEnabled = false;
            }
        }

        private void MenuItem_ResetSelectedProperty_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_Objects.SelectedIndex >= 0 && list_Properties.SelectedItem is InkProperty selectedProperty)
            {
                selectedProperty.Value = selectedProperty.DefaultValue;
                selectedProperty.DisableValueSynchronization();
                checkBox_SyncProperty.IsChecked = false;
                SetValueEditState(true);
            }
        }

        private void MenuItem_ResetAllProperties_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_Objects.SelectedItem is InkObject selectedObject)
            {
                foreach (InkProperty property in selectedObject.Properties.Values)
                {
                    property.Value = property.DefaultValue;
                    property.DisableValueSynchronization();
                    checkBox_SyncProperty.IsChecked = false;
                    SetValueEditState(true);
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private int pageCounter = 1, textBoxCounter = 1;
        private ObservableCollection<InkPage> pages;
        private InkPage currentPage;
        private InkObject? currentObject;
    }
}
