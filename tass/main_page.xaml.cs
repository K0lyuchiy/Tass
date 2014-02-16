using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tass
{
    public delegate void _MarkerEventHandler();

    public class MarkerEvent
    {
        public event _MarkerEventHandler someEvent;

        public void OnSomeEvent()
        {
            if (someEvent != null)
                someEvent();
        }
    }
    /// <summary>
    /// Логика взаимодействия для main_page.xaml
    /// </summary>
    public partial class main_page : Page
    {
        public MarkerEvent evt_tarirovka_click;
        public MarkerEvent evt_Training_Click;
        public MarkerEvent evt_draw_calc_choise;
        public MarkerEvent evt_Measuring_Click;

        Button settings_button;
        Button calibrate_button;
        Button meausure_button;
        Button calculate_button;
        TextBlock first_text;

        public main_page()
        {
            evt_tarirovka_click = new MarkerEvent();
            evt_Training_Click = new MarkerEvent();
            evt_draw_calc_choise = new MarkerEvent();
            evt_Measuring_Click = new MarkerEvent();
            InitializeComponent();
        }
        
        public Grid drawMainPage() 
        {
            StackPanel stack_panel = new StackPanel();
            stack_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            Grid grid_main = new Grid();
            //  grid_main.ShowGridLines = true;            
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            ColumnDefinition col_def_1 = new ColumnDefinition();
            ColumnDefinition col_def_2 = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.ColumnDefinitions.Add(col_def_1);
            grid_main.ColumnDefinitions.Add(col_def_2);

            settings_button = AuxiliaryFunc.getNewButton("settings_button", "Тарировка", 150, 25, 15);
            settings_button.Click += new RoutedEventHandler(tarirovka_click);

            calibrate_button = AuxiliaryFunc.getNewButton("Тренировка", 150, 25, 15);
            calibrate_button.Click += new RoutedEventHandler(Training_Click);

            meausure_button = AuxiliaryFunc.getNewButton("Измерения", 150, 25, 15);
            meausure_button.Click += new RoutedEventHandler(Measuring_Click);

            calculate_button = AuxiliaryFunc.getNewButton("Расчеты", 150, 25, 15);
            calculate_button.Click += new RoutedEventHandler(draw_calc_choise);

            stack_panel.Children.Add(settings_button);
            stack_panel.Children.Add(calibrate_button);
            stack_panel.Children.Add(meausure_button);
            stack_panel.Children.Add(calculate_button);

            Grid.SetRow(stack_panel, 0);
            Grid.SetColumn(stack_panel, 0);
            Grid.SetRowSpan(stack_panel, 2);

            first_text = new TextBlock();
            first_text.Width = 400;
            first_text.Height = 400;
            first_text.Inlines.Add(new Run("  Измерительная система"));
            first_text.Inlines.Add(new Bold(new Run(" \"ТАСС\" ")));
            first_text.Inlines.Add(new Run("предназначена для анализа\n и тренировки силовых способностей  в режимах тяговых усилий\n или силы хвата кисти."));

            Grid.SetRow(first_text, 1);
            Grid.SetColumn(first_text, 1);

            StackPanel stack_panel_2 = new StackPanel();
            stack_panel_2.Height = 400;
            stack_panel_2.Width = 400;
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stack_panel_2.Background = Brushes.Beige;

            Grid.SetRow(stack_panel_2, 0);
            Grid.SetColumn(stack_panel_2, 1);

            grid_main.Children.Add(stack_panel);
            grid_main.Children.Add(stack_panel_2);
            grid_main.Children.Add(first_text);
            //set_titles("ТАСС", "Главное меню");
            return grid_main;
        }

        private void tarirovka_click(object sender, RoutedEventArgs e)
        {
            evt_tarirovka_click.OnSomeEvent(); 
        }

        private void Training_Click(object sender, RoutedEventArgs e)
        {
            evt_Training_Click.OnSomeEvent(); 
        }

        private void draw_calc_choise(object sender, RoutedEventArgs e)
        {
            evt_draw_calc_choise.OnSomeEvent(); 
        }

        private void Measuring_Click(object sender, RoutedEventArgs e)
        {
            evt_Measuring_Click.OnSomeEvent(); 
        } 
    }
}
