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

namespace tass
{
    delegate void updateLabelCallback(string tekst);
    delegate void updateLabelCallback_1(bool flag);
    delegate void updateButtonCallback(bool value_);
    delegate void updatePlotterCallback(bool flag);
    delegate void updatePlotterCallback_training(bool flag);

    public class MarkerEvent1
    {
        public event _MarkerEventHandler1 someEvent;

        public void OnSomeEvent()
        {
            if (someEvent != null)
                someEvent();
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        COM_Interface com_interface;      
        public static SerialPort _serialPort;
        private DeviceTune device;
        public ProgramSettings program_settings; 
        Template pattern;     
        private main_page main_pg;
        public training_tab tr_tab;
        private settings_tab set_tab;
        tassDB tdb; 
        tarirovka_tab tar_tab;
        calculating_tab calc_tab;
        measuring_tab meas_tab;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                //SplashScreen splashScreen = new SplashScreen("logo.bmp");
                //splashScreen.Show(true);

                InitializeCOMInterface();
                
                create_training_tab(out tr_tab);
                create_tarirovka_tab(out tar_tab);
                create_calc_tab(out calc_tab);
                create_setings_tab(out set_tab);
                create_measuring_tab(out meas_tab);

                main_pg = new main_page();
                main_pg.evt_tarirovka_click.someEvent += new _MarkerEventHandler(tarirovka_click);
                main_pg.evt_Training_Click.someEvent += new _MarkerEventHandler(Training_Click);
                main_pg.evt_draw_calc_choise.someEvent += new _MarkerEventHandler(draw_calc_choise);
                main_pg.evt_Measuring_Click.someEvent += new _MarkerEventHandler(Measuring_Click);
                 
                tdb = new tassDB();
                program_settings = new ProgramSettings(); 
                SetParametersFromSettings(tdb.get_default_settings());
                device = new DeviceTune();
                pattern = new Template(); 
                //Прорисовка главного меню
                MainMenuDraw();
                // прорисовка заставки
                //TimeSpan timeSp = new TimeSpan(0, 0, 3);
                //splashScreen.Close(timeSp);
                //Thread.Sleep(1000);
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
            }
        }

        private void create_training_tab(out training_tab tr_tab)
        {
            tr_tab = new training_tab(this.Dispatcher);
            tr_tab.evtT.someEvent += new _MarkerEventHandler1(train_done);
        }

        private void create_calc_tab(out calculating_tab calc_tab)
        {
            calc_tab = new calculating_tab(this.Dispatcher);
            calc_tab.evtT.someEvent += new _MarkerEventHandler1(digitization);
            calc_tab.evtT1.someEvent += new _MarkerEventHandler1(auto_calculate);
            calc_tab.evt_CloseTab.someEvent += new _MarkerEventHandler1(close_tab);
            calc_tab.evt_auto_calculate.someEvent += new _MarkerEventHandler1(auto_calculate_2);
        }

        private void create_tarirovka_tab(out tarirovka_tab tar_tab)
        {
            tar_tab = new tarirovka_tab(this.Dispatcher);
            tar_tab.evt_tarirovka_step_2.someEvent += new _MarkerEventHandler1(tarirovka_step_2);
            tar_tab.evt_tarirovka_cancel.someEvent += new _MarkerEventHandler1(tarirovka_cancel);
            tar_tab.evt_tarirovka_done.someEvent += new _MarkerEventHandler1(tarirovka_done);
        }

        private void create_setings_tab(out settings_tab set_tab)
        { 
            set_tab = new settings_tab();
            set_tab.evt_cancel_settings_tab.someEvent += new _MarkerEventHandler1(cancel_settings);
            set_tab.evt_apply_settings.someEvent += new _MarkerEventHandler1(apply_settings);
        }

        private void create_measuring_tab(out measuring_tab meas_tab)
        {
            meas_tab = new measuring_tab(this.Dispatcher);
            meas_tab.evt_cancel.someEvent += new _MarkerEventHandler1(MainMenuDraw);
            meas_tab.evt_start_analyse.someEvent += new _MarkerEventHandler1(start_analyse);
            meas_tab.evt_CloseTab.someEvent += new _MarkerEventHandler1(close_tab);
            meas_tab.evt_measuring_draw.someEvent += new _MarkerEventHandler1(measuring_draw);
            meas_tab.evt_measuring_done.someEvent += new _MarkerEventHandler1(measuring_done);
        }

        private void auto_calculate_2()
        {
            close_tab();
            mainWindow_2.Children.Add(calc_tab.draw_calculating_2());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
         
        private void InitializeCOMInterface()
        {
            com_interface = new COM_Interface();
            _serialPort = com_interface.getSerialPort("");
            this.status_text.Text = com_interface.status_text;
        }
 
        private void SetParametersFromSettings(int num_of_default)
        {
            DataTable dt = tdb.query_get_raw(num_of_default.ToString());
            foreach (DataRow r in dt.Rows)
            {
                program_settings.num_of_set_template = Int16.Parse(r[1].ToString());
                program_settings.f_max = (float)Double.Parse(r[2].ToString());
                program_settings.num_of_attempts = Int16.Parse(r[5].ToString());
                program_settings.time_incr = (float)Double.Parse(r[3].ToString());
                program_settings.t_pause = (float)Double.Parse(r[4].ToString());
                program_settings.name_of_set_template = r[0].ToString();
                program_settings.template_color_num = Int32.Parse(r[8].ToString());
                program_settings.template_line_width = Int32.Parse(r[9].ToString());
                program_settings.measurement_color_num = Int32.Parse(r[10].ToString());
                program_settings.measurement_line_width = Int32.Parse(r[11].ToString());
                program_settings.grid_color_num = Int32.Parse(r[12].ToString());
                program_settings.grid_line_width = Int32.Parse(r[13].ToString());
                program_settings.axis_color_num = Int32.Parse(r[14].ToString());
                program_settings.axis_line_width = Int32.Parse(r[15].ToString());
            }
        }
 
        // Прорисовка главного меню
        public void MainMenuDraw()
        {
            mainWindow_2.Children.Add(main_pg.drawMainPage());
        }

        // Обработчики кнопок главного меню
        private void settings_click(object sender, RoutedEventArgs e)
        {
            draw_settings_tab();
        }

        private void cancel_settings()
        {
            close_tab();
            MainMenuDraw();
        }

        private void draw_settings_tab()
        {
            create_setings_tab(out set_tab);
            close_tab();
            mainWindow_2.Children.Add(set_tab.drawSetings(ref program_settings));
        }
           
        void tarirovka_click()
        {
            
            close_tab();
            draw_tarirovka();
        }

        void draw_tarirovka()
        {
            create_tarirovka_tab(out tar_tab);
            mainWindow_2.Children.Add(tar_tab.draw_tarirovka_step_1());
        }

        private void tarirovka_step_2()
        {
            close_tab();
            mainWindow_2.Children.Add(tar_tab.draw_tarirovka_step_2());
        }

        private void tarirovka_cancel()
        {
            close_tab();
            MainMenuDraw();
        }

        private void tarirovka_done()
        {
            tar_tab.getTunedDevice(out device, ref program_settings);
            close_tab();
            MainMenuDraw();
        }

        private void apply_settings()
        {
            set_tab.getProgramSettings(ref pattern, ref program_settings);
            close_tab();
            MainMenuDraw();
        }

        private void digitization()
        {
            create_calc_tab(out calc_tab);
            close_tab();
            mainWindow_2.Children.Add(calc_tab.digitization()); 
        }

        private void auto_calculate()
        {
            close_tab();
            mainWindow_2.Children.Add(calc_tab.auto_calculate(program_settings)); 
        }        

        private void Training_Click(object sender, RoutedEventArgs e)
        {
            Training_Click();
        }

        private void Training_Click()
        {
            close_tab();
            create_training_tab(out tr_tab);
            mainWindow_2.Children.Add(tr_tab.training_tab_draw(program_settings)); 
        } 

        // Переход в главное меню после окончания тренировки
        private void train_done()
        {
            close_tab();
            MainMenuDraw();
        }
         
        private void Measuring_Click()
        {
            create_measuring_tab(out meas_tab);
            close_tab();
            mainWindow_2.Children.Add(meas_tab.measuring_tab_3alternatives(program_settings, pattern)); 
        }

        public void set_titles(string center_str, string right_str)
        {
            title_center.Content = center_str;
            title_right.Content = right_str;
        }

        private void start_analyse()
        { 
            close_tab();
            mainWindow_2.Children.Add(calc_tab.draw_calculating());
        }

        private void measuring_draw()
        { 
            close_tab();
            mainWindow_2.Children.Add(meas_tab.measuring_draw());
        }

        private void measuring_done()
        { 
            close_tab();
            mainWindow_2.Children.Add(meas_tab.draw_measuring_done());
        }

        private void draw_calc_choise()
        {
            close_tab();
            mainWindow_2.Children.Add(calc_tab.draw_calculating());
        }

        private void close_tab(object sender, RoutedEventArgs e)
        {
            close_tab();
        }

        private void close_tab()
        {  
            mainWindow_2.Children.Clear();
        }

        private void Main_Window_Click(object sender, RoutedEventArgs e)
        {
            close_tab();
            MainMenuDraw();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            mainWindow_.Children.Clear();
            this.Close();
        }
    }
}

