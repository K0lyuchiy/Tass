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
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.Data;

using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using ExcelLibrary;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Diagnostics;


using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;

using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;

namespace tass
{
    /// <summary>
    /// Логика взаимодействия для measuring_tab.xaml
    /// </summary>
    public partial class measuring_tab : UserControl
    {
        public MarkerEvent1 evt_cancel;
        public MarkerEvent1 evt_start_analyse;
        public MarkerEvent1 evt_CloseTab;
        public MarkerEvent1 evt_measuring_draw;
        public MarkerEvent1 evt_measuring_done;

        public ProgramSettings program_settings;   
        PlotterControl plotter_cntrl; 
        Excel_Interface excelData;              
        Template pattern;
        Template temp_template;
        print print_obj;
        tassDB tdb;          

        ProgressBar progress_bar;
        delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        UpdateProgressBarDelegate updProgress;
        float F_treshold;
        float t_treshold;

        RadioButton rb_1;
        RadioButton rb_2;
        RadioButton rb_3;
        Label lbs_1;
        TextBox number_text;
        TextBox prefix_text;    
        CheckBox ch_1;
        CheckBox ch_2;
        CheckBox ch_3;
        CheckBox ch_4;
        CheckBox ch_template;   
        TextBox tx_time;
        TextBox tx_force;  
          
        public measuring_tab(Dispatcher disp)
        {
            plotter_cntrl = new PlotterControl(disp);
            tdb = new tassDB();
            InitializeComponent();
            evt_cancel = new MarkerEvent1();
            evt_start_analyse = new MarkerEvent1();
            evt_CloseTab = new MarkerEvent1();
            evt_measuring_draw = new MarkerEvent1();
            evt_measuring_done = new MarkerEvent1();
            excelData = new Excel_Interface();
            print_obj = new print();
        }

        public Grid measuring_tab_3alternatives(ProgramSettings program_settings_in, Template pattern_in)
        {
            program_settings = program_settings_in;
            pattern = pattern_in;

            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            row_def_1.Height = new GridLength(100);
            row_def_2.Height = new GridLength(150);
            row_def_4.Height = new GridLength(80);

            ColumnDefinition col_def_1 = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.RowDefinitions.Add(row_def_3);
            grid_main.RowDefinitions.Add(row_def_4);
            grid_main.ColumnDefinitions.Add(col_def_1);

            StackPanel stack_panel_1 = new StackPanel();
            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_1.Orientation = Orientation.Horizontal;

            StackPanel stack_panel_2 = new StackPanel();
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stack_panel_2.Orientation = Orientation.Horizontal;

            StackPanel stack_panel_3 = new StackPanel();
            stack_panel_3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stack_panel_3.Orientation = Orientation.Horizontal;

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Режим измерения\n\n ")));
            header.Width = 300;
            header.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Thickness margin = header.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            header.Margin = margin;

            stack_panel_1.Children.Add(header);

            Grid.SetRow(stack_panel_1, 0);
            Grid.SetColumn(stack_panel_1, 0);

            StackPanel stack_panel_4 = new StackPanel();
            stack_panel_4.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_4.Orientation = Orientation.Vertical;

            TextBlock parameters_info = new TextBlock();
            parameters_info.Inlines.Add((new Run(" Повторить  циклы развития усилия с параметрами:")));
            margin = parameters_info.Margin;
            margin.Bottom = 5;
            margin.Left = 50;
            margin.Top = 25;
            margin.Right = 0;
            parameters_info.Margin = margin;
            parameters_info.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Label label_f = new Label();
            label_f.Height = 25;
            label_f.Width = 200;
            label_f.Content = "F = " + program_settings.f_max.ToString() + " [Н],  t = " + program_settings.time_incr.ToString() + " [c], n = " + program_settings.num_of_attempts.ToString();
            AuxiliaryFunc.setMargin((Control)label_f, 0, 50, 25, 0);
            label_f.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            stack_panel_4.Children.Add(parameters_info);
            stack_panel_4.Children.Add(label_f);
            Grid.SetRow(stack_panel_4, 1);
            Grid.SetColumn(stack_panel_4, 0);

            rb_1 = new RadioButton();
            rb_1.Content = "Слепой режим измерений";

            rb_2 = new RadioButton();
            rb_2.Content = "Режим измерений с выключенным шаблоном";

            rb_3 = new RadioButton();
            rb_3.Content = "Режим измерений с включенным шаблоном";

            stack_panel_2.Children.Add(rb_1);
            stack_panel_2.Children.Add(rb_2);
            stack_panel_2.Children.Add(rb_3);

            stack_panel_2.Orientation = Orientation.Vertical;

            Grid.SetRow(stack_panel_2, 2);
            Grid.SetColumn(stack_panel_2, 0);

            Button button_1 = AuxiliaryFunc.getNewButton("Старт", 200, 25);
            AuxiliaryFunc.setMargin((Control)button_1, 25, 10, 0, 10);

            button_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_1.Click += new RoutedEventHandler(go_to_measuring);

            Button button_2 = AuxiliaryFunc.getNewButton("Отмена", 200, 25);
            AuxiliaryFunc.setMargin((Control)button_2, 25, 10, 0, 10);
            button_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            button_2.Click += new RoutedEventHandler(cancel_measuring);

            stack_panel_3.Children.Add(button_1);
            stack_panel_3.Children.Add(button_2);
            stack_panel_3.Orientation = Orientation.Horizontal;
            stack_panel_3.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Grid.SetRow(stack_panel_3, 3);
            Grid.SetColumn(stack_panel_3, 0);

            grid_main.Children.Add(stack_panel_1);
            grid_main.Children.Add(stack_panel_2);
            grid_main.Children.Add(stack_panel_3);
            grid_main.Children.Add(stack_panel_4);

            return grid_main;
        }

        public Grid measuring_draw()
        {
            int mode_flag = plotter_cntrl.FLAG_MEASURING_MODE;

            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            RowDefinition row_def_5 = new RowDefinition();
            row_def_1.Height = new GridLength(120);
            row_def_2.Height = new GridLength(80);
            row_def_4.Height = new GridLength(180);
            row_def_5.Height = new GridLength(80);
            ColumnDefinition col_def_1 = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.RowDefinitions.Add(row_def_3);
            grid_main.RowDefinitions.Add(row_def_4);
            grid_main.RowDefinitions.Add(row_def_5);
            grid_main.ColumnDefinitions.Add(col_def_1);

            StackPanel stack_panel_top = new StackPanel();
            stack_panel_top.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_top.Orientation = Orientation.Vertical;

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Измерение\n\n ")));
            header.Width = 300;
            header.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Thickness margin = header.Margin;
            margin.Bottom = 0;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            header.Margin = margin;

            Label label_start_measuring = new Label();
            label_start_measuring.Content = "Нажмите старт для начала измерений";
            label_start_measuring.Height = 25;
            label_start_measuring.Width = 300;
            AuxiliaryFunc.setMargin((Control)label_start_measuring, 0, 25, 10, 0);

            stack_panel_top.Children.Add(header);
            stack_panel_top.Children.Add(label_start_measuring);

            Grid.SetRow(stack_panel_top, 0);
            Grid.SetColumn(stack_panel_top, 0);

            StackPanel st_1 = new StackPanel();

            st_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            st_1.Orientation = Orientation.Vertical;

            Label label_measuring = new Label();
            label_measuring.Height = 35;
            label_measuring.Width = 200;
            label_measuring.FontSize = 16;
            label_measuring.FontWeight = FontWeights.Bold;
            label_measuring.Content = "ИДЕТ ИЗМЕРЕНИЕ";
            AuxiliaryFunc.setMargin((Control)label_measuring, 0, 50, 25, 0);
            label_measuring.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            st_1.Children.Add(label_measuring);

            Grid.SetRow(st_1, 1);
            Grid.SetColumn(st_1, 0);

            Expander set_expander = new Expander();
            switch (mode_flag)
            {
                case 1:
                    plotter_cntrl.SHOW_PATTERN = true;
                    progress_bar = new ProgressBar();
                    progress_bar.Maximum = pattern.length;
                    progress_bar.Value = 0;

                    plotter_cntrl.plotter = new ChartPlotter();
                    st_1.Children.Add(progress_bar);
                    break;
                case 2:
                    plotter_cntrl.SHOW_PATTERN = true;
                    plotter_cntrl.plotter = new ChartPlotter();
                    plotter_cntrl.plotter.Legend.Remove();
                    plotter_cntrl.plotter.AxisGrid.Remove();
                    Grid.SetRow(plotter_cntrl.plotter, 2);
                    Grid.SetColumn(plotter_cntrl.plotter, 0);
                    grid_main.Children.Add(plotter_cntrl.plotter);
                    Grid settings_grid = new Grid();
                    RowDefinition row_1__ = new RowDefinition();
                    RowDefinition row_2__ = new RowDefinition();
                    RowDefinition row_3__ = new RowDefinition();
                    RowDefinition row_4__ = new RowDefinition();
                    ColumnDefinition col_1__ = new ColumnDefinition();
                    ColumnDefinition col_2__ = new ColumnDefinition();
                    col_1__.Width = new GridLength(100);
                    settings_grid.RowDefinitions.Add(row_1__);
                    settings_grid.RowDefinitions.Add(row_2__);
                    settings_grid.RowDefinitions.Add(row_3__);
                    settings_grid.RowDefinitions.Add(row_4__);
                    settings_grid.ColumnDefinitions.Add(col_1__);
                    settings_grid.ColumnDefinitions.Add(col_2__);
                    StackPanel st_set = new StackPanel();
                    Label lb_set_0 = AuxiliaryFunc.getNewLabel("F", 200, 25, 0);
                    lb_set_0.Content = "Fmax = ";
                    AuxiliaryFunc.setMargin((Control)lb_set_0, 0, 0, 0, 0);
                    lb_set_0.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    Grid.SetRow(lb_set_0, 0);
                    Grid.SetColumn(lb_set_0, 0);
                    StackPanel stack_1 = new StackPanel();
                    stack_1.Orientation = Orientation.Horizontal;
                    tx_force = new TextBox();
                    // установка диапазона
                    F_treshold = 2 * program_settings.f_max;
                    tx_force.Text = 2.ToString();
                    tx_force.Height = 25;
                    tx_force.Width = 50;
                    Label lb_set_0_1 = AuxiliaryFunc.getNewLabel("Fmax_", 200, 25, 0);
                    lb_set_0_1.Content = "F";
                    AuxiliaryFunc.setMargin((Control)lb_set_0_1, 0, 0, 0, 0);
                    lb_set_0_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                    stack_1.Children.Add(tx_force);
                    stack_1.Children.Add(lb_set_0_1);
                    Grid.SetRow(stack_1, 0);
                    Grid.SetColumn(stack_1, 1);

                    Label lb_set_1 = AuxiliaryFunc.getNewLabel("sh", 200, 25, 0);
                    lb_set_1.Content = "tполн.экр. = ";
                    AuxiliaryFunc.setMargin((Control)lb_set_1, 0, 0, 0, 0);
                    lb_set_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                    Grid.SetRow(lb_set_1, 1);
                    Grid.SetColumn(lb_set_1, 0);

                    StackPanel stack_2 = new StackPanel();
                    stack_2.Orientation = Orientation.Horizontal;

                    tx_time = new TextBox();
                    tx_time.Height = 25;
                    tx_time.Width = 50;

                    // установка диапазона
                    t_treshold = 3 * pattern.time_axis;
                    tx_time.Text = 3.ToString();
                    plotter_cntrl.set_scale();

                    Label lb_set_1_1 = AuxiliaryFunc.getNewLabel("tp", 200, 25, 0);
                    lb_set_1_1.Content = "tп";
                    AuxiliaryFunc.setMargin((Control)lb_set_1_1, 0, 0, 0, 0);
                    lb_set_1_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                    stack_2.Children.Add(tx_time);
                    stack_2.Children.Add(lb_set_1_1);
                    Grid.SetRow(stack_2, 1);
                    Grid.SetColumn(stack_2, 1);

                    Button button_set_2 = AuxiliaryFunc.getNewButton("Установить коэффициенты", 200, 25);
                    button_set_2.Click += new RoutedEventHandler(set_set);
                    button_set_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    button_set_2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    AuxiliaryFunc.setMargin((Control)button_set_2, 2, 2, 2, 2);
                    Grid.SetRow(button_set_2, 2);
                    Grid.SetColumn(button_set_2, 1);

                    Button button_set_3 = AuxiliaryFunc.getNewButton("Установить по умолчанию", 200, 25);
                    button_set_3.Click += new RoutedEventHandler(set_default_set);
                    button_set_3.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    button_set_3.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    AuxiliaryFunc.setMargin((Control)button_set_3, 2, 2, 2, 2);

                    Grid.SetRow(button_set_3, 3);
                    Grid.SetColumn(button_set_3, 1);

                    settings_grid.Children.Add(lb_set_0);
                    settings_grid.Children.Add(stack_1);
                    settings_grid.Children.Add(lb_set_1);
                    settings_grid.Children.Add(stack_2);

                    settings_grid.Children.Add(button_set_2);
                    settings_grid.Children.Add(button_set_3);

                    set_expander.Header = "Настройки графика";
                    set_expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                    margin.Bottom = 2;
                    margin.Left = 2;
                    margin.Top = 2;
                    margin.Right = 2;
                    set_expander.BorderThickness = margin;

                    st_set.Children.Add(settings_grid);
                    set_expander.Content = st_set;
                    set_expander.Width = 400;
                    st_set.Background = Brushes.LightGray;

                    AuxiliaryFunc.setMargin((Control)set_expander, 25, 10, 0, 10);

                    Grid.SetRow(set_expander, 3);
                    Grid.SetColumn(set_expander, 0);
                    break;
                case 3:
                    plotter_cntrl.TRAIN_FLAG = true;
                    plotter_cntrl.plotter = new ChartPlotter();
                    Grid.SetRow(plotter_cntrl.plotter, 2);
                    Grid.SetColumn(plotter_cntrl.plotter, 0);
                    grid_main.Children.Add(plotter_cntrl.plotter);
                    break;
                default:
                    break;
            }

            StackPanel stack_panel_1 = new StackPanel();
            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            stack_panel_1.Orientation = Orientation.Horizontal;

            Button button_1 = AuxiliaryFunc.getNewButton("Старт/Пауза", 200, 25);
            button_1.Click += new RoutedEventHandler(start_measuring);
            AuxiliaryFunc.setMargin((Control)button_1, 25, 10, 0, 10);
            button_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Button button_2 = AuxiliaryFunc.getNewButton("Готово", 200, 25);
            button_2.Click += new RoutedEventHandler(measuring_done);
            AuxiliaryFunc.setMargin((Control)button_2, 25, 10, 0, 10);
            button_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            stack_panel_1.Orientation = Orientation.Horizontal;
            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            stack_panel_1.Children.Add(button_1);
            stack_panel_1.Children.Add(button_2);
            Grid.SetRow(stack_panel_1, 4);
            Grid.SetColumn(stack_panel_1, 0);

            grid_main.Children.Add(set_expander);
            grid_main.Children.Add(stack_panel_top);
            grid_main.Children.Add(st_1);
            grid_main.Children.Add(stack_panel_1);

            return grid_main;
        }
        
        public Grid draw_measuring_done()
        {
            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            RowDefinition row_def_5 = new RowDefinition();

            row_def_1.Height = new GridLength(80);
            row_def_3.Height = new GridLength(50);
            row_def_4.Height = new GridLength(225);
            row_def_5.Height = new GridLength(80);
            ColumnDefinition col_def_1 = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.RowDefinitions.Add(row_def_3);
            grid_main.RowDefinitions.Add(row_def_4);
            grid_main.RowDefinitions.Add(row_def_5);
            grid_main.ColumnDefinitions.Add(col_def_1);

            StackPanel stack_panel_1 = new StackPanel();
            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_1.Orientation = Orientation.Horizontal;

            StackPanel stack_panel_2 = new StackPanel();
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_2.Orientation = Orientation.Horizontal;

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Результаты измерений\n\n ")));
            header.Width = 300;
            header.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Thickness margin = header.Margin;
            margin.Bottom = 0;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            header.Margin = margin;

            Grid.SetRow(header, 0);
            Grid.SetColumn(header, 0);

            plotter_cntrl.plotter = new ChartPlotter();
            plotter_cntrl.plotter.Legend.Remove();
            plotter_cntrl.plotter.FitToView();
            Grid.SetRow(plotter_cntrl.plotter, 1);
            Grid.SetColumn(plotter_cntrl.plotter, 0);

            // Отрисовка измерений
            plotter_cntrl.plot_measured_data();

            StackPanel template_shift = new StackPanel();
            margin = template_shift.Margin;
            margin.Bottom = 5;
            margin.Left = 10;
            margin.Top = 10;
            margin.Right = 5;
            template_shift.Margin = margin;

            RepeatButton button_left = new RepeatButton();
            button_left.Content = "<<";
            button_left.Height = 25;
            button_left.Width = 100;
            button_left.Click += new RoutedEventHandler(plotter_cntrl.shift_to_left);
            button_left.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_left.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            AuxiliaryFunc.setMargin((Control)button_left, 2, 2, 2, 2);

            ch_template = new CheckBox();
            ch_template.Checked += ch_template_Checked;
            ch_template.Unchecked += ch_template_Unchecked;
            AuxiliaryFunc.setMargin((Control)ch_template, 0, 10, 0, 0);
            button_left.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            ch_template.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Label lb_template = AuxiliaryFunc.getNewLabel("Шаблон", 100, 25, 0);
            lb_template.Content = "Шаблон";
            AuxiliaryFunc.setMargin((Control)lb_template, 0, 10, 0, 10);
            lb_template.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            RepeatButton button_right = new RepeatButton();
            button_right.Content = ">>";
            button_right.Height = 25;
            button_right.Width = 100;
            button_right.Click += new RoutedEventHandler(plotter_cntrl.shift_to_right);
            button_right.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_right.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            AuxiliaryFunc.setMargin((Control)button_right, 2, 2, 2, 2);

            template_shift.Children.Add(button_left);
            template_shift.Children.Add(ch_template);
            template_shift.Children.Add(lb_template);
            template_shift.Children.Add(button_right);
            template_shift.Orientation = Orientation.Horizontal;

            Grid.SetRow(template_shift, 2);
            Grid.SetColumn(template_shift, 0);

            StackPanel st_panel = new StackPanel();

            Grid settings_grid = AuxiliaryFunc.getTable(6, 1);

            ch_1 = new CheckBox();
            ch_1.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Label lb_set_0 = AuxiliaryFunc.getNewLabel("Шаблон", 200, 25, 0);
            lb_set_0.Content = "Шаблон:";
            AuxiliaryFunc.setMargin((Control)lb_set_0, 0, 0, 0, 0);
            lb_set_0.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            StackPanel st_1 = new StackPanel();
            st_1.Children.Add(ch_1);
            st_1.Children.Add(lb_set_0);
            st_1.Orientation = Orientation.Horizontal;
            Grid.SetRow(st_1, 0);
            Grid.SetColumn(st_1, 0);

            ch_2 = new CheckBox();
            ch_2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            ch_2.IsChecked = true;

            Label lb_set_1 = AuxiliaryFunc.getNewLabel("Кривая F(t)", 200, 25, 0);
            lb_set_1.Content = "Кривая F(t):";
            AuxiliaryFunc.setMargin((Control)lb_set_1, 0, 0, 0, 0);
            lb_set_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            StackPanel st_2 = new StackPanel();
            st_2.Children.Add(ch_2);
            st_2.Children.Add(lb_set_1);
            st_2.Orientation = Orientation.Horizontal;
            Grid.SetRow(st_2, 1);
            Grid.SetColumn(st_2, 0);

            ch_3 = new CheckBox();
            ch_3.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            ch_3.IsChecked = true;

            Label lb_set_2 = AuxiliaryFunc.getNewLabel("Координатная сетка", 200, 25, 0);
            lb_set_2.Content = "Координатная сетка";
            AuxiliaryFunc.setMargin((Control)lb_set_2, 0, 0, 0, 0);
            lb_set_2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            StackPanel st_3 = new StackPanel();
            st_3.Children.Add(ch_3);
            st_3.Children.Add(lb_set_2);
            st_3.Orientation = Orientation.Horizontal;
            Grid.SetRow(st_3, 2);
            Grid.SetColumn(st_3, 0);

            ch_4 = new CheckBox();
            ch_4.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            ch_4.IsChecked = true;

            Label lb_set_3 = AuxiliaryFunc.getNewLabel("Оси координат", 200, 25, 0);
            lb_set_3.Content = "Оси координат";
            AuxiliaryFunc.setMargin((Control)lb_set_3, 0, 0, 0, 0);
            lb_set_3.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            StackPanel st_4 = new StackPanel();
            st_4.Children.Add(ch_4);
            st_4.Children.Add(lb_set_3);
            st_4.Orientation = Orientation.Horizontal;
            Grid.SetRow(st_4, 3);
            Grid.SetColumn(st_4, 0);

            Button button_set_2 = AuxiliaryFunc.getNewButton("Установить", 100, 25);
            button_set_2.Click += new RoutedEventHandler(set_plot_settings);
            button_set_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_set_2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            AuxiliaryFunc.setMargin((Control)button_set_2, 2, 2, 2, 2);
            Grid.SetRow(button_set_2, 4);
            Grid.SetColumn(button_set_2, 0);

            Button button_set_3 = AuxiliaryFunc.getNewButton("Сброс", 100, 25);
            button_set_3.Click += new RoutedEventHandler(clear_plot_settings);
            button_set_3.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_set_3.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            AuxiliaryFunc.setMargin((Control)button_set_3, 2, 2, 2, 2);

            Grid.SetRow(button_set_3, 5);
            Grid.SetColumn(button_set_3, 0);

            Expander set_expander = new Expander();
            set_expander.Header = "Настройки графика";
            set_expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            margin = set_expander.Margin;
            margin.Bottom = 2;
            margin.Left = 2;
            margin.Top = 2;
            margin.Right = 2;
            set_expander.BorderThickness = margin;

            st_panel.Children.Add(settings_grid);
            set_expander.Content = st_panel;
            set_expander.Width = 200;
            st_panel.Background = Brushes.LightGray;

            AuxiliaryFunc.setMargin((Control)set_expander, 25, 10, 0, 10);

            settings_grid.Children.Add(st_1);
            settings_grid.Children.Add(st_2);

            settings_grid.Children.Add(st_3);

            settings_grid.Children.Add(st_4);

            settings_grid.Children.Add(button_set_2);

            settings_grid.Children.Add(button_set_3);

            Grid.SetRow(settings_grid, 3);
            Grid.SetColumn(settings_grid, 0);

            StackPanel save_panel = new StackPanel();

            Grid save_grid = AuxiliaryFunc.getTable(6, 2);

            Label lbs_0 = AuxiliaryFunc.getNewLabel("Расположение", 200, 25, 0);
            lbs_0.Content = "Расположение:";
            AuxiliaryFunc.setMargin((Control)lbs_0, 0, 0, 0, 0);
            lbs_0.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            Grid.SetRow(lbs_0, 0);
            Grid.SetColumn(lbs_0, 0);

            Button button_s_0 = AuxiliaryFunc.getNewButton("Обзор", 200, 25);
            button_s_0.Click += new RoutedEventHandler(open_folder);
            AuxiliaryFunc.setMargin((Control)button_s_0, 0, 0, 0, 0);
            button_s_0.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_s_0.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(button_s_0, 0);
            Grid.SetColumn(button_s_0, 1);

            lbs_1 = AuxiliaryFunc.getNewLabel("Расположение:", 200, 25, 0);
            AuxiliaryFunc.setMargin((Control)lbs_1, 0, 0, 0, 0);
            lbs_1.Content = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().Remove(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().IndexOf("tass.exe")) + "measurements_results\\";
            lbs_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(lbs_1, 1);
            Grid.SetColumn(lbs_1, 0);

            Label lbs_2 = AuxiliaryFunc.getNewLabel("Имя", 200, 25, 0);
            lbs_2.Content = "Имя файла (префикс+трехзначный номер):";
            Grid.SetRow(lbs_2, 2);
            Grid.SetColumn(lbs_2, 0);

            StackPanel sp_0 = new StackPanel();
            sp_0.Orientation = Orientation.Horizontal;

            Label lbs_3 = AuxiliaryFunc.getNewLabel("Префикс", 100, 25, 0);
            lbs_3.Content = "Префикс:";
            lbs_3.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            prefix_text = new TextBox();
            prefix_text = (TextBox)AuxiliaryFunc.setNewControl("prefix_text", (Control)prefix_text, 200, 25, 0);
            prefix_text.Text = "measurement";
            prefix_text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            sp_0.Children.Add(lbs_3);
            sp_0.Children.Add(prefix_text);
            Grid.SetRow(sp_0, 3);
            Grid.SetColumn(sp_0, 0);

            StackPanel sp_1 = new StackPanel();
            sp_1.Orientation = Orientation.Horizontal;

            Label lbs_4 = AuxiliaryFunc.getNewLabel("Номер:", 100, 25, 0);
            lbs_4.Content = "Номер:";
            lbs_4.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(lbs_4, 3);
            Grid.SetColumn(lbs_4, 1);

            number_text = new TextBox();
            number_text = (TextBox)AuxiliaryFunc.setNewControl("number_text", (Control)number_text, 100, 25);
            number_text.Text = (tdb.get_max_idx() + 1).ToString();
            number_text.IsReadOnly = true;
            AuxiliaryFunc.setMargin((Control)button_s_0, 0, 0, 0, 5);
            number_text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            number_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            Grid.SetRow(number_text, 3);
            Grid.SetColumn(number_text, 1);

            sp_1.Children.Add(lbs_4);
            sp_1.Children.Add(number_text);
            Grid.SetRow(sp_1, 3);
            Grid.SetColumn(sp_1, 1);

            Button button_2 = AuxiliaryFunc.getNewButton("Сохранить", 200, 25);
            button_2.Click += new RoutedEventHandler(save_measuring);
            button_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            AuxiliaryFunc.setMargin((Control)button_2, 25, 0, 0, 5);
            Grid.SetRow(button_2, 4);
            Grid.SetColumn(button_2, 1);

            save_grid.Children.Add(lbs_0);
            save_grid.Children.Add(button_s_0);
            save_grid.Children.Add(lbs_1);
            save_grid.Children.Add(lbs_2);
            save_grid.Children.Add(sp_0);
            save_grid.Children.Add(sp_1);
            save_grid.Children.Add(button_2);

            save_panel.Children.Add(save_grid);

            Expander save_expander = new Expander();
            save_expander.Header = "Сохранить результаты измерений";
            save_expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            margin.Bottom = 2;
            margin.Left = 2;
            margin.Top = 2;
            margin.Right = 2;
            save_expander.BorderThickness = margin;

            save_expander.Content = save_panel;
            save_panel.Background = Brushes.LightGray;

            AuxiliaryFunc.setMargin((Control)save_expander, 25, 10, 0, 10);
            stack_panel_1.Children.Add(set_expander);
            set_expander.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_1.Children.Add(save_expander);

            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Grid.SetRow(stack_panel_1, 3);
            Grid.SetColumn(stack_panel_1, 0);

            Button button_1 = AuxiliaryFunc.getNewButton("Печать", 200, 25);
            button_1.Click += new RoutedEventHandler(print_measures);
            AuxiliaryFunc.setMargin((Control)button_1, 25, 10, 0, 10);

            Button button_3 = AuxiliaryFunc.getNewButton("Повторить измерения", 200, 25);
            button_3.Click += new RoutedEventHandler(repeat_measuring);
            AuxiliaryFunc.setMargin((Control)button_3, 25, 10, 0, 10);

            Button button_4 = AuxiliaryFunc.getNewButton("Начать анализ", 200, 25);
            button_4.Click += new RoutedEventHandler(start_analyse);
            AuxiliaryFunc.setMargin((Control)button_4, 25, 10, 0, 10);

            stack_panel_2.Children.Add(button_1);
            stack_panel_2.Children.Add(button_3);
            stack_panel_2.Children.Add(button_4);
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            stack_panel_2.Orientation = Orientation.Horizontal;

            Grid.SetRow(stack_panel_2, 4);
            Grid.SetColumn(stack_panel_2, 0);

            grid_main.Children.Add(header);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(template_shift);
            grid_main.Children.Add(stack_panel_1);
            grid_main.Children.Add(stack_panel_2);

            return grid_main;
        }

        private void go_to_measuring(object sender, RoutedEventArgs e)
        {
            if ((bool)rb_1.IsChecked)
                plotter_cntrl.FLAG_MEASURING_MODE = 1;
            if ((bool)rb_2.IsChecked)
                plotter_cntrl.FLAG_MEASURING_MODE = 2;
            if ((bool)rb_3.IsChecked)
                plotter_cntrl.FLAG_MEASURING_MODE = 3; 
            evt_measuring_draw.OnSomeEvent();
        }

        private void start_measuring(object sender, RoutedEventArgs e)
        {
            if (plotter_cntrl.FLAG_MEASURING_MODE == 1)
            {
                updProgress = new UpdateProgressBarDelegate(progress_bar.SetValue); 
            }
            pattern.create_template();
            plotter_cntrl.StartGettingData();
        }

        private void measuring_done(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.SHOW_PATTERN = false;
            plotter_cntrl.TRAIN_FLAG = false;
            evt_measuring_done.OnSomeEvent();           
        }

        private void start_analyse(object sender, RoutedEventArgs e)
        {
            evt_start_analyse.OnSomeEvent();
        }

        private void cancel_measuring(object sender, RoutedEventArgs e)
        {
            close_tab();
            evt_cancel.OnSomeEvent();
        }

        private void repeat_measuring(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.SHOW_PATTERN = false;
            plotter_cntrl.TRAIN_FLAG = false;
            evt_measuring_draw.OnSomeEvent();
        }

        public void close_tab()
        {
            evt_CloseTab.OnSomeEvent();
        }

        private void print_measures(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.plotter.SaveScreenshot(Directory.GetCurrentDirectory() + "\\file.jpg");
            print_obj.print_measures();
        }

        private void open_folder(object sender, RoutedEventArgs e)
        { 
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";

            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                lbs_1.Content = folderDialog.SelectedPath;
        }

        private void ch_template_Unchecked(object sender, RoutedEventArgs e)
        {
            pattern = temp_template;
            if (ch_1 != null)
                ch_1.IsChecked = false;
            plotter_cntrl.ch_template = false;
            plotter_cntrl.delete_template();
        }

        private void ch_template_Checked(object sender, RoutedEventArgs e)
        {
            temp_template = pattern;
            if (ch_1 != null)
                ch_1.IsChecked = true;
            plotter_cntrl.ch_template = true;
            plotter_cntrl.plot_template();
        }

        private void clear_plot_settings(object sender, RoutedEventArgs e)
        {
            ch_template.IsChecked = false;
            plotter_cntrl.ch_template = false;
            ch_1.IsChecked = false;
            ch_2.IsChecked = false;
            ch_3.IsChecked = false;
            ch_4.IsChecked = false;
            set_plot_setings();
        }

        private void set_plot_settings(object sender, RoutedEventArgs e)
        {
            set_plot_setings();
        }

        private void set_plot_setings()
        {
            if (ch_1.IsChecked.Value)
            {
                ch_template.IsChecked = true;
                plotter_cntrl.ch_template = true;
            }
            else
            {
                ch_template.IsChecked = false;
                plotter_cntrl.ch_template = false;
            }
            if (ch_2.IsChecked.Value)
                plotter_cntrl.plot_signal();
            else
            {
                plotter_cntrl.delete_signal();
            }
            if (ch_3.IsChecked.Value)
                plotter_cntrl.plot_grid();
            else
            {
                plotter_cntrl.clear_grid();
            }
            if (ch_4.IsChecked.Value)
                plotter_cntrl.plot_axis();
            else
            {
                plotter_cntrl.clear_axis();
            }
        }

        private void set_default_set(object sender, RoutedEventArgs e)
        {
            F_treshold = 2 * program_settings.f_max;
            t_treshold = 3 * pattern.time_axis;
            plotter_cntrl.set_scale();
        }

        private void set_set(object sender, RoutedEventArgs e)
        {
            F_treshold = Single.Parse(tx_force.Text.ToString()) * program_settings.f_max;
            t_treshold = Single.Parse(tx_time.Text.ToString()) * pattern.time_axis;
            plotter_cntrl.set_scale();
        }

        private void save_measuring(object sender, RoutedEventArgs e)
        {
            string file_name = lbs_1.Content.ToString() + prefix_text.Text.ToString() + "_" + number_text.Text.ToString() + ".xls";
            tdb.SaveMeasuringToSQLite(prefix_text.Text.ToString(), program_settings.num_of_template.ToString(), number_text.Text.ToString(), lbs_1.Content.ToString());
            excelData.save_to_excel_file(file_name, plotter_cntrl.channel_2, plotter_cntrl.count_channel_2, plotter_cntrl.timeCoordinate, program_settings);
        }
    }
}
