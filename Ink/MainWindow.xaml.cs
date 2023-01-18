using ColorPickerDialog;
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
            if (comboBox_Pages.SelectedIndex >= 0)
            {
                currentPage = pages[comboBox_Pages.SelectedIndex];
                canvas_Page.Children.Clear();
                canvas_Page.Children.Add(gridSplitter_Horizontal);
                canvas_Page.Children.Add(gridSplitter_Vertical);
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
                canvas_Page.Background = new SolidColorBrush(currentPage.Background);
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
                textBox_Name.Text = string.Empty;
                groupBox_Type.Header = "TYPE: ";
                textBox_X.Text = string.Empty;
                textBox_Y.Text = string.Empty;
                textBox_Width.Text = string.Empty;
                textBox_Height.Text = string.Empty;
                checkBox_Visible.IsEnabled = false;
                slider_X.Visibility = Visibility.Hidden;
                slider_Y.Visibility = Visibility.Hidden;
                checkBox_SyncProperty.Visibility = Visibility.Hidden;
                comboBox_SyncPropertyWithObject.Visibility = Visibility.Hidden;
                groupBox_Properties.Visibility = Visibility.Hidden;
                textBox_PropertyValue.Visibility = Visibility.Hidden;
                comboBox_PropertyValue.Visibility = Visibility.Hidden;
                checkBox_PropertyValue.Visibility = Visibility.Hidden;
            }
        }

        private void MenuItem_NewTextBox_Click(object sender, RoutedEventArgs e)
        {
            InkTextBox inkTextBox = new($"TextBox{textBoxCounter++}");
            currentPage.Objects.Add(inkTextBox);
            inkTextBox.AddToPage(canvas_Page);
            comboBox_Objects.SelectedIndex = comboBox_Objects.Items.Count - 1;
            inkTextBox.Click += InkObject_Click;
        }

        private void InkObject_Click(object sender, MouseButtonEventArgs e)
        {
            // 如果：
            // 1. 未选中任何Object；或
            // 2. 选中的Object不是被点击的
            if (sender is InkObject inkObject && (currentObject is null || !currentObject.Equals(inkObject)))
            {
                if (currentPage is not null && comboBox_Objects.Items.Contains(inkObject))
                {
                    comboBox_Objects.SelectedItem = inkObject;
                }
            }
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
                        textBox_PropertyValue.Focus();
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
                SetComboBox_SyncPropertyWithObject_ItemsSource();
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
            SetComboBox_SyncPropertyWithObject_ItemsSource();
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

        private void ComboBox_SyncPropertyWithObject_DropDownClosed(object sender, EventArgs e)
        {
            //if (comboBox_SyncPropertyWithObject.SelectedItem is InkObject selectedSyncObject
            //    && list_Properties.SelectedItem is InkProperty selectedProperty)
            //{
            //    if (selectedProperty.ValueSourceObject is not null 
            //        && (!selectedProperty.ValueSourceObject.Equals(selectedSyncObject)))
            //    {
            //        checkBox_SyncProperty.IsChecked = false;
            //    }
            //}
        }

        private void ComboBox_SyncPropertyWithObject_DropDownOpened(object sender, EventArgs e)
        {
            SetComboBox_SyncPropertyWithObject_ItemsSource();
        }

        private void SetComboBox_SyncPropertyWithObject_ItemsSource()
        {
            if (comboBox_Objects.SelectedIndex >= 0 && currentObject is not null)
            {
                List<InkObject> list_SyncObjects = new();
                foreach (InkPage page in pages)
                {
                    foreach (InkObject inkObject in page.Objects)
                    {
                        if (inkObject.Type == currentObject.Type && !(inkObject == currentObject))
                        {
                            list_SyncObjects.Add(inkObject);
                        }
                    }
                }
                comboBox_SyncPropertyWithObject.ItemsSource = list_SyncObjects;
            }
        }

        private void CheckBox_SyncProperty_Checked(object sender, RoutedEventArgs e)
        {
            SetValueEditState(false);
            comboBox_SyncPropertyWithObject.IsEnabled = false;
        }

        private void CheckBox_SyncProperty_Unchecked(object sender, RoutedEventArgs e)
        {
            SetValueEditState(true);
            comboBox_SyncPropertyWithObject.IsEnabled = true;
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

        private void Button_AddPage_Click(object sender, RoutedEventArgs e)
        {
            pages.Add(new InkPage($"Page{pageCounter++}"));
            comboBox_Pages.SelectedIndex = comboBox_Pages.Items.Count - 1;
        }

        private void ComboBox_SyncPropertyWithObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SetComboBox_SyncPropertyWithObject_ItemsSource();
            //if (checkBox_SyncProperty.IsChecked == true)
            //{
            //    if (comboBox_SyncPropertyWithObject.SelectedItem is InkObject selectedSyncObject)
            //    {
            //        if (list_Properties.SelectedItem is InkProperty selectedProperty)
            //        {
            //            selectedProperty.DisableValueSynchronization();
            //            selectedProperty.SyncValueWith(selectedSyncObject.Properties[selectedProperty.Name], selectedSyncObject);
            //            SetValueEditState(false);
            //        }
            //    }
            //}
        }

        private void Button_RemovePage_Click(object sender, RoutedEventArgs e)
        {
            if (pages.Count > 1)
            {
                int currentPageIndex = pages.IndexOf(currentPage);
                pages.Remove(currentPage);
                if (currentPageIndex > 0)
                {
                    comboBox_Pages.SelectedIndex = currentPageIndex - 1;
                }
                else if (currentPageIndex == 0)
                {
                    comboBox_Pages.SelectedIndex = 0;
                }
            }
        }

        private void Button_RemoveAllPages_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void MenuItems_BackgroundColour_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem_Colour)
            {
                Color background = menuItem_Colour.Header switch
                {
                    "White" => Colors.White,
                    "Black" => Colors.Black,
                    "Grey" => Colors.Gray,
                    "Red" => Colors.Red,
                    "Orange" => Colors.Orange,
                    "Yellow" => Colors.Yellow,
                    "Green" => Colors.Green,
                    "Blue" => Colors.Blue,
                    "Purple" => Colors.Purple,
                    _ => Colors.Transparent,
                };
                canvas_Page.Background = new SolidColorBrush(background);
                if (currentPage is not null)
                {
                    currentPage.Background = background;
                }
            }
        }

        private void MenuItem_ResetBackgroundColour_Click(object sender, RoutedEventArgs e)
        {
            canvas_Page.Background = new SolidColorBrush(Colors.White);
            if (currentPage is not null)
            {
                currentPage.Background = Colors.White;
            }
        }

        private void MenuItem_VisualizeRGB_Click(object sender, RoutedEventArgs e)
        {
            new ColorDialog(false).Show();
        }

        private void MenuItem_Guides_Checked(object sender, RoutedEventArgs e)
        {
            if (gridSplitter_Horizontal is not null && gridSplitter_Vertical is not null)
            {
                gridSplitter_Horizontal.Visibility = Visibility.Visible;
                gridSplitter_Vertical.Visibility = Visibility.Visible;
            }
        }

        private void MenuItem_Guides_Unchecked(object sender, RoutedEventArgs e)
        {
            if (gridSplitter_Horizontal is not null && gridSplitter_Vertical is not null)
            {
                gridSplitter_Horizontal.Visibility = Visibility.Hidden;
                gridSplitter_Vertical.Visibility = Visibility.Hidden;
            }
        }

        private void MenuItem_CustomizeBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new(true);
            if (colorDialog.ShowDialog() == true)
            {
                Color background = Color.FromRgb((byte)colorDialog.R, (byte)colorDialog.G, (byte)colorDialog.B);
                canvas_Page.Background = new SolidColorBrush(background);
                currentPage.Background = background;
            }
        }

        private void MenuItem_RemoveSelectedObject_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_Objects.SelectedItem is InkObject selectedObject)
            {
                int currentObjectIndex = comboBox_Objects.SelectedIndex;
                RemoveObject(selectedObject);
                if (currentObjectIndex > 0)
                {
                    comboBox_Objects.SelectedIndex = currentObjectIndex - 1;
                }
                else if (currentObjectIndex == 0)
                {
                    comboBox_Objects.SelectedIndex = 0;
                }
            }
        }

        private void MenuItem_ClearAllObjectsOnPage_Click(object sender, RoutedEventArgs e)
        {
            while (currentPage.Objects.Count > 0)
            {
                RemoveObject(currentPage.Objects[^1]);
            }
        }

        private void RemoveObject(InkObject objectToRemove)
        {
            foreach (InkPage page in pages)
            {
                foreach (InkObject inkObject in page.Objects)
                {
                    IEnumerable<InkProperty> synchronizedProperties = from property
                                                                      in inkObject.Properties.Values
                                                                      where property.ValueSourceObject == objectToRemove
                                                                      select property;
                    foreach (InkProperty property in synchronizedProperties)
                    {
                        property.DisableValueSynchronization();
                    }
                }
            }
            objectToRemove.RemoveFromPage(canvas_Page);
            currentPage.Objects.Remove(objectToRemove);
        }
    }

    public partial class MainWindow : Window
    {
        private int pageCounter = 1, textBoxCounter = 1;
        private readonly ObservableCollection<InkPage> pages;
        private InkPage currentPage;
        private InkObject? currentObject;
    }
}
