using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Threading;
using System.Windows.Threading;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace tass
{
    /// <summary>
    /// Логика взаимодействия для tarirovka_tab.xaml
    /// </summary>
    public partial class tarirovka_tab : Page
    {
        private TextBox tarirovka_weight_text;
        private Button next_button;
        private Label balance_done_label;
        private Button calibrate_step_1_button;

        public MarkerEvent1 evt_tarirovka_done;
        public MarkerEvent1 evt_tarirovka_step_2;
        public MarkerEvent1 evt_tarirovka_cancel;

        private PlotterControl plotter_cntrl;
        private DeviceTune device;         

        public tarirovka_tab(Dispatcher disp)
        { 
            evt_tarirovka_step_2 = new MarkerEvent1();
            evt_tarirovka_done = new MarkerEvent1();
            evt_tarirovka_cancel = new MarkerEvent1();

            device = new DeviceTune();

            plotter_cntrl = new PlotterControl(disp);
            plotter_cntrl.evt_1.someEvent += new _MarkerEventHandler1(tarirovka_1_done);
            plotter_cntrl.evt_2.someEvent += new _MarkerEventHandler1(tarirovka_2_done);
        }

        private void UpdateLabel(string new_string)
        {
            if (balance_done_label.Dispatcher.CheckAccess() == false)
            {
                updateLabelCallback uCallBack = new updateLabelCallback(UpdateLabel);
                this.Dispatcher.Invoke(uCallBack, new_string);
            }
            else
            {
                balance_done_label.Content = new_string;
            }
        }

        private void UpdateButton(bool new_value)
        {
            if (balance_done_label.Dispatcher.CheckAccess() == false)
            {
                updateButtonCallback uCallBack = new updateButtonCallback(UpdateButton);
                this.Dispatcher.Invoke(uCallBack, new_value);
            }
            else
            {
                calibrate_step_1_button.IsEnabled = false;
                next_button.IsEnabled = new_value;
            }
        }

        public Grid draw_tarirovka_step_1()
        {
            StackPanel head_panel = new StackPanel();
            head_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            head_panel.Orientation = Orientation.Horizontal;

            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            RowDefinition row_def_5 = new RowDefinition();
            row_def_1.Height = new GridLength(100);
            row_def_2.Height = new GridLength(100);
            row_def_4.Height = new GridLength(60);
            row_def_5.Height = new GridLength(60);
            ColumnDefinition col_def_1 = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.RowDefinitions.Add(row_def_3);
            grid_main.RowDefinitions.Add(row_def_4);
            grid_main.RowDefinitions.Add(row_def_5);
            grid_main.ColumnDefinitions.Add(col_def_1);


            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Тарировка системы\n\n ")));
            header.Width = 300;
            Thickness margin = header.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            header.Margin = margin;

            Expander tarirovka_info = new Expander();
            tarirovka_info.Header = "СПРАВКА";
            tarirovka_info.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            StackPanel expander_panel = new StackPanel();
            expander_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            TextBlock header_info = new TextBlock();
            header_info.Inlines.Add((new Run(" Балансировка и тарировка производится при первичном включениии или каждые 20 минут непрерывного режима работы системы.")));

            expander_panel.Children.Add(header_info);

            tarirovka_info.Content = expander_panel;
            margin = tarirovka_info.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            tarirovka_info.Margin = margin;

            head_panel.Children.Add(header);
            head_panel.Children.Add(tarirovka_info);
            Grid.SetRow(head_panel, 0);
            Grid.SetColumn(head_panel, 0);

            StackPanel stack_panel = new StackPanel();
            stack_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            TextBlock first_step_text = new TextBlock();
            first_step_text.Inlines.Add(new Bold(new Run(" Шаг 1.\n ")));
            first_step_text.Inlines.Add(new Run("\nСбалансируйте измерительную систему. Для этого полностью разгрузите силовоспринимающие элементы конструкции,"));
            first_step_text.Inlines.Add(new Run(" обеспечивая установку \nначала отсчетов училий. (\"0\"), \n "));
            margin = first_step_text.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 5;
            margin.Right = 0;
            first_step_text.Margin = margin;

            stack_panel.Children.Add(first_step_text);
            Grid.SetRow(stack_panel, 1);
            Grid.SetColumn(stack_panel, 0);

            plotter_cntrl.plotter = new ChartPlotter();
            margin = plotter_cntrl.plotter.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            plotter_cntrl.plotter.Margin = margin;
            Grid.SetRow(plotter_cntrl.plotter, 2);
            Grid.SetColumn(plotter_cntrl.plotter, 0);
            plotter_cntrl.plotter.Legend.Remove();
            plotter_cntrl.plotter.FitToView();

            StackPanel buttons_panel_1 = new StackPanel();
            buttons_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            buttons_panel_1.Orientation = Orientation.Horizontal;

            balance_done_label = AuxiliaryFunc.getNewLabel("Балансировка системы не выполнена", 200, 25, 5);
            balance_done_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            calibrate_step_1_button = AuxiliaryFunc.getNewButton("Балансировать систему", 200, 25);
            calibrate_step_1_button.Click += new RoutedEventHandler(tarirovka_1);
            AuxiliaryFunc.setMargin((Control)calibrate_step_1_button, 25, 10, 0, 10);

            buttons_panel_1.Children.Add(balance_done_label);
            buttons_panel_1.Children.Add(calibrate_step_1_button);
            Grid.SetRow(buttons_panel_1, 3);
            Grid.SetColumn(buttons_panel_1, 0);

            StackPanel buttons_panel_2 = new StackPanel();
            buttons_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            buttons_panel_2.Orientation = Orientation.Horizontal;

            next_button = AuxiliaryFunc.getNewButton("Далее", 150, 25);
            next_button.Click += new RoutedEventHandler(tarirovka_step_2);
            next_button.IsEnabled = false;
            AuxiliaryFunc.setMargin((Control)next_button, 25, 10, 0, 10);

            Button cancel_button = AuxiliaryFunc.getNewButton("Отмена", 150, 25);
            cancel_button.Click += new RoutedEventHandler(tarirovka_cancel);
            AuxiliaryFunc.setMargin((Control)cancel_button, 25, 10, 0, 10);

            buttons_panel_2.Children.Add(next_button);
            buttons_panel_2.Children.Add(cancel_button);
            Grid.SetRow(buttons_panel_2, 4);
            Grid.SetColumn(buttons_panel_2, 0);

            grid_main.Children.Add(head_panel);
            grid_main.Children.Add(stack_panel);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(buttons_panel_1);
            grid_main.Children.Add(buttons_panel_2);
            Grid.SetRow(grid_main, 0);
            Grid.SetColumn(grid_main, 0);         

            // Отрисовывать уровень в виде точки
            plotter_cntrl.TRAIN_FLAG = false;
            plotter_cntrl.PRECALIBRATE_FLAG = true;

            plotter_cntrl.StartGettingData();
            plotter_cntrl.StartPlotter();
 
            return grid_main;
        }

        public Grid draw_tarirovka_step_2()
        {
            StackPanel head_panel = new StackPanel();
            head_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            head_panel.Orientation = Orientation.Horizontal;

            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            RowDefinition row_def_5 = new RowDefinition();
            RowDefinition row_def_6 = new RowDefinition();
            row_def_1.Height = new GridLength(100);
            row_def_2.Height = new GridLength(100);
            row_def_4.Height = new GridLength(60);
            row_def_5.Height = new GridLength(60);
            row_def_6.Height = new GridLength(60);
            ColumnDefinition col_def_1 = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.RowDefinitions.Add(row_def_3);
            grid_main.RowDefinitions.Add(row_def_4);
            grid_main.RowDefinitions.Add(row_def_5);
            grid_main.RowDefinitions.Add(row_def_6);
            grid_main.ColumnDefinitions.Add(col_def_1);

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Тарировка системы\n\n ")));
            header.Width = 300;
            Thickness margin = header.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            header.Margin = margin;

            Expander tarirovka_info = new Expander();
            tarirovka_info.Header = "СПРАВКА";
            tarirovka_info.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            StackPanel expander_panel = new StackPanel();
            expander_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            TextBlock header_info = new TextBlock();
            header_info.Inlines.Add((new Run(" Балансировка и тарировка производится при первичном включениии или каждые 20 минут непрерывного режима работы системы.")));

            expander_panel.Children.Add(header_info);

            tarirovka_info.Content = expander_panel;
            margin = tarirovka_info.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            tarirovka_info.Margin = margin;

            head_panel.Children.Add(header);
            head_panel.Children.Add(tarirovka_info);
            Grid.SetRow(head_panel, 0);
            Grid.SetColumn(head_panel, 0);

            StackPanel stack_panel = new StackPanel();
            stack_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            TextBlock first_step_text = new TextBlock();
            first_step_text.Inlines.Add(new Bold(new Run(" Шаг 2.\n\n ")));
            first_step_text.Inlines.Add(new Run(" Тарировка производится при воздействии нормированного усилия известной величины, \n "));
            first_step_text.Inlines.Add(new Run("в частности, при удержании груза известной массы (подвешенного к измерителю в\n режиме тяговых усилий).\n\n"));
            first_step_text.Inlines.Add(new Run("Поднять и, удерживая навесу груз (не менее 3 сек.)."));
            margin = first_step_text.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 5;
            margin.Right = 0;
            first_step_text.Margin = margin;

            stack_panel.Children.Add(first_step_text);
            Grid.SetRow(stack_panel, 1);
            Grid.SetColumn(stack_panel, 0);

            plotter_cntrl.plotter = new ChartPlotter();
            margin = plotter_cntrl.plotter.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            plotter_cntrl.plotter.Margin = margin;
            Grid.SetRow(plotter_cntrl.plotter, 2);
            Grid.SetColumn(plotter_cntrl.plotter, 0);
            plotter_cntrl.plotter.Legend.Remove();
            plotter_cntrl.plotter.FitToView();

            StackPanel buttons_panel_1 = new StackPanel();
            buttons_panel_1.Orientation = Orientation.Horizontal;
            buttons_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Label tarirovka_weight_label = AuxiliaryFunc.getNewLabel("Введите массу груза, используемого для тарировки (кг.)", 200, 25, 5);
            tarirovka_weight_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            tarirovka_weight_label.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            tarirovka_weight_text = new TextBox();
            tarirovka_weight_text = (TextBox)AuxiliaryFunc.setNewControl("tarirovka_weight_text", (Control)tarirovka_weight_text, 200, 25);
            tarirovka_weight_text.Text = "10";
            tarirovka_weight_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            AuxiliaryFunc.setMargin((Control)tarirovka_weight_text, 25, 10, 0, 10);
            tarirovka_weight_text.TextAlignment = TextAlignment.Right;

            buttons_panel_1.Children.Add(tarirovka_weight_label);
            buttons_panel_1.Children.Add(tarirovka_weight_text);
            Grid.SetRow(buttons_panel_1, 3);
            Grid.SetColumn(buttons_panel_1, 0);

            balance_done_label = AuxiliaryFunc.getNewLabel("Тарировка системы не выполнена", 200, 25, 5);
            balance_done_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            balance_done_label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Button calibrate_button = AuxiliaryFunc.getNewButton("Тарировать систему", 200, 25);
            calibrate_button.Click += new RoutedEventHandler(tarirovka_2);
            AuxiliaryFunc.setMargin((Control)calibrate_button, 25, 10, 0, 10);
            calibrate_button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            StackPanel buttons_panel_1_1 = new StackPanel();
            buttons_panel_1_1.Orientation = Orientation.Horizontal;
            buttons_panel_1_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            buttons_panel_1_1.Children.Add(balance_done_label);
            buttons_panel_1_1.Children.Add(calibrate_button);
            Grid.SetRow(buttons_panel_1_1, 4);
            Grid.SetColumn(buttons_panel_1_1, 0);

            StackPanel buttons_panel_2 = new StackPanel();
            buttons_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            buttons_panel_2.Orientation = Orientation.Horizontal;

            next_button = AuxiliaryFunc.getNewButton("Готово", 200, 25);
            next_button.IsEnabled = false;
            next_button.Click += new RoutedEventHandler(tarirovka_done);
            AuxiliaryFunc.setMargin((Control)next_button, 25, 10, 0, 10);

            Button cancel_button = AuxiliaryFunc.getNewButton("Отмена", 200, 25);
            cancel_button.Click += new RoutedEventHandler(tarirovka_cancel);
            AuxiliaryFunc.setMargin((Control)cancel_button, 25, 10, 0, 10);

            buttons_panel_2.Children.Add(next_button);
            buttons_panel_2.Children.Add(cancel_button);
            Grid.SetRow(buttons_panel_2, 5);
            Grid.SetColumn(buttons_panel_2, 0);

            grid_main.Children.Add(head_panel);
            grid_main.Children.Add(stack_panel);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(buttons_panel_1);
            grid_main.Children.Add(buttons_panel_1_1);
            grid_main.Children.Add(buttons_panel_2);
            Grid.SetRow(grid_main, 0);
            Grid.SetColumn(grid_main, 0);

            // Отрисовывать уровень
            plotter_cntrl.PRECALIBRATE_FLAG = true;

            plotter_cntrl.StartGettingData();
            plotter_cntrl.StartPlotter();

            return grid_main;
        }
        
        /* Балансировка системы */
        private void tarirovka_1(object sender, RoutedEventArgs e)
        { 
            device.BALANCE_DONE = false;
            plotter_cntrl.calibrate_flag = true;
            start_tarirovka();
        }

        /* Тарировка системы */
        private void tarirovka_2(object sender, RoutedEventArgs e)
        { 
            device.tarirovka_weight = (float)Double.Parse(tarirovka_weight_text.Text.ToString());
            device.TARIROVKA_DONE = false;
            plotter_cntrl.tarirovka_flag = true;
            start_tarirovka();            
        }

        private void start_tarirovka()
        {
            plotter_cntrl.closePlotter();// Закрыть соединение
            plotter_cntrl.chart_line_source_1.DataSource = null;
            plotter_cntrl.PRECALIBRATE_FLAG = false;
            plotter_cntrl.StartPlotter();
            plotter_cntrl.StartGettingData(); 
        }

        private void tarirovka_1_done()
        {
            // Проверка выполнения балансировки
            if (device.balance_check(plotter_cntrl.channel_2, plotter_cntrl.count_channel_2))
            { 
                UpdateLabel("Балансировка выполнена");
                UpdateButton(true);
            }
            else
                UpdateLabel("Балансировка не выполнена. Повторите балансировку."); 
        }

        private void tarirovka_2_done()
        {
            // Проверка выполнения тарировки
            if (device.tarirovka_check(plotter_cntrl.channel_2, plotter_cntrl.count_channel_2))
            {
                UpdateLabel("Тарировка выполнена");
                UpdateButton(true);
            }
            else 
                UpdateLabel("Тарировка не выполнена. Повторите тарировку."); 
        } 

        private void tarirovka_step_2(object sender, RoutedEventArgs e)
        {
            evt_tarirovka_step_2.OnSomeEvent(); 
        }
        
        private void tarirovka_done(object sender, RoutedEventArgs e)
        {
            evt_tarirovka_done.OnSomeEvent();
        }

        private void tarirovka_cancel(object sender, RoutedEventArgs e)
        { 
            evt_tarirovka_cancel.OnSomeEvent();
        }

        public void getTunedDevice(out DeviceTune device_in, ref ProgramSettings program_settings_in)
        {
            // установка параметров после успешной тарировки  
            program_settings_in.set_parameters(ref device);            
            device_in = device;
        }
    }
}
