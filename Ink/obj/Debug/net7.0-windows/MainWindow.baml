   M S B A M L   `   `   `  ????>  :Ink, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:InkInk  5   	       Ink.MainWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:Ink     5   	   _ [MaterialDesignThemes.Wpf, Version=1.0.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     ? Title$  Ink Workspace??5	   	   $	??800??6   $
??1400??6,    ? WindowState$ 	Maximized=?69    ? Icon$ /PowerInk.png??6Q   .??5
      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??19* ?6   ?? 5      $
??Auto ?6   ?? 5      $??70 ?6    ? ColumnDefinitions 5   
   ?? 5      $	??17* ?6   ?? 5      $
??Auto ?6   ?? 5      $??10 ?6   ?? 5      $??4* ?6   ?? 5      $??10 ?6   .+?5   
   o? $??0??6   $??0??6   $??5??6,   .@?5      m? $
??File??6   .@?5      m? -   5        $	??New??5      6(   m? 5      -   5        $??
New window??5      6/   ?? 5      m? 5      -   5        $
??Exit??5      6)   5      m? 5      $
??Page??6   .@?5       m? $??Background colour??6   .@?5!      m? -   5        $??White??5!      6.   m? 5"      -   5        $??Black??5"      6.   m? 5#      -   5        $
??Grey??5#      6-   m? 5$      -   5        $	??Red??5$      6,   m? 5%      -   5        $??Orange??5%      6/   m? 5&      -	   5        $??Yellow??5&      6/   m? 5'      -
   5        $??Green??5'      6.   m? 5(      -   5        $
??Blue??5(      6-   m? 5)      -   5        $??Purple??5)      6/   ?? 5*      m? 5+      -   5        $??	Customize??5+      62   5,      m? 5-      -   5        $??Background image??5-      65   m? 5.      -   5        $??Reset background??5.      65   ?? 5/      m? 50      -   5        $??Show guides??50       m? IsCheckable$
 True??60    m? 	IsChecked$
 True??6C   6v   m? 51      -   5        $(??"Show border around selected object??51      $
 True??6G   $
 True??6Z   6?   52      m? 53      $??Object??6   .@?54      m? -   5        $??New text box??54      61   m? 55      -   5        $??New image box??55      62   m? 56      -   5        $??New line??56      6-   m? 57      -   5        $??New ellipse??57      60   m? 58      -   5        $??New rectangle??58      62   m? 59      -   5        $??New sketchpad??59      62   ?? 5:      m? 5;      -   5        $#??Remove all text boxes on page??5;      6B   m? 5<      -   5        $$??Remove all image boxes on page??5<      6C   5=      m? 5>      -   5         m?Name$ menuItem_Property??5>      $??Property??62   .@?5?      m? -   5        $??View value history??5?      67   m? 5@      -   5        $??Clear value history??5@      68   ?? 5A      m? 5B      -   5        $$ menuItem_ResetSelectedProperty??5B      $>??8Reset value and disable value sync of selected property ??6C   6?   m? 5C      -   5        $! menuItem_ResetAllProperties??5C      $<??6Reset values and disable values sync of all properties??6@   6?   5D      m? 5E      $
??Code??6   .@?5F      m? $!??Open new code editor window??6   ?? 5G      m? 5H      $??Run submitted code??6   5I      m? 5J      $??Colour??6   .@?5K      m? -   5        $??RGB visualizer??5K      63   5L      5M      ?? 5N   
   -    5         ??Name$ canvas_Page??5N      $??1??6&   $??0??63   	 K? ZIndex$	 -2??6C   .+?5O      ?? -!   5        
 ??Name$
 gridSplitter_Horizontal??5O      ${?False??6<   $??2??6?   $y?Visible=?6?    J? Opacity$ 1Q?6?   ??6N   ??  ?? ElementName$ canvas_Page?? ?? Path$ ActualWidth?
 ?? Top 6?   ?? $ slider_Y??$ Value?6?   ?? 5P      -"   5        $
 gridSplitter_Vertical??5P      ${?False??6:   $??2??6L   $y?Visible=?6?   $ 1Q?6?   ??6V   ?? $ canvas_Page??$ ActualHeight? ?? Left 6?   ?? $ slider_X??$ Value?6?   ?? 5Q      -#   5         ??Name$ border_SelectionBorder??5Q      
????? ?$??1q?6H   5R      ?? 5S   
   -$   5        $ canvas_PropertiesPanel??5S      $??1??61   $??3??6>   $??3??6O   $	 0??6_   .+?5T      ?? -%   5         ??Name$ comboBox_Objects??5T       "  MaterialDesignOutlinedComboBox#??[  61   -  'MaterialDesignThemes.Wpf.ComboBoxAssist   ShowSelectedItem$ False??6i   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Object??6?      
IsFloating$
 True??6?   $ 5??6  ??6?   ?? $ canvas_PropertiesPanel??$ ActualWidth? ? ContextMenu 5U      ?? 5V      .@?5W      m? -&   5        $??Remove selected object??5W   #   6C   m? 5X      -'   5        $$??Clear all objects on this page??5X   #   6K   5Y      5[      ?? 5\      -(   5         ??Name$ textBox_Name??5\      $
 Name??6,   $
 True??6F      
HelperText$9 3Update of name takes effect on next time selected. ??6f   .  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton$
 True??6?   $ 51??6?   ??6?   ?? $ canvas_PropertiesPanel??$ ActualWidth?64  ?? 5]      -)   5         ??Name$ groupBox_Type??5]      $??Object??6.   $	 105??6>   ??6O   ?? $ canvas_PropertiesPanel??$ ActualWidth?.??5^      ?? .+?5_      ?? -*   5        $ 	textBox_X??5_      $ X??61   $
 True??6H   ??6h   ?? $ canvas_PropertiesPanel??$ ActualWidth??? 5`      -+   5        $ 	textBox_Y??5`      $ Y??61   $
 True??6H   ??6h   ?? $ canvas_PropertiesPanel??$ ActualWidth??? 5a      -,   5        $ textBox_Width??5a      $ Width??65   $
 True??6P   ??6p   ?? $ canvas_PropertiesPanel??$ ActualWidth??? 5b      --   5        $ textBox_Height??5b      $ Height??66   $
 True??6R   ??6r   ?? $ canvas_PropertiesPanel??$ ActualWidth?5c      5d      ?? 5e      -.   5         ??Name$ checkBox_Visible??5e      $??Visible??61   $	 310??6C   ?? 5f      -/   5        $ checkBox_SyncProperty??5f      $	 340??66   $y?Hidden=?6?   ??6G   ?? $ canvas_PropertiesPanel??$ ActualWidth?6?   ?? 5g      -0   5        $% comboBox_SyncPropertyWithObject??5g      $	 325??6@   $ 22??6Q   $. (Sync the value of selected property with??6b   $
 True??6?   $&  Unckeck the checkbox to change. ??6?   $
{?True??6?   $y?Hidden=?6  6?  ?? 5h      -1   5        $ groupBox_Properties??5h      $??
PROPERTIES??64   $	 385??6H   $	??170??6?   $y?Hidden=?6?   ??6Y   ?? $ canvas_PropertiesPanel??$ ActualWidth?.??5i      ?? -2   5         ??Name$ list_Properties??5i      ??63   ?? $ groupBox_Properties??$ ActualHeight?6{   5j      ?? 5k      -3   5        $ textBox_PropertyValue??5k       ! MaterialDesignOutlinedTextBox#??[ 65   $ Property Value??6l   $
 True??6?   $
 True??6?   $	 565??6?   $y?Hidden=?60   ?? AcceptsReturn$
 True??6D  $	??140??6Y  ??6?   ?? $ canvas_PropertiesPanel??$ ActualWidth?6Y  ?? 5l      -4   5        $ comboBox_PropertyValue??5l      #??[  67   $ Property Value??6o   $
 True??6?   $	 565??6?   $y?Hidden=?6  ??6?   ?? $ canvas_PropertiesPanel??$ ActualWidth?6  ?? 5m      -5   5        $ checkBox_PropertyValue??5m      $	 565??67   $??Property Value??6H   $y?Hidden=?6a   5n      ?? 5o   
   -6   5         ??Name$ slider_X??5o      $??2??6#   $??0??60   $	 1??6@   $y?Hidden=?6?   ??6Q   ?? $ canvas_Page??$ ActualWidth? ? Maximum 6?   ?? $ canvas_Page??$ ActualWidth?6?   ?? 5p   
   -7   5        $ slider_Y??5p      $??1??6#   $??1??60   $	 1??6@    ?? IsDirectionReversed$
 True??6Q     ?? Orientation$  Vertical=?6l   $y?Hidden=?6?   ??6?   ?? $ canvas_Page??$ ActualHeight? 6?   ?? $ canvas_Page??$ ActualHeight?6?   c? 5q   
   $??3??6   $??0??6   .@?5r      ?? -8   5        $ comboBox_Pages??5r      #??[  6/   $ Current Page??6g   $
 True??6?   $ False??6?   6?   ?? 5s      ?? 5t      -9   5          MaterialDesignFlatButton#??[ 5t      $	??Add??6G   6U   ?? 5u      -:   5        #??[ 5u      $??Remove??6G   6X   ?? 5v      -;   5        #??[ 5v      $??
Remove all??6G   6\   ?? 5w      ?? 5x      -<   5        #??[ 5x      $??Rename??6G   6X   ?? 5y      -=   5        $ textBox_PageRename??5y      $	??250??62   $9 3Update of name takes effect on next time selected. ??6A   $ 	Page Name??6?   $
 True??6?   $y?	Collapsed=?6?   6  5z      ? 5{   
   ->   5        ! ?Name$ ! popup_PropertyValueHistory??5{      " ? AllowsTransparency$" False??64   $
??Fade=?6O   # ? 	StaysOpen$# False??6?   $ ? PlacementTarget$ 6e   ?? $ menuItem_Property??.??5|      ?? 
???????.+?5}      ?? ?? 5~      -?   5        $# comboBox_PropertyValueHistory??5~      #??[  6B   $ Value History??6z   $
 True??6?   ?? 5      ?? 5?      -@   5        % ??Name$!% button_RestorePropertyValue??5?      #??[ 6>   $??Restore??6p   6?   5?      5?      5?      5?      