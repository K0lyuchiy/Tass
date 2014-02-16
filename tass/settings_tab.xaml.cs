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
using System.Data;

namespace tass
{
    /// <summary>
    /// Логика взаимодействия для settings_tab.xaml
    /// </summary>
    public partial class settings_tab : Page
    {
        public MarkerEvent1 evt_cancel_settings_tab;
        public MarkerEvent1 evt_apply_settings;

        tassDB tdb;
        ProgramSettings program_settings; 
        Template pattern;

        TextBox template_name_text;
        TextBox force_max_text;
        CheckBox allow_auto_check;
        CheckBox allow_digit_check;
        TextBox attempts_num;
        TextBox time_value;
        TextBox time_pause_text;
        Grid base_grid;
        ComboBox color_list_box;
        ComboBox widths_list_box;
        ComboBox color_list_box_1;
        ComboBox widths_list_box_1;
        ComboBox color_list_box_2;
        ComboBox widths_list_box_2;
        ComboBox color_list_box_3;
        ComboBox widths_list_box_3;

        ListView list_of_templates;

        public settings_tab()
        {
            evt_cancel_settings_tab = new MarkerEvent1();
            evt_apply_settings = new MarkerEvent1();
            program_settings = new ProgramSettings();
            pattern = new Template();
            tdb = new tassDB();
            InitializeComponent();
        }

        public Grid drawSetings(ref ProgramSettings program_settings_in)
        { 
            base_grid = new Grid();
            //base_grid.ShowGridLines = true;
            RowDefinition row_def_1_ = new RowDefinition();
            RowDefinition row_def_2_ = new RowDefinition();
            RowDefinition row_def_3_ = new RowDefinition();
            row_def_3_.Height = new GridLength(80);
            ColumnDefinition col_def_1_ = new ColumnDefinition();
            col_def_1_.Width = new GridLength(600);
            ColumnDefinition col_def_2_ = new ColumnDefinition();
            //col_def_1_.Width = System.Windows.GridLength.Auto;
            //col_def_2_.Width = System.Windows.GridLength.Auto;
            base_grid.RowDefinitions.Add(row_def_1_);
            base_grid.RowDefinitions.Add(row_def_2_);
            base_grid.RowDefinitions.Add(row_def_3_);
            base_grid.ColumnDefinitions.Add(col_def_1_);
            base_grid.ColumnDefinitions.Add(col_def_2_);

            // Основные настройки
            Grid grid_1 = new Grid();
            // grid_1.ShowGridLines = true;
            RowDefinition row_def_grid_1 = new RowDefinition();
            RowDefinition row_def_grid_2 = new RowDefinition();
            RowDefinition row_def_grid_3 = new RowDefinition();
            RowDefinition row_def_grid_4 = new RowDefinition();
            RowDefinition row_def_grid_5 = new RowDefinition();
            RowDefinition row_def_grid_6 = new RowDefinition();
            RowDefinition row_def_grid_7 = new RowDefinition();
            RowDefinition row_def_grid_8 = new RowDefinition();

            ColumnDefinition col_def_grid_1_1 = new ColumnDefinition();
            ColumnDefinition col_def_grid_1_2 = new ColumnDefinition();
            col_def_grid_1_1.Width = System.Windows.GridLength.Auto;
            col_def_grid_1_2.Width = System.Windows.GridLength.Auto;

            grid_1.RowDefinitions.Add(row_def_grid_1);
            grid_1.RowDefinitions.Add(row_def_grid_2);
            grid_1.RowDefinitions.Add(row_def_grid_3);
            grid_1.RowDefinitions.Add(row_def_grid_4);
            grid_1.RowDefinitions.Add(row_def_grid_5);
            grid_1.RowDefinitions.Add(row_def_grid_6);
            grid_1.RowDefinitions.Add(row_def_grid_7);
            grid_1.RowDefinitions.Add(row_def_grid_8);

            grid_1.ColumnDefinitions.Add(col_def_grid_1_1);
            grid_1.ColumnDefinitions.Add(col_def_grid_1_2);

            Thickness margin = grid_1.Margin;
            margin.Bottom = 5;
            margin.Left = 10;
            margin.Top = 20;
            margin.Right = 5;
            grid_1.Margin = margin;

            Grid.SetRow(grid_1, 0);
            Grid.SetColumn(grid_1, 0);

            Label force_max_label = AuxiliaryFunc.getNewLabel("Максимальное значение прикладываемой силы Fmax, [Н.]", 300, 25, 0);
            force_max_label.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(force_max_label, 0);
            Grid.SetColumn(force_max_label, 0);

            force_max_text = new TextBox();
            force_max_text = (TextBox)AuxiliaryFunc.setNewControl("force_max", (Control)force_max_text, 200, 25);
            force_max_text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            force_max_text.TextAlignment = System.Windows.TextAlignment.Right;
            AuxiliaryFunc.setMargin(force_max_text, 0, 25, 5, 0);
            Grid.SetRow(force_max_text, 0);
            Grid.SetColumn(force_max_text, 1);

            Label time_increase = AuxiliaryFunc.getNewLabel("Время развития усилия t, [c.]", 300, 25, 0);
            time_increase.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(time_increase, 1);
            Grid.SetColumn(time_increase, 0);

            time_value = new TextBox();
            time_value = ((TextBox)AuxiliaryFunc.setNewControl("time_value", (Control)time_value, 200, 25));
            time_value.TextAlignment = System.Windows.TextAlignment.Right;
            AuxiliaryFunc.setMargin(time_value, 0, 25, 5, 0);
            time_value.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetRow(time_value, 1);
            Grid.SetColumn(time_value, 1);

            Label time_pause = AuxiliaryFunc.getNewLabel("Интервал между попытками t, [c.]", 200, 25, 0);
            time_pause.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(time_pause, 2);
            Grid.SetColumn(time_pause, 0);

            time_pause_text = new TextBox();
            time_pause_text = (TextBox)AuxiliaryFunc.setNewControl("time_pause_text", (Control)time_pause_text, 200, 25);
            time_pause_text.TextAlignment = System.Windows.TextAlignment.Right;

            time_pause_text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            AuxiliaryFunc.setMargin(time_pause_text, 0, 25, 5, 0);
            Grid.SetRow(time_pause_text, 2);
            Grid.SetColumn(time_pause_text, 1);

            Label number_of_attempts = AuxiliaryFunc.getNewLabel("Количество попыток", 200, 25, 0);

            number_of_attempts.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(number_of_attempts, 3);
            Grid.SetColumn(number_of_attempts, 0);

            attempts_num = new TextBox();
            attempts_num = (TextBox)AuxiliaryFunc.setNewControl("attempts_num", (Control)attempts_num, 200, 25);
            attempts_num.TextAlignment = System.Windows.TextAlignment.Right;
            attempts_num.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            AuxiliaryFunc.setMargin(attempts_num, 0, 25, 5, 0);
            Grid.SetRow(attempts_num, 3);
            Grid.SetColumn(attempts_num, 1);

            Label allow_digit = AuxiliaryFunc.getNewLabel("Разрешить оцифровку", 200, 25, 0);
            allow_digit.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetRow(allow_digit, 4);
            Grid.SetColumn(allow_digit, 0);

            allow_digit_check = new CheckBox();
            allow_digit_check = (CheckBox)AuxiliaryFunc.setNewControl("allow_digit_check", (Control)allow_digit_check, 100, 25);
            allow_digit_check.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            allow_digit_check.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            allow_digit_check.Checked += allow_digit_check_Checked;
            AuxiliaryFunc.setMargin(allow_digit_check, 0, 125, 0, 0);
            Grid.SetRow(allow_digit_check, 4);
            Grid.SetColumn(allow_digit_check, 1);

            Label allow_auto_calculate = AuxiliaryFunc.getNewLabel("Разрешить автоматические расчеты", 200, 25, 0);
            allow_auto_calculate.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetRow(allow_auto_calculate, 5);
            Grid.SetColumn(allow_auto_calculate, 0);

            allow_auto_check = new CheckBox();
            allow_auto_check.Checked += allow_auto_check_Checked;
            allow_auto_check.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            allow_auto_check.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            AuxiliaryFunc.setMargin(allow_auto_check, 0, 125, 0, 0);
            allow_auto_check = (CheckBox)AuxiliaryFunc.setNewControl("allow_auto_check", (Control)allow_auto_check, 100, 25);
            Grid.SetRow(allow_auto_check, 5);
            Grid.SetColumn(allow_auto_check, 1);

            template_name_text = new TextBox();
            template_name_text = (TextBox)AuxiliaryFunc.setNewControl("template_name_text", (Control)template_name_text, 200, 25);
            template_name_text.TextAlignment = System.Windows.TextAlignment.Right;
            AuxiliaryFunc.setMargin(template_name_text, 0, 25, 5, 0);
            template_name_text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(template_name_text, 6);
            Grid.SetColumn(template_name_text, 1);

            StackPanel stack_p = new StackPanel();

            Label template_name_lb = AuxiliaryFunc.getNewLabel("Имя шаблона", 200, 25, 0);
            template_name_lb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetRow(template_name_lb, 6);
            Grid.SetColumn(template_name_lb, 0); 

            grid_1.Children.Add(force_max_label);
            grid_1.Children.Add(force_max_text);
            grid_1.Children.Add(time_increase);
            grid_1.Children.Add(time_value);
            grid_1.Children.Add(number_of_attempts);
            grid_1.Children.Add(time_pause);
            grid_1.Children.Add(time_pause_text);
            grid_1.Children.Add(attempts_num);
            grid_1.Children.Add(allow_digit);
            grid_1.Children.Add(allow_digit_check);
            grid_1.Children.Add(allow_auto_calculate);
            grid_1.Children.Add(allow_auto_check);
            grid_1.Children.Add(template_name_lb);
            grid_1.Children.Add(template_name_text);

            // заполнение полей настроек
            force_max_text.Text = program_settings_in.f_max.ToString();
            attempts_num.Text = program_settings_in.num_of_attempts.ToString();
            allow_digit_check.IsChecked = program_settings_in.digitization;
            allow_auto_check.IsChecked = program_settings_in.calculation;
            time_value.Text = program_settings_in.time_incr.ToString();
            time_pause_text.Text = program_settings_in.t_pause.ToString();

            //////////////////////// вспомогательные настройки
            StackPanel expander_panel = new StackPanel();
            expander_panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            expander_panel.Background = Brushes.Bisque;

            Grid grid_2 = new Grid();
            margin = grid_2.Margin;
            margin.Bottom = 5;
            margin.Left = 10;
            margin.Top = 20;
            margin.Right = 5;
            grid_2.Margin = margin;
            //grid_2.ShowGridLines = true;
            RowDefinition row_def_grid_2_1 = new RowDefinition();
            RowDefinition row_def_grid_2_2 = new RowDefinition();
            RowDefinition row_def_grid_2_3 = new RowDefinition();
            RowDefinition row_def_grid_2_4 = new RowDefinition();
            RowDefinition row_def_grid_2_5 = new RowDefinition();
            RowDefinition row_def_grid_2_6 = new RowDefinition();
            RowDefinition row_def_grid_2_7 = new RowDefinition();
            RowDefinition row_def_grid_2_8 = new RowDefinition();
            ColumnDefinition col_def_grid_2_1 = new ColumnDefinition();
            ColumnDefinition col_def_grid_2_2 = new ColumnDefinition();
            ColumnDefinition col_def_grid_2_3 = new ColumnDefinition();
            col_def_grid_2_1.Width = System.Windows.GridLength.Auto;
            col_def_grid_2_2.Width = System.Windows.GridLength.Auto;
            col_def_grid_2_3.Width = System.Windows.GridLength.Auto;

            grid_2.RowDefinitions.Add(row_def_grid_2_1);
            grid_2.RowDefinitions.Add(row_def_grid_2_2);
            grid_2.RowDefinitions.Add(row_def_grid_2_3);
            grid_2.RowDefinitions.Add(row_def_grid_2_4);
            grid_2.RowDefinitions.Add(row_def_grid_2_5);
            grid_2.RowDefinitions.Add(row_def_grid_2_6);
            grid_2.RowDefinitions.Add(row_def_grid_2_7);
            grid_2.RowDefinitions.Add(row_def_grid_2_8);
            grid_2.ColumnDefinitions.Add(col_def_grid_2_1);
            grid_2.ColumnDefinitions.Add(col_def_grid_2_2);
            grid_2.ColumnDefinitions.Add(col_def_grid_2_3);

            Label template_label = AuxiliaryFunc.getNewLabel("Шаблон", 175, 25, 5);

            template_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(template_label, 0);
            Grid.SetColumn(template_label, 0);

            Label template_color_label = AuxiliaryFunc.getNewLabel("цвет", 150, 25, 5);

            template_color_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(template_color_label, 0);
            Grid.SetColumn(template_color_label, 1);


            color_list_box = AuxiliaryFunc.get_color_combo_box(); 
            AuxiliaryFunc.setMargin(color_list_box, 0, 0, 5, 0);
            color_list_box.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            color_list_box.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            color_list_box.SelectedIndex = program_settings_in.template_color_num;

            Grid.SetRow(color_list_box, 0);
            Grid.SetColumn(color_list_box, 2);

            Label template_width_label = AuxiliaryFunc.getNewLabel("толщина линии", 100, 25, 5);
            template_width_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(template_width_label, 1);
            Grid.SetColumn(template_width_label, 1);

            widths_list_box = AuxiliaryFunc.get_line_width_combo_box();

            widths_list_box.SelectedIndex = program_settings_in.template_line_width;


            AuxiliaryFunc.setMargin(widths_list_box, 0, 0, 5, 0);
            color_list_box.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            color_list_box.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            Grid.SetRow(widths_list_box, 1);
            Grid.SetColumn(widths_list_box, 2);


            Label curve_label = AuxiliaryFunc.getNewLabel("Кривая", 100, 25, 5);
            curve_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(curve_label, 2);
            Grid.SetColumn(curve_label, 0);

            Label curve_color_label = AuxiliaryFunc.getNewLabel("цвет", 100, 25, 5);
            curve_color_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(curve_color_label, 2);
            Grid.SetColumn(curve_color_label, 1);

            color_list_box_1 = AuxiliaryFunc.get_color_combo_box(); 
            AuxiliaryFunc.setMargin(color_list_box_1, 0, 0, 5, 0);
            color_list_box_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            color_list_box_1.SelectedIndex = program_settings_in.measurement_color_num;

            Grid.SetRow(color_list_box_1, 2);
            Grid.SetColumn(color_list_box_1, 2);

            Label curve_width_label = AuxiliaryFunc.getNewLabel("толщина линии", 100, 25, 5);

            curve_width_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            Grid.SetRow(curve_width_label, 3);
            Grid.SetColumn(curve_width_label, 1);

            widths_list_box_1 = AuxiliaryFunc.get_line_width_combo_box(); 
            AuxiliaryFunc.setMargin(widths_list_box_1, 0, 0, 5, 0);
            widths_list_box_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            widths_list_box_1.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            widths_list_box_1.SelectedIndex = program_settings_in.measurement_line_width;

            Grid.SetRow(widths_list_box_1, 3);
            Grid.SetColumn(widths_list_box_1, 2);


            Label grid_label = AuxiliaryFunc.getNewLabel("Координатная сетка", 100, 25, 5);
            grid_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(grid_label, 4);
            Grid.SetColumn(grid_label, 0);

            Label grid_color_label = AuxiliaryFunc.getNewLabel("цвет", 100, 25, 5);

            grid_color_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(grid_color_label, 4);
            Grid.SetColumn(grid_color_label, 1);

            color_list_box_2 = AuxiliaryFunc.get_color_combo_box();

            color_list_box_2.SelectedIndex = program_settings_in.grid_color_num;


            AuxiliaryFunc.setMargin(color_list_box_2, 0, 0, 5, 0);
            color_list_box_2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            Grid.SetRow(color_list_box_2, 4);
            Grid.SetColumn(color_list_box_2, 2);

            Label grid_width_label = AuxiliaryFunc.getNewLabel("толщина линий", 100, 25, 5);

            grid_width_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(grid_width_label, 5);
            Grid.SetColumn(grid_width_label, 1);

            widths_list_box_2 = AuxiliaryFunc.get_line_width_combo_box(); 
            AuxiliaryFunc.setMargin(widths_list_box_2, 0, 0, 5, 0);
            widths_list_box_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            widths_list_box_2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            widths_list_box_2.SelectedIndex = program_settings_in.grid_line_width;

            Grid.SetRow(widths_list_box_2, 5);
            Grid.SetColumn(widths_list_box_2, 2);

            Label axis_label = AuxiliaryFunc.getNewLabel("Оси координат", 100, 25, 5);
            axis_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(axis_label, 6);
            Grid.SetColumn(axis_label, 0);

            Label axis_color_label = AuxiliaryFunc.getNewLabel("цвет", 100, 25, 5);
            axis_color_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(axis_color_label, 6);
            Grid.SetColumn(axis_color_label, 1);

            color_list_box_3 = AuxiliaryFunc.get_color_combo_box(); 
            AuxiliaryFunc.setMargin(color_list_box_3, 0, 0, 5, 0);
            color_list_box_3.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            color_list_box_3.SelectedIndex = program_settings_in.axis_color_num;

            Grid.SetRow(color_list_box_3, 6);
            Grid.SetColumn(color_list_box_3, 2);

            Label axis_width_label = AuxiliaryFunc.getNewLabel("толщина линий", 100, 25, 5);
            axis_width_label.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            Grid.SetRow(axis_width_label, 7);
            Grid.SetColumn(axis_width_label, 1);

            widths_list_box_3 = AuxiliaryFunc.get_line_width_combo_box();
            AuxiliaryFunc.setMargin(widths_list_box_3, 0, 0, 5, 0);
            widths_list_box_3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            widths_list_box_3.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            widths_list_box_3.SelectedIndex = program_settings_in.axis_line_width;

            Grid.SetRow(widths_list_box_3, 7);
            Grid.SetColumn(widths_list_box_3, 2);

            StackPanel button_panel_1 = new StackPanel();
            button_panel_1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_panel_1.Orientation = Orientation.Horizontal;
            Button save_settings_button = AuxiliaryFunc.getNewButton("save", 200, 25);
            save_settings_button.Content = "Сохранить шаблон";

            AuxiliaryFunc.setMargin((Control)save_settings_button, 25, 10, 0, 10);
            save_settings_button.Click += new RoutedEventHandler(save_settings);
            button_panel_1.Children.Add(save_settings_button);
            Grid.SetRow(button_panel_1, 2);
            Grid.SetColumn(button_panel_1, 0);


            StackPanel button_panel_2 = new StackPanel();
            button_panel_2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button_panel_2.Orientation = Orientation.Horizontal;
            Grid.SetRow(button_panel_2, 2);
            Grid.SetColumn(button_panel_2, 1);

            Button apply_settings_button = AuxiliaryFunc.getNewButton("apply", 200, 25);
            apply_settings_button.Content = "Применить";
            AuxiliaryFunc.setMargin((Control)apply_settings_button, 25, 10, 0, 10);
            apply_settings_button.Click += new RoutedEventHandler(apply_settings);

            Button cancel_settings_button = AuxiliaryFunc.getNewButton("cancel", 200, 25);
            cancel_settings_button.Content = "Выход";
            AuxiliaryFunc.setMargin((Control)cancel_settings_button, 25, 10, 0, 10);
            cancel_settings_button.Click += new RoutedEventHandler(cancel_settings);

            button_panel_2.Children.Add(apply_settings_button);
            button_panel_2.Children.Add(cancel_settings_button);
            
            GroupBox gb_1 = new GroupBox();
            Grid.SetRow(gb_1, 0);
            Grid.SetColumn(gb_1, 0);
            gb_1.Content = grid_1;
            gb_1.Background = Brushes.Snow;
            gb_1.Header = "Основные настройки";

            margin = gb_1.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            gb_1.Margin = margin;

            GroupBox gb_2 = new GroupBox();
            Grid.SetRow(gb_2, 1);
            Grid.SetColumn(gb_2, 0);

            gb_2.Content = grid_2;

            gb_2.Background = Brushes.Snow;
            gb_2.Header = "Вспомогательные настройки";

            margin = gb_2.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            gb_2.Margin = margin;

            grid_2.Children.Add(curve_label);
            grid_2.Children.Add(curve_color_label);
            grid_2.Children.Add(curve_width_label);
            grid_2.Children.Add(grid_label);
            grid_2.Children.Add(grid_color_label);
            grid_2.Children.Add(grid_width_label);
            grid_2.Children.Add(axis_label);
            grid_2.Children.Add(axis_color_label);
            grid_2.Children.Add(axis_width_label);
            grid_2.Children.Add(template_label);
            grid_2.Children.Add(template_color_label);

            grid_2.Children.Add(color_list_box);
            grid_2.Children.Add(color_list_box_1);
            grid_2.Children.Add(color_list_box_2);
            grid_2.Children.Add(color_list_box_3);
            grid_2.Children.Add(widths_list_box);
            grid_2.Children.Add(widths_list_box_1);
            grid_2.Children.Add(widths_list_box_2);
            grid_2.Children.Add(widths_list_box_3);

            grid_2.Children.Add(template_width_label);

            list_of_templates = new ListView();

            GridView myGridView = new GridView();

            myGridView.AllowsColumnReorder = true;
            myGridView.ColumnHeaderToolTip = "Шаблоны настроек";

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("Template_number");
            gvc1.Header = "Номер шаблона";
            gvc1.Width = 100;

            myGridView.Columns.Add(gvc1);
            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new Binding("Template_name");
            gvc2.Header = "Имя шаблона";
            gvc2.Width = 100;
            myGridView.Columns.Add(gvc2);
            GridViewColumn gvc3 = new GridViewColumn();
            gvc3.DisplayMemberBinding = new Binding("F_max");
            gvc3.Header = "Сила";
            gvc3.Width = 80;

            myGridView.Columns.Add(gvc3);
            GridViewColumn gvc4 = new GridViewColumn();
            gvc4.DisplayMemberBinding = new Binding("T_increase");
            gvc4.Header = "Время развития усилия";
            gvc4.Width = 140;
            myGridView.Columns.Add(gvc4);
            GridViewColumn gvc5 = new GridViewColumn();
            gvc5.DisplayMemberBinding = new Binding("Number_of_attempts");
            gvc5.Header = "Количество попыток";
            gvc5.Width = 130;
            myGridView.Columns.Add(gvc5);

            list_of_templates.View = myGridView;
            margin = list_of_templates.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            list_of_templates.Margin = margin;
            list_of_templates.SelectionChanged += list_of_templates_SelectionChanged;

            query_template();


            GroupBox gb_3 = new GroupBox();
            //gb_1.Content = "Основные настройки";
            Grid.SetRow(gb_3, 0);
            Grid.SetColumn(gb_3, 1);
            Grid.SetRowSpan(gb_3, 2);
            gb_3.Content = list_of_templates;
            gb_3.Background = Brushes.Snow;
            gb_3.Header = "Список шаблонов настроек";
            margin = gb_3.Margin;
            margin.Bottom = 5;
            margin.Left = 5;
            margin.Top = 5;
            margin.Right = 5;
            gb_3.Margin = margin;

            base_grid.Children.Add(gb_1);
            base_grid.Children.Add(gb_2);
            base_grid.Children.Add(button_panel_1);

            base_grid.Children.Add(gb_3);
            base_grid.Children.Add(button_panel_2);

            return base_grid;
        }

        private void query_template()
        {
            list_of_templates.DataContext = tdb.query_();
            Binding bind = new Binding();
            list_of_templates.SetBinding(ListView.ItemsSourceProperty, bind);
        }       

        private void list_of_templates_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetParametersFromSettings();
        }

        private void SetParametersFromSettings()
        {
            DataRowView drw = (DataRowView)list_of_templates.SelectedValue;
            DataRow dr = drw.Row;

            DataTable dt = tdb.query_get_raw(dr[1].ToString());
            foreach (DataRow r in dt.Rows)
            {
                force_max_text.Text = r[2].ToString();
                time_value.Text = r[3].ToString();
                time_pause_text.Text = r[4].ToString();
                attempts_num.Text = r[5].ToString();
                template_name_text.Text = r[0].ToString();

                program_settings.num_of_set_template = Int16.Parse(r[1].ToString());
                color_list_box.SelectedIndex = Int32.Parse(r[8].ToString());
                widths_list_box.SelectedIndex = Int32.Parse(r[9].ToString());
                color_list_box_1.SelectedIndex = Int32.Parse(r[10].ToString());
                widths_list_box_1.SelectedIndex = Int32.Parse(r[11].ToString());
                color_list_box_2.SelectedIndex = Int32.Parse(r[12].ToString());
                widths_list_box_2.SelectedIndex = Int32.Parse(r[13].ToString());
                color_list_box_3.SelectedIndex = Int32.Parse(r[14].ToString());
                widths_list_box_3.SelectedIndex = Int32.Parse(r[15].ToString());
            }
            program_settings.f_max = (float)Double.Parse(force_max_text.Text.ToString());
            program_settings.num_of_attempts = Int16.Parse(attempts_num.Text.ToString());
            program_settings.digitization = allow_digit_check.IsChecked.Value;
            program_settings.calculation = allow_auto_check.IsChecked.Value;
            program_settings.time_incr = (float)Double.Parse(time_value.Text.ToString());
            program_settings.t_pause = (float)Double.Parse(time_pause_text.Text.ToString());

            program_settings.template_color_num = color_list_box.SelectedIndex;
            program_settings.template_line_width = widths_list_box.SelectedIndex;
            program_settings.measurement_color_num = color_list_box_1.SelectedIndex;
            program_settings.measurement_line_width = widths_list_box_1.SelectedIndex;
            program_settings.grid_color_num = color_list_box_2.SelectedIndex;
            program_settings.grid_line_width = widths_list_box_2.SelectedIndex;
            program_settings.axis_color_num = color_list_box_3.SelectedIndex;
            program_settings.axis_line_width = widths_list_box_3.SelectedIndex;
        }        

        bool check_password()
        {
             List<string> str = new List<string>();
             var window = new VariablesWindow(str);
             window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
             window.ShowDialog();
             string pass = "";
             if (window.DialogResult == true)
                 str = new List<string>(window.Variables);
             if (str.Count != 0)
                 pass = str[0];
             else
                 ;
             if (pass == "1")
             {
                 return true;
             }
             else 
            {
                return false;
            }
        }
         
        void allow_digit_check_Checked(object sender, RoutedEventArgs e)
        {
            if (check_password())
                allow_digit_check.IsChecked = true;
            else
            {
                allow_digit_check.IsChecked = false;
                MessageBox.Show("Введен неверный пароль", "Ошибка идентификации");
            }
        }

        void allow_auto_check_Checked(object sender, RoutedEventArgs e)
        {
            if (check_password())
                allow_auto_check.IsChecked = true;
            else
            {
                allow_auto_check.IsChecked = false;
                MessageBox.Show("Введен неверный пароль", "Ошибка идентификации");
            }
        }
  
        private void save_settings(object sender, RoutedEventArgs e)
        {
            program_settings.f_max = (float)Double.Parse(force_max_text.Text.ToString());
            program_settings.num_of_attempts = Int16.Parse(attempts_num.Text.ToString());
            program_settings.digitization = allow_digit_check.IsChecked.Value;
            program_settings.calculation = allow_auto_check.IsChecked.Value;
            program_settings.time_incr = (float)Double.Parse(time_value.Text.ToString());
            program_settings.t_pause = (float)Double.Parse(time_pause_text.Text.ToString());

            program_settings.template_color_num = color_list_box.SelectedIndex;
            program_settings.template_line_width = widths_list_box.SelectedIndex;
            program_settings.measurement_color_num = color_list_box_1.SelectedIndex;
            program_settings.measurement_line_width = widths_list_box_1.SelectedIndex;
            program_settings.grid_color_num = color_list_box_2.SelectedIndex;
            program_settings.grid_line_width = widths_list_box_2.SelectedIndex;
            program_settings.axis_color_num = color_list_box_3.SelectedIndex;
            program_settings.axis_line_width = widths_list_box_3.SelectedIndex;

            if (template_name_text.Text.ToString().Length == 0)
            {
                template_name_text.Text = "template_" + (tdb.get_last_idx_of_templates() + 1).ToString();
            }
            else
                ;
            if (tdb.is_template_in_db(template_name_text.Text.ToString()))
            {
                // предложить перезаписать существующий в БД шаблон
                MessageBoxResult result = MessageBox.Show("Вы хотите перезаписать уже имеющийся в БД шаблон с таким именем?",
                "Подтверждение перезаписи", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    //  
                    replace_settings(template_name_text.Text.ToString());
                }
                else if (result == MessageBoxResult.No)
                {
                    //  
                }
                else
                {
                    //  
                }

            }
            else
            {
                insert_new_template();
            }
            query_template();
        }

        private void save_template(object sender, RoutedEventArgs e)
        {
            int idx_f_max = base_grid.Children.IndexOf(force_max_text);
            string str = ((TextBox)base_grid.Children[idx_f_max]).Text.ToString();
            int f_max = Int32.Parse(str);
            int idx_time_value = base_grid.Children.IndexOf(time_value);
            int idx_attempts_num = base_grid.Children.IndexOf(attempts_num);
            int idx_allow_digit_check = base_grid.Children.IndexOf(allow_digit_check);
            int idx_allow_auto_check = base_grid.Children.IndexOf(allow_auto_check);

            str = ((TextBox)base_grid.Children[idx_time_value]).Text.ToString();
            int time_value_ = Int32.Parse(str);
            str = ((TextBox)base_grid.Children[idx_attempts_num]).Text.ToString();
            int attempts_num_ = Int32.Parse(str);
        }

        private void apply_settings(object sender, RoutedEventArgs e)
        {
            evt_apply_settings.OnSomeEvent();
        }

        public void getProgramSettings(ref Template pattern_in, ref ProgramSettings program_settings_in)
        { 
            program_settings_in.f_max = (float)Double.Parse(force_max_text.Text.ToString());
            program_settings_in.num_of_attempts = Int16.Parse(attempts_num.Text.ToString());
            program_settings_in.digitization = allow_digit_check.IsChecked.Value;
            program_settings_in.calculation = allow_auto_check.IsChecked.Value;
            program_settings_in.time_incr = (float)Double.Parse(time_value.Text.ToString());
            program_settings_in.t_pause = (float)Double.Parse(time_pause_text.Text.ToString());

            program_settings_in.template_color_num = color_list_box.SelectedIndex;
            program_settings_in.template_line_width = widths_list_box.SelectedIndex;
            program_settings_in.measurement_color_num = color_list_box_1.SelectedIndex;
            program_settings_in.measurement_line_width = widths_list_box_1.SelectedIndex;
            program_settings_in.grid_color_num = color_list_box_2.SelectedIndex;
            program_settings_in.grid_line_width = widths_list_box_2.SelectedIndex;
            program_settings_in.axis_color_num = color_list_box_3.SelectedIndex;
            program_settings_in.axis_line_width = widths_list_box_3.SelectedIndex;

            pattern_in.A = program_settings_in.f_max;
            pattern_in.num_of_pulses = program_settings_in.num_of_attempts;
            pattern_in.t_pulse = program_settings_in.time_incr;
            pattern_in.t_zero = program_settings_in.t_pause;

            tdb.replace_set(program_settings_in.num_of_set_template.ToString());
        }

        private void cancel_settings(object sender, RoutedEventArgs e)
        {
            evt_cancel_settings_tab.OnSomeEvent();  
        }

        private void replace_settings(string name_templ)
        {
            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("f_max", force_max_text.Text);
            data.Add("template_name", template_name_text.Text);
            data.Add("t_increase", time_value.Text);
            data.Add("time_pause", time_pause_text.Text);
            data.Add("number_of_attempts", attempts_num.Text);
            data.Add("allow_digit", allow_digit_check.IsChecked.ToString());
            data.Add("allow_auto_calculate", allow_auto_check.IsChecked.ToString());
            data.Add("template_color_num", color_list_box.SelectedIndex.ToString());
            data.Add("template_line_width", widths_list_box.SelectedIndex.ToString());
            data.Add("measurement_color_num", color_list_box_1.SelectedIndex.ToString());
            data.Add("measurement_line_width", widths_list_box_1.SelectedIndex.ToString());
            data.Add("grid_color_num", color_list_box_2.SelectedIndex.ToString());
            data.Add("grid_line_width", widths_list_box_2.SelectedIndex.ToString());
            data.Add("axis_color_num", color_list_box_3.SelectedIndex.ToString());
            data.Add("axis_line_width", widths_list_box_3.SelectedIndex.ToString());

            tdb.replace_row(name_templ, data);
        }

        private void insert_new_template()
        {
            tdb.insert_(program_settings, template_name_text.Text);
        }
    }
}
