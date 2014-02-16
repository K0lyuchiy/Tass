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
    /// Логика взаимодействия для calculating_tab.xaml
    /// </summary>
    public partial class calculating_tab : UserControl
    {
        public MarkerEvent1 evtT;
        public MarkerEvent1 evtT1;
        public MarkerEvent1 evtT2;
        public MarkerEvent1 evt_CloseTab;
        public MarkerEvent1 evt_auto_calculate;

        Microsoft.Research.DynamicDataDisplay.Charts.Navigation.CursorCoordinateGraph plotCurs;
        Template temp_template;
        Excel_Interface excelData;   
        PlotterControl plotter_cntrl;
        print print_obj;
        public ProgramSettings program_settings;
        Template pattern;
        Calculations calculation_op;   
        tassDB tdb; 

        Grid grid_table_print;
        Grid grid_table_print_2;          
        ListView list_of_data;
        RepeatButton button_right;
        RepeatButton button_left;
        CheckBox ch_1;
        CheckBox ch_2;
        CheckBox ch_3;
        CheckBox ch_4;
        CheckBox ch_template;    
        Expander download_data;
        TextBox num_of_exp_text;
        TextBox name_of_exp_text;

        public calculating_tab(Dispatcher disp)
        {
            plotter_cntrl = new PlotterControl(disp);
            InitializeComponent();

            evtT = new MarkerEvent1();
            evtT1 = new MarkerEvent1();
            evt_CloseTab = new MarkerEvent1();
            evt_auto_calculate = new MarkerEvent1();
            tdb = new tassDB();
            excelData = new Excel_Interface();
            calculation_op = new Calculations();
            print_obj = new print();
        }

        public Grid draw_calculating()
        {
            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();

            RowDefinition row_def_4 = new RowDefinition();
            RowDefinition row_def_5 = new RowDefinition();
            RowDefinition row_def_6 = new RowDefinition();
            RowDefinition row_def_7 = new RowDefinition();
            ColumnDefinition col_def_1 = new ColumnDefinition();
            row_def_1.Height = new GridLength(80);
            row_def_2.Height = new GridLength(30);
            row_def_3.Height = new GridLength(50);

            row_def_4.Height = new GridLength(50);

            row_def_6.Height = new GridLength(50);
            row_def_7.Height = new GridLength(50);

            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.RowDefinitions.Add(row_def_3);
            grid_main.RowDefinitions.Add(row_def_4);
            grid_main.RowDefinitions.Add(row_def_5);
            grid_main.RowDefinitions.Add(row_def_6);

            grid_main.RowDefinitions.Add(row_def_7);
            grid_main.ColumnDefinitions.Add(col_def_1);

            StackPanel stack_panel_1 = new StackPanel();
            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_1.Orientation = Orientation.Horizontal;

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Расчеты\n\n ")));
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

            Label label_1 = AuxiliaryFunc.getNewLabel("Выберите данные для расчетов:", 200, 25, 0);
            AuxiliaryFunc.setMargin((Control)label_1, 0, 25, 0, 0);
            Grid.SetRow(label_1, 1);
            Grid.SetColumn(label_1, 0);

            Button button_1 = AuxiliaryFunc.getNewButton("Текущий эксперимент", 200, 25);
            AuxiliaryFunc.setMargin((Control)button_1, 0, 25, 0, 25);
            button_1.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            button_1.Click += new RoutedEventHandler(get_current_experiment);

            /////////---------------------------------------------------------
            Canvas cvs = new Canvas();
            Canvas.SetLeft(cvs, 99);

            download_data = new Expander();
            download_data.Header = "Загрузить данные из БД";
            download_data.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            download_data.Width = 365;

            StackPanel expander_panel_download = new StackPanel();
            expander_panel_download.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            expander_panel_download.Width = 364;
            list_of_data = new ListView();

            GridView myGridView = new GridView();

            myGridView.AllowsColumnReorder = true;
            myGridView.ColumnHeaderToolTip = "Записи базы данных";

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("Name_of_measurement");
            gvc1.Header = "Имя измерения";
            gvc1.Width = 120;
            myGridView.Columns.Add(gvc1);
            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new Binding("Num_of_measurement");
            gvc2.Header = "Номер измерения";
            gvc2.Width = 120;
            myGridView.Columns.Add(gvc2);

            GridViewColumn gvc3 = new GridViewColumn();
            gvc3.DisplayMemberBinding = new Binding("Date");
            gvc3.Header = "Время измерения";
            gvc3.Width = 120;
            myGridView.Columns.Add(gvc3); 

            list_of_data.View = myGridView;

            StackPanel st_but = new StackPanel();
            st_but.Orientation = Orientation.Horizontal;
            st_but.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Button button_2 = AuxiliaryFunc.getNewButton("Загрузить данные из БД", 200, 25, 0);
            // button_2.Content = "Загрузить данные из БД";
            button_2.Click += new RoutedEventHandler(get_from_db);
            button_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            st_but.Children.Add(button_2);
            st_but.Background = Brushes.LightGray;

            expander_panel_download.Children.Add(list_of_data);
            expander_panel_download.Children.Add(st_but);
            expander_panel_download.Orientation = Orientation.Vertical;

            download_data.Content = expander_panel_download;

            query_data();

            cvs.Children.Add(download_data);

            /////------------------------------------------------------------

            stack_panel_1.Children.Add(button_1);
            stack_panel_1.Children.Add(cvs);
            stack_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            margin = stack_panel_1.Margin;
            margin.Bottom = 0;
            margin.Left = 25;
            margin.Top = 10;
            margin.Right = 0;
            stack_panel_1.Margin = margin;

            Grid.SetRow(stack_panel_1, 2);
            Grid.SetColumn(stack_panel_1, 0);

            StackPanel panel_exp_num = new StackPanel();
            panel_exp_num.Orientation = Orientation.Horizontal;
            panel_exp_num.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            Label label_num_exp = AuxiliaryFunc.getNewLabel("Выбран эксперимент № ", 200, 25, 0);
            label_num_exp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            label_num_exp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            num_of_exp_text = new TextBox();
            num_of_exp_text = (TextBox)AuxiliaryFunc.setNewControl("num_of_exp_text", (Control)num_of_exp_text, 200, 25);
            AuxiliaryFunc.setMargin((Control)num_of_exp_text, 0, 10, 0, 25);
            num_of_exp_text.IsEnabled = false;

            Label label_name_exp = AuxiliaryFunc.getNewLabel("Название эксперимента ", 200, 25, 0);
            label_name_exp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            label_name_exp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            name_of_exp_text = new TextBox();
            name_of_exp_text = (TextBox)AuxiliaryFunc.setNewControl("name_of_exp_text", (Control)name_of_exp_text, 200, 25);
            AuxiliaryFunc.setMargin((Control)name_of_exp_text, 0, 10, 0, 25);

            panel_exp_num.Children.Add(label_num_exp);
            panel_exp_num.Children.Add(num_of_exp_text);
            panel_exp_num.Children.Add(label_name_exp);
            panel_exp_num.Children.Add(name_of_exp_text);

            Grid.SetRow(panel_exp_num, 3);
            Grid.SetColumn(panel_exp_num, 0);

            plotter_cntrl.plotter = new ChartPlotter();
            plotter_cntrl.plotter.Legend.Remove();
            Grid.SetRow(plotter_cntrl.plotter, 4);
            Grid.SetColumn(plotter_cntrl.plotter, 0);
            plotter_cntrl.plotter.FitToView();
            
            StackPanel template_shift = new StackPanel();
            margin = template_shift.Margin;
            margin.Bottom = 5;
            margin.Left = 10;
            margin.Top = 10;
            margin.Right = 5;
            template_shift.Margin = margin;

            button_left = new RepeatButton();
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

            button_right = new RepeatButton();
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

            Grid.SetRow(template_shift, 5);
            Grid.SetColumn(template_shift, 0);

            StackPanel stack_panel_buttons = new StackPanel();

            Button button_1_ = AuxiliaryFunc.getNewButton("Оцифровка", 200, 25);
            button_1_.Click += new RoutedEventHandler(digitization);
            AuxiliaryFunc.setMargin((Control)button_1_, 25, 10, 0, 10);

            Button button_2_ = AuxiliaryFunc.getNewButton("Автоматическая оцифровка", 200, 25);
            button_2_.Click += new RoutedEventHandler(auto_digitization);
            AuxiliaryFunc.setMargin((Control)button_2_, 25, 10, 0, 10);

            stack_panel_buttons.Children.Add(button_1_);
            stack_panel_buttons.Children.Add(button_2_);
            stack_panel_buttons.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            stack_panel_buttons.Orientation = Orientation.Horizontal;

            Grid.SetRow(stack_panel_buttons, 6);
            Grid.SetColumn(stack_panel_buttons, 0);

            grid_main.Children.Add(header);
            grid_main.Children.Add(label_1);
            grid_main.Children.Add(panel_exp_num);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(template_shift);
            grid_main.Children.Add(stack_panel_1);
            grid_main.Children.Add(stack_panel_buttons);

            return grid_main;
        }

        public void digitization(object sender, RoutedEventArgs e)
        {
            evtT.OnSomeEvent();
        }

        public void close_tab()
        {
            evt_CloseTab.OnSomeEvent();
        }

        public Grid digitization()
        {
            // расчеты в ручном режиме
            close_tab();
            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
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
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_2.Orientation = Orientation.Horizontal;

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Расчеты\n\n ")));
            header.Inlines.Add(new Run(" Оцифровка в ручном режиме\n\n "));
            header.Width = 300;
            header.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Grid.SetRow(header, 0);
            Grid.SetColumn(header, 0);

            plotter_cntrl.plotter = new ChartPlotter(); 

            plotCurs = new Microsoft.Research.DynamicDataDisplay.Charts.Navigation.CursorCoordinateGraph();
            plotter_cntrl.plotter.AddChild(plotCurs);
            //////
            Grid.SetRow(plotter_cntrl.plotter, 1);
            Grid.SetColumn(plotter_cntrl.plotter, 0);

            Button button_1 = AuxiliaryFunc.getNewButton("Повторить оцифровку в ручном режиме", 200, 25, 10, 200);
            button_1.Click += new RoutedEventHandler(digitization);
            button_1.IsEnabled = false;

            Button button_2 = AuxiliaryFunc.getNewButton("Печать", 200, 25, 10, 200); 
            button_2.Click += new RoutedEventHandler(printing_1);
            button_2.IsEnabled = false;

            Button button_3 = AuxiliaryFunc.getNewButton("Автоматические расчеты", 200, 25, 10, 200);
            button_3.Click += new RoutedEventHandler(auto_digitization);
            button_3.IsEnabled = false;

            stack_panel_2.Children.Add(button_1);
            stack_panel_2.Children.Add(button_2);
            stack_panel_2.Children.Add(button_3);
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Grid.SetRow(stack_panel_2, 3);
            Grid.SetColumn(stack_panel_2, 0);

            grid_main.Children.Add(header);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(stack_panel_1);
            grid_main.Children.Add(stack_panel_2);

            plotter_cntrl.plot_measured_data();
            digitize_measurement();

            return grid_main;
        }

        private void auto_digitization(object sender, RoutedEventArgs e)
        {
            close_tab();
            evt_auto_calculate.OnSomeEvent();
           
        }


        public Grid draw_calculating_2()
        {
            // Оцифровка в автоматическом режиме 
            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            row_def_1.Height = new GridLength(80);
            row_def_3.Height = new GridLength(80);
            RowDefinition row_def_5 = new RowDefinition();
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
            header.Inlines.Add(new Bold(new Run(" Расчеты\n\n ")));
            header.Inlines.Add(new Run(" Оцифровка в автоматическом режиме\n\n "));
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
            Grid.SetRow(plotter_cntrl.plotter, 1);
            Grid.SetColumn(plotter_cntrl.plotter, 0);
            plotter_cntrl.plotter.Legend.Remove();
            plotter_cntrl.plotter.FitToView();

            StackPanel template_shift = new StackPanel();
            margin = template_shift.Margin;
            margin.Bottom = 5;
            margin.Left = 25;
            margin.Top = 10;
            margin.Right = 5;
            template_shift.Margin = margin;

            button_left = new RepeatButton();
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

            button_right = new RepeatButton();
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

            plotter_cntrl.plot_measured_data();
            // массив содержащий индексы минимумов и максимумов
            int[] temp_array = digitize_measurement();
            calculation_op.time_force_array = temp_array;

            grid_table_print = new Grid();
            //grid_table_print.ShowGridLines = true;
            RowDefinition row_1 = new RowDefinition();

            RowDefinition row_2 = new RowDefinition();
            RowDefinition row_3 = new RowDefinition();
            RowDefinition row_4 = new RowDefinition();
            ColumnDefinition col_1 = new ColumnDefinition();
            grid_table_print.RowDefinitions.Add(row_1);
            grid_table_print.RowDefinitions.Add(row_2);
            grid_table_print.RowDefinitions.Add(row_3);
            grid_table_print.RowDefinitions.Add(row_4);
            grid_table_print.ColumnDefinitions.Add(col_1);

            Rectangle rect0 = new Rectangle();
            rect0.Stroke = Brushes.Black;
            Grid.SetRow(rect0, 0);
            Grid.SetColumn(rect0, 0);

            Label lb_ = AuxiliaryFunc.getNewLabel("##", 30, 25, 2);
            lb_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb_, 0);
            Grid.SetColumn(lb_, 0);

            Rectangle rect01 = new Rectangle();
            rect01.Stroke = Brushes.Black;
            Grid.SetRow(rect01, 1);
            Grid.SetColumn(rect01, 0);

            Label lb0_ = AuxiliaryFunc.getNewLabel("F (Н)", 30, 25, 2);
            lb0_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb0_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb0_, 1);
            Grid.SetColumn(lb0_, 0);

            Rectangle rect02 = new Rectangle();
            rect02.Stroke = Brushes.Black;
            Grid.SetRow(rect02, 2);
            Grid.SetColumn(rect02, 0);

            Label lb1_ = AuxiliaryFunc.getNewLabel("t1 (с)", 30, 25, 2);
            lb1_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb1_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb1_, 2);
            Grid.SetColumn(lb1_, 0);

            Rectangle rect03 = new Rectangle();
            rect03.Stroke = Brushes.Black;
            Grid.SetRow(rect03, 3);
            Grid.SetColumn(rect03, 0);

            Label lb2_ = AuxiliaryFunc.getNewLabel("t2 (с)", 30, 25, 2);
            lb2_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb2_.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetRow(lb2_, 3);
            Grid.SetColumn(lb2_, 0);

            grid_table_print.Children.Add(rect01);
            grid_table_print.Children.Add(rect02);
            grid_table_print.Children.Add(rect03);
            grid_table_print.Children.Add(rect0);
            grid_table_print.Children.Add(lb0_);
            grid_table_print.Children.Add(lb_);
            grid_table_print.Children.Add(lb1_);
            grid_table_print.Children.Add(lb2_);

            int num_of_col = 1;
            for (int f = 1; f < 300; f = f + 2)
            {
                if (temp_array[f] == 0)
                    break;
                num_of_col++;

                ColumnDefinition col = new ColumnDefinition();
                grid_table_print.ColumnDefinitions.Add(col);

                Rectangle rect1 = new Rectangle();
                rect1.Stroke = Brushes.Gray;
                Grid.SetRow(rect1, 1);
                Grid.SetColumn(rect1, num_of_col - 1);

                Label lb = AuxiliaryFunc.getNewLabel(plotter_cntrl.channel_2[temp_array[f + 1]].ToString(), 30, 25, 5);
                lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb, 1);
                Grid.SetColumn(lb, num_of_col - 1);

                Rectangle rect2 = new Rectangle();
                rect2.Stroke = Brushes.Gray;
                Grid.SetRow(rect2, 0);
                Grid.SetColumn(rect2, num_of_col - 1);

                Label lb0 = AuxiliaryFunc.getNewLabel((num_of_col - 1).ToString(), 30, 25, 5);
                lb0.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb0.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb0, 0);
                Grid.SetColumn(lb0, num_of_col - 1);

                Rectangle rect3 = new Rectangle();
                rect3.Stroke = Brushes.Gray;
                Grid.SetRow(rect3, 2);
                Grid.SetColumn(rect3, num_of_col - 1);

                Label lb1 = AuxiliaryFunc.getNewLabel(plotter_cntrl.timeCoordinate[temp_array[f]].ToString(), 30, 25, 5);
                lb1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb1, 2);
                Grid.SetColumn(lb1, num_of_col - 1);

                Rectangle rect4 = new Rectangle();
                rect4.Stroke = Brushes.Gray;
                Grid.SetRow(rect4, 3);
                Grid.SetColumn(rect4, num_of_col - 1);

                Label lb2 = AuxiliaryFunc.getNewLabel(plotter_cntrl.timeCoordinate[temp_array[f + 1]].ToString(), 30, 25, 5);
                lb2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb2, 3);
                Grid.SetColumn(lb2, num_of_col - 1);

                grid_table_print.Children.Add(rect1);
                grid_table_print.Children.Add(rect2);
                grid_table_print.Children.Add(rect3);
                grid_table_print.Children.Add(rect4);
                grid_table_print.Children.Add(lb0);
                grid_table_print.Children.Add(lb);
                grid_table_print.Children.Add(lb1);
                grid_table_print.Children.Add(lb2);
            }
            stack_panel_1.Children.Add(grid_table_print);
            margin = stack_panel_1.Margin;
            margin.Bottom = 10;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            stack_panel_1.Margin = margin;

            Grid.SetRow(stack_panel_1, 3);
            Grid.SetColumn(stack_panel_1, 0);

            Button button_1 = AuxiliaryFunc.getNewButton("Повторить оцифровку в ручном режиме", 300, 25);
            button_1.IsEnabled = false;
            button_1.Click += new RoutedEventHandler(digitization);
            AuxiliaryFunc.setMargin((Control)button_1, 25, 10, 0, 10);

            Button button_2 = AuxiliaryFunc.getNewButton("Печать", 200, 25);
            button_2.Click += new RoutedEventHandler(printing_2);
            AuxiliaryFunc.setMargin((Control)button_2, 25, 10, 0, 10);

            Button button_3 = AuxiliaryFunc.getNewButton("Автоматический расчет", 200, 25);
            button_3.Click += new RoutedEventHandler(auto_calculate);
            AuxiliaryFunc.setMargin((Control)button_3, 25, 10, 0, 10);

            stack_panel_2.Children.Add(button_1);
            stack_panel_2.Children.Add(button_2);
            stack_panel_2.Children.Add(button_3);
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Grid.SetRow(stack_panel_2, 4);
            Grid.SetColumn(stack_panel_2, 0);

            grid_main.Children.Add(header);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(template_shift);
            grid_main.Children.Add(stack_panel_1);
            grid_main.Children.Add(stack_panel_2);

            return grid_main;
        }

        private void auto_calculate(object sender, RoutedEventArgs e)
        {
            evtT1.OnSomeEvent();
        }

        public Grid auto_calculate(ProgramSettings program_settings_in)
        {            
            program_settings = program_settings_in;
            /// Автоматический режим 
            Grid grid_main = new Grid();
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            RowDefinition row_def_3 = new RowDefinition();
            RowDefinition row_def_4 = new RowDefinition();
            row_def_1.Height = new GridLength(80);
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
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stack_panel_2.Orientation = Orientation.Horizontal;

            TextBlock header = new TextBlock();
            header.Inlines.Add(new Bold(new Run(" Расчеты\n\n ")));
            header.Inlines.Add(new Run(" Автоматический режим\n\n "));
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
            Grid.SetRow(plotter_cntrl.plotter, 1);
            Grid.SetColumn(plotter_cntrl.plotter, 0);
            plotter_cntrl.plotter.Legend.Remove();
            plotter_cntrl.plotter.FitToView();

            int[] temp_array = calculation_op.time_force_array;

            grid_table_print_2 = new Grid();
            //grid_table.ShowGridLines = true;
            RowDefinition row_1 = new RowDefinition();
            row_1.Height = new GridLength(40);
            ColumnDefinition col_1 = new ColumnDefinition();
            ColumnDefinition col_2 = new ColumnDefinition();
            ColumnDefinition col_3 = new ColumnDefinition();
            ColumnDefinition col_4 = new ColumnDefinition();
            ColumnDefinition col_5 = new ColumnDefinition();
            ColumnDefinition col_6 = new ColumnDefinition();
            ColumnDefinition col_7 = new ColumnDefinition();
            ColumnDefinition col_8 = new ColumnDefinition();
            ColumnDefinition col_9 = new ColumnDefinition();
            ColumnDefinition col_10 = new ColumnDefinition();

            grid_table_print_2.RowDefinitions.Add(row_1);
            grid_table_print_2.ColumnDefinitions.Add(col_1);
            grid_table_print_2.ColumnDefinitions.Add(col_2);
            grid_table_print_2.ColumnDefinitions.Add(col_3);
            grid_table_print_2.ColumnDefinitions.Add(col_4);
            grid_table_print_2.ColumnDefinitions.Add(col_5);
            grid_table_print_2.ColumnDefinitions.Add(col_6);
            grid_table_print_2.ColumnDefinitions.Add(col_7);
            grid_table_print_2.ColumnDefinitions.Add(col_8);
            grid_table_print_2.ColumnDefinitions.Add(col_9);
            grid_table_print_2.ColumnDefinitions.Add(col_10);

            Rectangle rect = new Rectangle();
            rect.Stroke = Brushes.Black;
            Grid.SetRow(rect, 0);
            Grid.SetColumn(rect, 0);

            Label lb_ = AuxiliaryFunc.getNewLabel("## цикла", 30, 25, 2);
            Grid.SetRow(lb_, 0);
            Grid.SetColumn(lb_, 0);

            Rectangle rect0 = new Rectangle();
            rect0.Stroke = Brushes.Black;
            Grid.SetRow(rect0, 0);
            Grid.SetColumn(rect0, 1);
            Label lb0_ = AuxiliaryFunc.getNewLabel("F2 (Н)", 30, 25, 2);

            lb0_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb0_.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            lb_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb0_, 0);
            Grid.SetColumn(lb0_, 1);

            Rectangle rect01 = new Rectangle();
            rect01.Stroke = Brushes.Black;
            Grid.SetRow(rect01, 0);
            Grid.SetColumn(rect01, 2);

            Label lb1_ = AuxiliaryFunc.getNewLabel("t1 (с)", 30, 25, 2);
            lb1_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb1_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb1_, 0);
            Grid.SetColumn(lb1_, 2);

            Rectangle rect02 = new Rectangle();
            rect02.Stroke = Brushes.Black;
            Grid.SetRow(rect02, 0);
            Grid.SetColumn(rect02, 3);

            Label lb2_ = AuxiliaryFunc.getNewLabel("t2 (с)", 30, 25, 2);
            lb2_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb2_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb2_, 0);
            Grid.SetColumn(lb2_, 3);

            Rectangle rect03 = new Rectangle();
            rect03.Stroke = Brushes.Black;
            Grid.SetRow(rect03, 0);
            Grid.SetColumn(rect03, 4);

            Label lb3_ = AuxiliaryFunc.getNewLabel("\x0394t(с)", 30, 25, 2);
            lb3_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb3_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb3_, 0);
            Grid.SetColumn(lb3_, 4);

            Rectangle rect04 = new Rectangle();
            rect04.Stroke = Brushes.Black;
            Grid.SetRow(rect04, 0);
            Grid.SetColumn(rect04, 5);

            Label lb4_ = AuxiliaryFunc.getNewLabel("G2 (Н/с)", 30, 25, 2);
            lb4_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb4_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb4_, 0);
            Grid.SetColumn(lb4_, 5);

            Rectangle rect05 = new Rectangle();
            rect05.Stroke = Brushes.Black;
            Grid.SetRow(rect05, 0);
            Grid.SetColumn(rect05, 6);

            Label lb5_ = AuxiliaryFunc.getNewLabel("\x222BF(t)dt (Н \x02D9 с)", 30, 25, 2);
            lb5_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb5_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb5_, 0);
            Grid.SetColumn(lb5_, 6);

            Rectangle rect06 = new Rectangle();
            rect06.Stroke = Brushes.Black;
            Grid.SetRow(rect06, 0);
            Grid.SetColumn(rect06, 7);

            Label lb6_ = AuxiliaryFunc.getNewLabel("\x0394F (Н)", 30, 25, 2);
            lb6_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb6_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb6_, 0);
            Grid.SetColumn(lb6_, 7);

            Rectangle rect07 = new Rectangle();
            rect07.Stroke = Brushes.Black;
            Grid.SetRow(rect07, 0);
            Grid.SetColumn(rect07, 8);

            Label lb7_ = AuxiliaryFunc.getNewLabel("\x0394G (Н/с)", 30, 25, 2);
            lb7_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb7_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb7_, 0);
            Grid.SetColumn(lb7_, 8);

            Rectangle rect08 = new Rectangle();
            rect08.Stroke = Brushes.Black;
            Grid.SetRow(rect08, 0);
            Grid.SetColumn(rect08, 9);

            Label lb8_ = AuxiliaryFunc.getNewLabel("\x0394\x222BF(t)dt (Н \x02D9 с)", 30, 25, 2);
            lb8_.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lb8_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(lb8_, 0);
            Grid.SetColumn(lb8_, 9);

            grid_table_print_2.Children.Add(rect);
            grid_table_print_2.Children.Add(rect0);
            grid_table_print_2.Children.Add(rect01);
            grid_table_print_2.Children.Add(rect02);
            grid_table_print_2.Children.Add(rect03);
            grid_table_print_2.Children.Add(rect04);
            grid_table_print_2.Children.Add(rect05);
            grid_table_print_2.Children.Add(rect06);
            grid_table_print_2.Children.Add(rect07);
            grid_table_print_2.Children.Add(rect08);

            grid_table_print_2.Children.Add(lb0_);
            grid_table_print_2.Children.Add(lb_);
            grid_table_print_2.Children.Add(lb1_);
            grid_table_print_2.Children.Add(lb2_);
            grid_table_print_2.Children.Add(lb3_);
            grid_table_print_2.Children.Add(lb4_);
            grid_table_print_2.Children.Add(lb5_);
            grid_table_print_2.Children.Add(lb6_);
            grid_table_print_2.Children.Add(lb7_);
            grid_table_print_2.Children.Add(lb8_);

            int num_of_rows = 1;

            for (int f = 1; f < 300; f = f + 2)
            {
                if (temp_array[f] == 0)
                    break;
                num_of_rows++;

                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(40);
                grid_table_print_2.RowDefinitions.Add(row);

                // номер строки в таблице
                Rectangle rect_1 = new Rectangle();
                rect_1.Stroke = Brushes.Gray;
                Grid.SetRow(rect_1, num_of_rows - 1);
                Grid.SetColumn(rect_1, 0);

                Label lb0 = AuxiliaryFunc.getNewLabel((num_of_rows - 1).ToString(), 30, 25, 5);
                lb0.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb0.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb0, num_of_rows - 1);
                Grid.SetColumn(lb0, 0);
                // Максимальная приложенная сила
                Rectangle rect_2 = new Rectangle();
                rect_2.Stroke = Brushes.Gray;
                Grid.SetRow(rect_2, num_of_rows - 1);
                Grid.SetColumn(rect_2, 1);

                Label lb = AuxiliaryFunc.getNewLabel(plotter_cntrl.channel_2[temp_array[f + 1]].ToString(), 30, 25, 5);
                lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb, num_of_rows - 1);
                Grid.SetColumn(lb, 1);
                //t1

                Rectangle rect_3 = new Rectangle();
                rect_3.Stroke = Brushes.Gray;
                Grid.SetRow(rect_3, num_of_rows - 1);
                Grid.SetColumn(rect_3, 2);

                Label lb1 = AuxiliaryFunc.getNewLabel(plotter_cntrl.timeCoordinate[temp_array[f]].ToString(), 30, 25, 5);
                lb1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb1, num_of_rows - 1);
                Grid.SetColumn(lb1, 2);
                //t2

                Rectangle rect_4 = new Rectangle();
                rect_4.Stroke = Brushes.Gray;
                Grid.SetRow(rect_4, num_of_rows - 1);
                Grid.SetColumn(rect_4, 3);

                Label lb2 = AuxiliaryFunc.getNewLabel(plotter_cntrl.timeCoordinate[temp_array[f + 1]].ToString(), 30, 25, 5);
                lb2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb2, num_of_rows - 1);
                Grid.SetColumn(lb2, 3);

                ///////////
                float t_ = program_settings.time_incr;
                float F_2 = plotter_cntrl.channel_2[temp_array[f + 1]];
                float delta_t = (plotter_cntrl.timeCoordinate[temp_array[f + 1]] - plotter_cntrl.timeCoordinate[temp_array[f]]);
                float G2 = F_2 / delta_t;
                float F = program_settings.f_max;

                float integral_F_2 = 0;
                for (int l = temp_array[f]; l < temp_array[f + 1]; l++)
                {
                    integral_F_2 += plotter_cntrl.channel_2[l];
                }

                float delta_F = Math.Abs(F - F_2);
                float G = F / t_;
                float delta_G = G2 - G;
                float integral_F = F * t_ / 2;
                float delta_integral = Math.Abs(integral_F_2 - integral_F);
                ///////////
                //delta_t
                Rectangle rect_5 = new Rectangle();
                rect_5.Stroke = Brushes.Gray;
                Grid.SetRow(rect_5, num_of_rows - 1);
                Grid.SetColumn(rect_5, 4);

                Label lb3 = AuxiliaryFunc.getNewLabel(delta_t.ToString(), 30, 25, 5);
                lb3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb3.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb3, num_of_rows - 1);
                Grid.SetColumn(lb3, 4);

                // G2
                Rectangle rect_6 = new Rectangle();
                rect_6.Stroke = Brushes.Gray;
                Grid.SetRow(rect_6, num_of_rows - 1);
                Grid.SetColumn(rect_6, 5);

                Label lb4 = AuxiliaryFunc.getNewLabel(G2.ToString(), 30, 25, 5);
                lb4.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb4.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb4, num_of_rows - 1);
                Grid.SetColumn(lb4, 5);

                // integral_F
                Rectangle rect_7 = new Rectangle();
                rect_7.Stroke = Brushes.Gray;
                Grid.SetRow(rect_7, num_of_rows - 1);
                Grid.SetColumn(rect_7, 6);

                Label lb5 = AuxiliaryFunc.getNewLabel(integral_F_2.ToString(), 30, 25, 5);
                lb5.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb5.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb5, num_of_rows - 1);
                Grid.SetColumn(lb5, 6);

                // delta_F
                Rectangle rect_8 = new Rectangle();
                rect_8.Stroke = Brushes.Gray;
                Grid.SetRow(rect_8, num_of_rows - 1);
                Grid.SetColumn(rect_8, 7);

                Label lb6 = AuxiliaryFunc.getNewLabel(delta_F.ToString(), 30, 25, 5);
                lb6.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb6.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb6, num_of_rows - 1);
                Grid.SetColumn(lb6, 7);

                // delta_G
                Rectangle rect_9 = new Rectangle();
                rect_9.Stroke = Brushes.Gray;
                Grid.SetRow(rect_9, num_of_rows - 1);
                Grid.SetColumn(rect_9, 8);

                Label lb7 = AuxiliaryFunc.getNewLabel(delta_G.ToString(), 30, 25, 5);
                lb7.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb7.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb7, num_of_rows - 1);
                Grid.SetColumn(lb7, 8);

                // delta_integarl
                Rectangle rect_10 = new Rectangle();
                rect_10.Stroke = Brushes.Gray;
                Grid.SetRow(rect_10, num_of_rows - 1);
                Grid.SetColumn(rect_10, 9);

                Label lb8 = AuxiliaryFunc.getNewLabel(delta_integral.ToString(), 30, 25, 5);
                lb8.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                lb8.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(lb8, num_of_rows - 1);
                Grid.SetColumn(lb8, 9);

                grid_table_print_2.Children.Add(rect_1);
                grid_table_print_2.Children.Add(rect_2);
                grid_table_print_2.Children.Add(rect_3);
                grid_table_print_2.Children.Add(rect_4);
                grid_table_print_2.Children.Add(rect_5);
                grid_table_print_2.Children.Add(rect_6);
                grid_table_print_2.Children.Add(rect_7);
                grid_table_print_2.Children.Add(rect_8);
                grid_table_print_2.Children.Add(rect_9);
                grid_table_print_2.Children.Add(rect_10);
                grid_table_print_2.Children.Add(lb0);
                grid_table_print_2.Children.Add(lb);
                grid_table_print_2.Children.Add(lb1);
                grid_table_print_2.Children.Add(lb2);
                grid_table_print_2.Children.Add(lb3);
                grid_table_print_2.Children.Add(lb4);
                grid_table_print_2.Children.Add(lb5);
                grid_table_print_2.Children.Add(lb6);
                grid_table_print_2.Children.Add(lb7);
                grid_table_print_2.Children.Add(lb8);
            }
            margin = grid_table_print_2.Margin;
            margin.Bottom = 10;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            grid_table_print_2.Margin = margin;
            Grid.SetRow(grid_table_print_2, 2);
            Grid.SetColumn(grid_table_print_2, 0);

            Button button_1 = AuxiliaryFunc.getNewButton("Повторить оцифровку в ручном режиме", 300, 25);
            button_1.Click += new RoutedEventHandler(digitization);
            button_1.IsEnabled = false;
            AuxiliaryFunc.setMargin((Control)button_1, 25, 10, 0, 10);

            Button button_2 = AuxiliaryFunc.getNewButton("Печать", 200, 25); 
            button_2.Click += new RoutedEventHandler(printing_3);
            AuxiliaryFunc.setMargin((Control)button_2, 25, 10, 0, 10);

            stack_panel_2.Children.Add(button_1);
            stack_panel_2.Children.Add(button_2);
            stack_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Grid.SetRow(stack_panel_2, 3);
            Grid.SetColumn(stack_panel_2, 0);

            grid_main.Children.Add(grid_table_print_2);
            grid_main.Children.Add(header);
            grid_main.Children.Add(plotter_cntrl.plotter);
            grid_main.Children.Add(stack_panel_1);
            grid_main.Children.Add(stack_panel_2);           
            plotter_cntrl.plot_measured_data();
            digitize_measurement();

            return grid_main;
        }

        // Выбор текущего эксперимента для обработки
        private void get_current_experiment(object sender, RoutedEventArgs e)
        {
            num_of_exp_text.Text = (tdb.get_max_idx() + 1).ToString();
            name_of_exp_text.Text = "measurement";
            plotter_cntrl.plot_measured_data();
        }

        private void get_from_db(object sender, RoutedEventArgs e)
        {
            DataRowView drw = (DataRowView)list_of_data.SelectedValue;
            DataRow dr = drw.Row;
            String path_str = tdb.query_get_path(dr[1].ToString());

            plotter_cntrl.clear_Buffer();
            excelData.load_from_excel_file(path_str, PlotterControl.BUFFER_SIZE, out plotter_cntrl.channel_2, out plotter_cntrl.timeCoordinate, out plotter_cntrl.count_channel_2);
            
            num_of_exp_text.Text = (dr[1].ToString()).ToString();
            name_of_exp_text.Text = (dr[0].ToString()).ToString(); 
            download_data.IsExpanded = false;
            plotter_cntrl.plot_measured_data();
        }

        private void query_data()
        {
            list_of_data.DataContext = tdb.query_data();
            Binding bind = new Binding();
            list_of_data.SetBinding(ListView.ItemsSourceProperty, bind);
        }


        private void printing_1(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void printing_3(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.plotter.SaveScreenshot(Directory.GetCurrentDirectory() + "\\file.jpg");
            print_obj.printing_3(grid_table_print_2);
        }

        private void printing_2(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.plotter.SaveScreenshot(Directory.GetCurrentDirectory() + "\\file.jpg");
            print_obj.printing_2(grid_table_print);
        }

        void ch_template_Unchecked(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.ch_template = false;
            pattern = temp_template;
            if (ch_1 != null)
                ch_1.IsChecked = false;
            plotter_cntrl.delete_template();
        }

        void ch_template_Checked(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.ch_template = true;
            temp_template = pattern;
            if (ch_1 != null)
                ch_1.IsChecked = true;
            plotter_cntrl.plot_template();
        }

        private int[] digitize_measurement()
        {
            //short[] temp_array = calculation_op.get_max_min(count_channel_2,  channel_2);
            int[] temp_array = calculation_op.get_points(plotter_cntrl.count_channel_2, /*channel_2*/calculation_op.get_smoothed_data(plotter_cntrl.count_channel_2, plotter_cntrl.channel_2));
            int[] temp_array_1 = calculation_op.get_smoothed_data(plotter_cntrl.count_channel_2, plotter_cntrl.channel_2);
            int[] temp_array_2 = calculation_op.get_derivative(plotter_cntrl.count_channel_2, /*channel_2*/calculation_op.get_smoothed_data(plotter_cntrl.count_channel_2, plotter_cntrl.channel_2));

            plotter_cntrl.source1 = new ObservableDataSource<Point>();
            plotter_cntrl.source1.SetXYMapping(p => p);
            plotter_cntrl.plotter.AddLineGraph(plotter_cntrl.source1,
             new Pen(Brushes.DarkGoldenrod, 0),
             new Microsoft.Research.DynamicDataDisplay.PointMarkers.CirclePointMarker { Size = 10, Fill = Brushes.Red },
             new PenDescription("Sin"));
            // plotter.AddLineGraph(source1, 1, "Шаблон");
            plotter_cntrl.source2 = new ObservableDataSource<Point>();
            plotter_cntrl.source2.SetXYMapping(p => p);
            plotter_cntrl.source3 = new ObservableDataSource<Point>();
            plotter_cntrl.source3.SetXYMapping(p => p);
            plotter_cntrl.plotter.AddLineGraph(plotter_cntrl.source2, 1, "Шаблон");
            plotter_cntrl.plotter.AddLineGraph(plotter_cntrl.source3, 1, "Шаблон");
            double x_1 = 0;
            double y_1 = 0;
            for (int i = 1; i < temp_array.Length; i++)
            {
                {
                    if (temp_array[i] != 0)
                    {
                        x_1 = (float)plotter_cntrl.timeCoordinate[temp_array[i]];
                        y_1 = (float)plotter_cntrl.channel_2[temp_array[i]];
                        Point p1 = new Point(x_1, y_1);
                        plotter_cntrl.source1.AppendAsync(Dispatcher, p1);
                    }
                }
            }
            for (int i = 1; i < plotter_cntrl.count_channel_2; i++)
            {
                {
                    x_1 = (float)plotter_cntrl.timeCoordinate[i];
                    y_1 = (float)temp_array_2[i];
                    Point p1 = new Point(x_1, y_1);
                    plotter_cntrl.source2.AppendAsync(Dispatcher, p1);
                }
            }
            for (int i = 1; i < plotter_cntrl.count_channel_2; i++)
            {
                {

                    x_1 = (float)plotter_cntrl.timeCoordinate[i];
                    y_1 = (float)temp_array_1[i];
                    Point p1 = new Point(x_1, y_1);
                    plotter_cntrl.source3.AppendAsync(Dispatcher, p1);
                }
            }
            return temp_array;
        }
    }
}
