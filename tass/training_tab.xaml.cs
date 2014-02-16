using System;
using System.Collections.Generic; 
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Documents; 

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;

using System.Windows.Threading;
using System.Threading;

namespace tass
{
    public delegate void _MarkerEventHandler1();

    /// <summary>
    /// Логика взаимодействия для training_tab.xaml
    /// </summary>
    /// 
    public partial class training_tab : Page 
    {
        public MarkerEvent1 evtT;
        public static PlotterControl plotter_cntrl;  

        public training_tab(Dispatcher disp)
        {
            evtT = new MarkerEvent1();
            plotter_cntrl = new PlotterControl(disp);
            plotter_cntrl.TRAIN_FLAG = true;
            plotter_cntrl.pattern.create_template();
            InitializeComponent();
        }

        public Grid training_tab_draw(ProgramSettings program_settings)
        {
            plotter_cntrl.closePlotter();
            Grid grid_main = new Grid();
            RowDefinition row_def = new RowDefinition();
            row_def.Height = new GridLength(150);
            RowDefinition row_def_1 = new RowDefinition();
            RowDefinition row_def_2 = new RowDefinition();
            row_def_2.Height = new GridLength(100);
            ColumnDefinition col_def = new ColumnDefinition();
            grid_main.RowDefinitions.Add(row_def);
            grid_main.RowDefinitions.Add(row_def_1);
            grid_main.RowDefinitions.Add(row_def_2);
            grid_main.ColumnDefinitions.Add(col_def);

            TextBlock train_text = new TextBlock();
            train_text.Inlines.Add(new Bold(new Run("Тренировка мышечного восприятия  ")));
            train_text.Inlines.Add(new Run("\n\n Повторение по шаблону циклов развития усилия с параметрами: \n\n"));
            train_text.Inlines.Add(new Bold(new Run("   Fш = " + program_settings.f_max.ToString() + " Н.; tш = " + program_settings.time_incr.ToString() + " c.; tпаузы = " + program_settings.t_pause.ToString() + " c.; Количество циклов = " + program_settings.num_of_attempts.ToString())));
            Grid.SetRow(train_text, 0);
            Grid.SetColumn(train_text, 0);
            train_text.Height = 80;
            Thickness margin = train_text.Margin;
            margin.Bottom = 25;
            margin.Left = 25;
            margin.Top = 25;
            margin.Right = 0;
            train_text.Margin = margin;

            plotter_cntrl.plotter = new ChartPlotter();
            plotter_cntrl.plotter.Legend.Remove();
            plotter_cntrl.plotter.FitToView();
            Grid.SetRow(plotter_cntrl.plotter, 1);
            Grid.SetColumn(plotter_cntrl.plotter, 0);

            Button step_3_button = AuxiliaryFunc.getNewButton("Стоп", 150, 25);
            step_3_button.Click += new RoutedEventHandler(train_done);
            step_3_button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            AuxiliaryFunc.setMargin((Control)step_3_button, 25, 10, 0, 10);
            Grid.SetRow(step_3_button, 2);
            Grid.SetColumn(step_3_button, 0);

            grid_main.Children.Add(train_text);
            grid_main.Children.Add(step_3_button);
            grid_main.Children.Add(plotter_cntrl.plotter);

            Grid.SetRow(grid_main, 1);
            Grid.SetColumn(grid_main, 0);

            plotter_cntrl.StartGettingData();
            plotter_cntrl.StartPlotter();

            return grid_main;
        }

        public void train_done(object sender, RoutedEventArgs e)
        {
            plotter_cntrl.closePlotter();
            evtT.OnSomeEvent();
        }  
    }
}
