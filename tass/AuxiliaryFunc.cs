using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using System.Windows.Media;

namespace tass
{
    public static class AuxiliaryFunc
    {
        public static Control setParameters(Control cntrl, int width, int height, int setLeft, int setTop)
        {
            cntrl.Width = width;
            cntrl.Height = height;
            Canvas.SetLeft(cntrl, setLeft);
            Canvas.SetTop(cntrl, setTop);

            return cntrl;
        }

        public static Label getNewLabel(string content, int min_width, int min_height, int margin)
        {
            Label new_button = new Label();
            new_button.Content = content;
            new_button.MinWidth = min_width;
            new_button.MinHeight = min_height;
            Thickness margin_ = new_button.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            new_button.Margin = margin_;
            return new_button;
        }

        public static Label getNewLabel(string content, int width_, int min_width, int min_height, int margin)
        {
            Label new_button = new Label();
            new_button.Content = content;
            new_button.MinWidth = min_width;
            new_button.Width = width_;
            new_button.MinHeight = min_height;
            Thickness margin_ = new_button.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            new_button.Margin = margin_;
            return new_button;
        }

        public static Control setNewControl(Control cntrl, int min_width, int min_height, int margin)
        {
            cntrl.MinWidth = min_width;
            cntrl.MinHeight = min_height;
            Thickness margin_ = cntrl.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            cntrl.Margin = margin_;
            return cntrl;
        }

        public static Control setNewControl(string name, Control cntrl, int min_width, int min_height, int margin)
        {
            cntrl.Name = name;
            cntrl.MinWidth = min_width;
            cntrl.MinHeight = min_height;
            Thickness margin_ = cntrl.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            cntrl.Margin = margin_;
            return cntrl;
        }

        public static Control setNewControl(string name, Control cntrl, int width, int height)
        {
            cntrl.Name = name;
            cntrl.Width = width;
            cntrl.Height = height;
            return cntrl;
        }

        public static ComboBox get_color_combo_box()
        {
            ComboBox combo_box = new ComboBox();
            combo_box.Height = 20;
            combo_box.Width = 200;
            combo_box.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            combo_box.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            Type brushesType = typeof(System.Windows.Media.Brushes);
            // Get all static properties
            var properties = brushesType.GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (var prop in properties)
            {
                string name = prop.Name;
                SolidColorBrush brush = (SolidColorBrush)prop.GetValue(null, null);
                Color color = brush.Color;
                ComboBoxItem item = new ComboBoxItem();
                item.Width = 150;
                item.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                item.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                StackPanel st = new StackPanel();
                st.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                st.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                st.Width = 150;
                st.Height = 15;
                st.Background = brush;
                item.Content = st;
                item.Name = color.ToString().Replace('#', '_');
                combo_box.Items.Add(item);
            }
            combo_box.SelectedIndex = 7;
            return combo_box;
        }

        public static ComboBox get_line_width_combo_box()
        {
            ComboBox combo_box = new ComboBox();
            combo_box.Width = 200;
            for (int i = 0; i < 10; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Width = 200;
                StackPanel st_1 = new StackPanel();
                st_1.Orientation = Orientation.Horizontal;
                Label lb = new Label();
                lb.FontSize = 11;
                setMargin(lb, 0, 0, 5, 0);
                lb.Content = (i + 1).ToString() + " px";
                lb.Width = 30;
                StackPanel st = new StackPanel();
                Thickness margin_ = st.Margin;
                margin_.Left = 0;
                margin_.Top = 0;
                margin_.Bottom = 5;
                margin_.Right = 0;
                st.Margin = margin_;
                st.Width = 120;
                st.Height = i + 1;
                st.Background = Brushes.Black;
                st_1.Children.Add(lb);
                st_1.Children.Add(st);
                item.Content = st_1;
                item.Name = "w" + i.ToString();
                combo_box.Items.Add(item);
            }
            combo_box.SelectedIndex = 0;
            return combo_box;
        }

        public static Button getNewButton(string name, string content, int min_width, int min_height, int left, int top, int right, int bottom)
        {
            Button new_button = new Button();
            new_button.Name = name;
            new_button.Content = content;
            new_button.MinWidth = 150;
            new_button.MinHeight = 25;
            Thickness margin = new_button.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            new_button.Margin = margin;
            return new_button;
        }

        public static Button getNewButton(string name, string content, int min_width, int min_height, int margin)
        {
            Button new_button = new Button();
            new_button.Name = name;
            new_button.Content = content;
            new_button.MinWidth = 150;
            new_button.MinHeight = 25;
            Thickness margin_ = new_button.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            new_button.Margin = margin_;
            return new_button;
        }

        public static Button getNewButton(string content, int min_width, int min_height, int margin)
        {
            Button new_button = new Button();
            new_button.Content = content;
            new_button.MinWidth = min_width;
            new_button.MinHeight = min_height;
            Thickness margin_ = new_button.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            new_button.Margin = margin_;
            return new_button;
        }

        public static Button getNewButton(string content, int min_width, int min_height, int margin, int maxLength)
        {
            Button new_button = new Button();
            new_button.Content = content;
            new_button.MinWidth = min_width;
            new_button.MinHeight = min_height;
            new_button.MaxWidth = maxLength;
            Thickness margin_ = new_button.Margin;
            margin_.Bottom = margin;
            margin_.Left = margin;
            margin_.Top = margin;
            margin_.Right = margin;
            new_button.Margin = margin_;
            return new_button;
        }

        public static Button getNewButton(string content, int width, int height)
        {
            Button new_button = new Button();
            new_button.Content = content;
            new_button.Width = width;
            new_button.Height = height;
            return new_button;
        }

        public static void setMargin(Control cntrl, int top, int left, int bottom, int right)
        {
            Thickness margin = cntrl.Margin;
            margin.Top = top;
            margin.Left = left;
            margin.Bottom = bottom;
            margin.Right = right;
            cntrl.Margin = margin;
        }
        
        public static Grid getTable(int num_of_rows, int num_of_coloumns)
        {
            Grid grid = new Grid();
            for (int i = 0; i < num_of_rows; i++)
            {
                RowDefinition row_def = new RowDefinition();
                grid.RowDefinitions.Add(row_def);
            }
            for (int i = 0; i < num_of_coloumns; i++)
            {
                ColumnDefinition col_def = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col_def);
            }
            return grid;
        }
    }
}
