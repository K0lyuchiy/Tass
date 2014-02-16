using System;
using System.Collections.Generic; 
using System.Windows;
using System.Windows.Controls;  
using System.Collections.ObjectModel; 

namespace tass
{
    public partial class VariablesWindow : Window
    {
        PasswordBox password;

        public ObservableCollection<string> Variables { get; set; }

        public VariablesWindow(IEnumerable<string> vars)
        {
            Variables = new ObservableCollection<string>(vars);

            this.Height = 120;
            this.MaxHeight = 120;
            this.MinHeight = 120;
            this.Width = 330;
            this.MaxWidth = 330;
            this.MinWidth = 330;
            this.Title = "Идентификация администратора";

            Grid grid_1 = new Grid();
            Thickness margin = grid_1.Margin;
            margin.Top = 10;
            margin.Left = 10;
            margin.Bottom = 0;
            margin.Right = 0;
            grid_1.Margin = margin;

            // grid_1.ShowGridLines = true;
            RowDefinition row_def_grid_1 = new RowDefinition();
            RowDefinition row_def_grid_2 = new RowDefinition();
            row_def_grid_1.Height = new GridLength(30);
            row_def_grid_2.Height = new GridLength(30);
            ColumnDefinition col_def_grid_1_1 = new ColumnDefinition();
            col_def_grid_1_1.Width = new GridLength(100);
            ColumnDefinition col_def_grid_1_2 = new ColumnDefinition(); 

            grid_1.RowDefinitions.Add(row_def_grid_1);
            grid_1.RowDefinitions.Add(row_def_grid_2);

            grid_1.ColumnDefinitions.Add(col_def_grid_1_1);
            grid_1.ColumnDefinitions.Add(col_def_grid_1_2);

            Label password_label = new Label();
            password_label.Content = "Пароль";
            Grid.SetRow(password_label, 0);
            Grid.SetColumn(password_label, 0);
            password_label.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            password_label.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            password = new PasswordBox();

            password.Width = 150;
            password.Height = 25;

            password.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            password.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetRow(password, 0);
            Grid.SetColumn(password, 1);

            Button bt = new Button();
            bt.Height = 25;
            bt.Width = 100;
            bt.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            bt.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            bt.Click += bt_Click;
            bt.Content = "Ok";

            margin = bt.Margin;
            margin.Top = 5;
            margin.Left = 5;
            margin.Bottom = 0;
            margin.Right = 27;
            bt.Margin = margin;

            Grid.SetRow(bt, 1);
            Grid.SetColumn(bt, 1);

            grid_1.Children.Add(password_label);
            grid_1.Children.Add(password);
            grid_1.Children.Add(bt);

            this.AddChild(grid_1);
        }

        void bt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Variables.Add((string)password.Password.ToString());
            this.DialogResult = true;
            this.Close();
        }
    }
}
