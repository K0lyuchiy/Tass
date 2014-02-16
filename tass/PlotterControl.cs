using System;
using System.Collections;
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
    public partial class PlotterControl
    {
        public Microsoft.Research.DynamicDataDisplay.ChartPlotter plotter;
        Queue m_smplQueue;
        public MarkerEvent1 evt_1;
        public MarkerEvent1 evt_2; 
        Dispatcher dp_test;
        // Three observable data sources. Observable data source contains
        // inside ObservableCollection. Modification of collection instantly modify
        // visual representation of graph. 
        public ObservableDataSource<Point> source1 = null;
        public ObservableDataSource<Point> source2 = null;
        public ObservableDataSource<Point> source3 = null;
        public ObservableDataSource<Point> source4 = null;

        EnumerableDataSource<float> animatedDataSource = null;

        public ObservableCollection<DataTable> _observableCollection =
        new ObservableCollection<DataTable>();

        static bool _continue;
        static SerialPort _serialPort;
       

        public static int BUFFER_SIZE = 50000;
        public float[] channel_1 = new float[BUFFER_SIZE];
        public float[] channel_2 = new float[BUFFER_SIZE];
        public float[] timeCoordinate = new float[BUFFER_SIZE];         

        public bool SHOW_PATTERN = true;         
        
        public int FLAG_MEASURING_MODE = 0;
        public bool PRECALIBRATE_FLAG = false;
         
        private DeviceTune device;
        public ProgramSettings program_settings;
          
        public Template pattern;
         
        delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

        UpdateProgressBarDelegate updProgress;

        public LineGraph chart_line_source_1;
        public LineGraph chart_line_source_2;         

        Label label_measuring;        

        public bool TRAIN_FLAG = true;
        public int count_channel_1;
        public int count_channel_2; 
        Thread simThread;
        Thread readThread;
        public bool calibrate_flag = false;
        public bool tarirovka_flag = false;
        public bool ch_template = false;
        LineGraph chart_plotter_template; 
        ObservableDataSource<Point> source_temp_1;
        LineGraph point_treshold;

        readonly double[] animatedX = new double[3000];
        readonly double[] animatedY = new double[3000];
        float[] temp_x = new float[1];
        float[] temp_y = new float[1];         

        float F_treshold;
        float t_treshold;

        double value_progress = 0;
        float[] coordinates = new float[2];   
        int READ_SIZE = 20; 

        public PlotterControl(Dispatcher disp)
        {
            evt_1 = new MarkerEvent1();
            evt_2 = new MarkerEvent1();
            m_smplQueue = new Queue();
            device = new DeviceTune();
            pattern = new Template();
            // Создание шаблона для тренировки
            pattern.create_template();
            program_settings = new ProgramSettings();
            dp_test = disp;
        } 

        private void UpdatePlotter_training(bool flag)
        {
            if (plotter.Dispatcher.CheckAccess() == false)
            {
                updatePlotterCallback_training uCallBack = new updatePlotterCallback_training(UpdatePlotter_training);
                dp_test.Invoke(uCallBack, flag);
            }
            else
            {
                try
                {
                    chart_line_source_1.DataSource = null;
                    // chart_line_source_2.DataSource = null;
                    count_channel_1 = 1;
                    count_channel_2 = 1;
                    // Create first source
                    source1 = new ObservableDataSource<Point>();
                    // Set identity mapping of point in collection to point on plot
                    source1.SetXYMapping(p => p);
                    chart_line_source_1 = plotter.AddLineGraph(source1, Color.FromRgb(12, 0, 4), 2, "Измерения");
                }
                catch (Exception fail)
                {
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    MessageBox.Show(error);
                }
            }
        }

        private void UpdatePlotter(bool flag)
        {
            if (plotter.Dispatcher.CheckAccess() == false)
            {

                updatePlotterCallback uCallBack = new updatePlotterCallback(UpdatePlotter);
                dp_test.Invoke(uCallBack, flag);
            }
            else
            {
                try
                {
                    {
                        animatedDataSource.RaiseDataChanged();
                    }
                }
                catch (Exception fail)
                {
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    MessageBox.Show(error);
                }
            }
        }   

        private void UpdateLabel_measuring(bool flag)
        {
            if (label_measuring.Dispatcher.CheckAccess() == false)
            {
                updateLabelCallback_1 uCallBack = new updateLabelCallback_1(UpdateLabel_measuring);
                dp_test.Invoke(uCallBack, flag);
            }
            else
            {
                if (flag)
                {
                    label_measuring.Foreground = Brushes.Red;
                }
                else
                    label_measuring.Foreground = Brushes.Black;
            }
        }

        public void plot_measured_data()
        {
            int buffer_length = count_channel_2;
            count_channel_1 = 0;
            count_channel_2 = 0;

            // Create first source
            source1 = new ObservableDataSource<Point>();
            // Set identity mapping of point in collection to point on plot
            source1.SetXYMapping(p => p);
            // Create second source
            source2 = new ObservableDataSource<Point>();
            // Set identity mapping of point in collection to point on plot
            source2.SetXYMapping(p => p);
            // Create third source
            source3 = new ObservableDataSource<Point>();
            // Set identity mapping of point in collection to point on plot
            source3.SetXYMapping(p => p);
            // Add all three graphs. Colors are not specified and chosen random
            chart_line_source_1 = plotter.AddLineGraph(source1, Color.FromRgb(12, 0, 4), 2, "Измерения");
            //plotter.AddLineGraph(source2, 2, "Шаблон");
            ObservableDataSource<Point> source_temp = new ObservableDataSource<Point>();
            Point p_temp = new Point(30, 210);
            source_temp.AppendAsync(dp_test, p_temp);
            plotter.AddLineGraph(source_temp, 0);

            double x_1 = 0;
            double y_1 = 0;
            double x_2 = 0;
            double y_2 = 0;
            for (int i = 1; i < buffer_length; i++)
            {
                x_1 = (float)timeCoordinate[i - 1];
                y_1 = (float)channel_1[i - 1];
                x_2 = (float)timeCoordinate[i - 1];
                y_2 = (float)channel_2[i - 1];

                Point p1 = new Point(x_2, y_2);
                Point p2 = new Point(x_1, y_1);

                source1.AppendAsync(dp_test, p1);
                source2.AppendAsync(dp_test, p2);
            }
            count_channel_1 = buffer_length;
            count_channel_2 = buffer_length;
        }

        public void plot_template()
        {
            double[] x_1 = new double[pattern.length];
            double[] y_1 = new double[pattern.length]; ;

            for (int i = 0; i < pattern.length; i++)
            {
                x_1[i] = (double)pattern.template_x[i];
                y_1[i] = (double)pattern.template_y[i];
            }
            var yDataSource = new EnumerableDataSource<double>(y_1);
            yDataSource.SetYMapping(Y => Y);
            yDataSource.AddMapping(Microsoft.Research.DynamicDataDisplay.PointMarkers.ShapeElementPointMarker.ToolTipTextProperty,
                Y => string.Format("Value is {0}", Y));

            var xDataSource = new EnumerableDataSource<double>(x_1);
            xDataSource.SetXMapping(X => X);
            CompositeDataSource compositeDataSource = new CompositeDataSource(xDataSource, yDataSource);
            chart_plotter_template = plotter.AddLineGraph(compositeDataSource, Color.FromRgb(12, 0, 4), 2, "Шаблон");
        }

        public void StartPlotter()
        {             
                try
                {
                    // Create first source
                    source1 = new ObservableDataSource<Point>();
                    // Set identity mapping of point in collection to point on plot
                    source1.SetXYMapping(p => p);
                    // Create second source
                    source2 = new ObservableDataSource<Point>();
                    // Set identity mapping of point in collection to point on plot
                    source2.SetXYMapping(p => p);
                    // Create third source
                    source3 = new ObservableDataSource<Point>();
                    // Set identity mapping of point in collection to point on plot
                    source3.SetXYMapping(p => p);
                    // Create third source
                    source4 = new ObservableDataSource<Point>();
                    // Set identity mapping of point in collection to point on plot
                    source4.SetXYMapping(p => p);
                    // Add all three graphs. Colors are not specified and chosen random
                    chart_line_source_1 = plotter.AddLineGraph(source1, Color.FromRgb(12, 0, 4), 2, "Измерения");
                    chart_line_source_2 = plotter.AddLineGraph(source2, 2, "Шаблон");
                    if (TRAIN_FLAG)
                    {
                        plot_template();
                    }
                    {
                        temp_x[0] = 0;
                        temp_y[0] = 0;
                    }
                    if (TRAIN_FLAG || PRECALIBRATE_FLAG)
                    {
                        EnumerableDataSource<float> xSrc = new EnumerableDataSource<float>(temp_x);
                        xSrc.SetXMapping(x => x);
                        animatedDataSource = new EnumerableDataSource<float>(temp_y);
                        animatedDataSource.SetYMapping(y => y);
                        // Adding graph to plotter
                        plotter.AddLineGraph(new CompositeDataSource(xSrc, animatedDataSource),
                              new Pen(Brushes.Magenta, 3),
                              new Microsoft.Research.DynamicDataDisplay.PointMarkers.CirclePointMarker
                              {
                                  Size = 10,
                                  Fill = Brushes.Orange
                              },
                              new PenDescription("Sin(x + phase)"));
                        ObservableDataSource<Point> source_temp_1 = new ObservableDataSource<Point>();
                        Point p_temp_1 = new Point(0, 0);
                        source_temp_1.AppendAsync(dp_test, p_temp_1);
                        plotter.AddLineGraph(source_temp_1, 0);
                    } 
                    ObservableDataSource<Point> source_temp = new ObservableDataSource<Point>();
                    Point p_temp = new Point(pattern.time_axis, program_settings.f_max * 1.25);
                    source_temp.AppendAsync(dp_test, p_temp);
                    plotter.AddLineGraph(source_temp, 0); 

                    simThread = new Thread(new ThreadStart(Simulation));
                    simThread.IsBackground = true;
                    simThread.Start();
                } 
               catch (Exception ex)
                {
                    string messageBoxText = ex.ToString();
                    string caption = "Error";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }
        }

        public void closePlotter()
        {
            try
            {
                _serialPort.Close();
                 _continue = true;
                if (readThread != null)
                    readThread.Abort();
                StopPlotter();
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
            }
        }
       
        public void set_scale()
        {
            if (point_treshold != null)
            {
                if (point_treshold.DataSource != null)
                    point_treshold.DataSource = null;
            }
            source_temp_1 = new ObservableDataSource<Point>();
            Point p_temp = new Point(t_treshold, F_treshold);
            source_temp_1.AppendAsync(dp_test, p_temp);
            point_treshold = plotter.AddLineGraph(source_temp_1, 0);
        }
       
        public void StopPlotter()
        {
            if (simThread != null)
                simThread.Abort();
        }

        private void Simulation()
        { 
            double x_1 = 0;
            double y_1 = 0;
            double x_2 = 0;
            double y_2 = 0;
            try
            {
               while (true)
                {
                    lock (m_smplQueue)
                    {
                        if (count_channel_1 != 0)
                        {
                            x_1 = (float)timeCoordinate[count_channel_1 - 1];
                            y_1 = (float)channel_1[count_channel_1 - 1];

                            x_2 = (float)timeCoordinate[count_channel_2 - 1];
                            y_2 = (float)channel_2[count_channel_2 - 1];
                        }
                        Point p1 = new Point(x_2, y_2);
                        Point p2 = new Point(x_1, y_1); 

                        if (TRAIN_FLAG)
                        {
                            if (count_channel_1 < pattern.length)
                            {
                                source1.AppendAsync(dp_test, p1);
                                temp_x[0] = (float)timeCoordinate[count_channel_1 - 1];
                                temp_y[0] = channel_1[count_channel_1 - 1]; 
                                UpdatePlotter(true);
                            }
                            else
                            {
                                // Возврат шаблона в начало координат
                                UpdatePlotter_training(true);
                            }
                        }
                        else if (PRECALIBRATE_FLAG)
                        {
                            temp_x[0] = 0;
                            temp_y[0] = channel_2[count_channel_2 - 1];
                            UpdatePlotter(true);
                        }
                        else
                        {

                            if (count_channel_1 < pattern.length)
                            {
                                if (FLAG_MEASURING_MODE == 1)
                                {
                                    int ijk = pattern.length;
                                    dp_test.Invoke(updProgress, new object[] { ProgressBar.ValueProperty, ++value_progress }); 
                                }
                                if (FLAG_MEASURING_MODE == 1 || FLAG_MEASURING_MODE == 2)
                                {
                                    if (Math.IEEERemainder(((long)count_channel_1), ((long)50)) == 0 && Math.IEEERemainder(((long)count_channel_1), ((long)100)) == 0)
                                    {
                                        UpdateLabel_measuring(true);
                                    }
                                    else if (Math.IEEERemainder(((long)count_channel_1), ((long)50)) == 0)
                                    {
                                        UpdateLabel_measuring(false);
                                    }
                                }
                                source1.AppendAsync(dp_test, p1);
                            }
                        }
                        Monitor.Pulse(m_smplQueue);
                        Monitor.Wait(m_smplQueue);
                    }
                    //Thread.Sleep(10); // Long-long time for computations...
                }
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                //MessageBox.Show(error);
            } 
        }

        public void StartGettingData()
        {
            try
            {
                _serialPort = MainWindow._serialPort;
                _serialPort.Open();
                _continue = true;
                clear_Buffer();
                readThread = new Thread(Read);
                readThread.Start();
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
            }
        }

        public void Read()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int offset = 0;

            while (_continue)
            {
                lock (m_smplQueue)
                {
                    try
                    {
                        int size = _serialPort.Read(buffer, offset, READ_SIZE);
                        if (size < 4)
                        {
                            int size_temp = _serialPort.Read(buffer, offset + size, READ_SIZE);
                            size = size + size_temp;
                        }
                        Debug.WriteLine(READ_SIZE.ToString() + "(" + DateTime.Now.Millisecond.ToString() + "):" + size.ToString(), "Field");

                        sendToChannelBuffer(buffer, offset, size);
                        offset = offset + size;
                        if ((offset + 4) > BUFFER_SIZE)
                        {
                            offset = 0;
                        }
                        checkState(); // для режима тарировки
                    }
                    catch (Exception fail)
                    {
                        String error = "The following error has occurred:\n\n";
                        error += fail.Message.ToString() + "\n\n";
                        Debug.WriteLine(error, "Field");
                    }
                    Monitor.Pulse(m_smplQueue);//
                    Monitor.Wait(m_smplQueue);
                }       
            }
        }

        private void checkState()
        {
            if (calibrate_flag)
            {
                if (count_channel_1 > 300)
                {
                    calibrate_flag = false;
                    {
                        _serialPort.Close();
                        _continue = false;
                        StopPlotter();                        
                        evt_1.OnSomeEvent();// Проверка выполнения балансировки
                        count_channel_1 = 0;
                        readThread.Abort();
                    }
                }
            }
            if (tarirovka_flag)
            {
                if (count_channel_1 > 300)
                {
                    tarirovka_flag = false;
                    {
                        _serialPort.Close();
                        _continue = false;
                        StopPlotter();                        
                        evt_2.OnSomeEvent();// Проверка выполнения балансировки
                        count_channel_1 = 0;
                        readThread.Abort();
                    }
                }
            } 
        }
        
        public void clear_Buffer()
        {
            channel_1 = new float[BUFFER_SIZE];
            channel_2 = new float[BUFFER_SIZE];
            timeCoordinate = new float[BUFFER_SIZE];
            count_channel_1 = 0;
            count_channel_2 = 0;
        }

        public void sendToChannelBuffer(byte[] buffer, int offset, int size)
        {
            for (int h = 0; h < size / 4; h++)
            {
                byte[] buf_temp = new byte[4];

                buf_temp[0] = buffer[offset];
                buf_temp[1] = buffer[offset + 1];
                buf_temp[2] = buffer[offset + 2];
                buf_temp[3] = buffer[offset + 3];
                {
                    byte ch_1_byte_1 = 0;
                    byte ch_1_byte_2 = 0;
                    byte ch_2_byte_1 = 0;
                    byte ch_2_byte_2 = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        int num_of_byte = buf_temp[i] >> 6;
                        switch (num_of_byte)
                        {
                            case 0:
                                ch_1_byte_1 = (byte)(buf_temp[i] & 0x3F);
                                break;
                            case 1:
                                ch_1_byte_2 = (byte)(buf_temp[i] & 0x3F);
                                break;
                            case 2:
                                ch_2_byte_1 = (byte)(buf_temp[i] & 0x3F);
                                break;
                            case 3:
                                ch_2_byte_2 = (byte)(buf_temp[i] & 0x3F);
                                break;
                            default:
                                break;
                        }
                    }

                    short data = 0;

                    if (TRAIN_FLAG)
                    {
                        channel_1[count_channel_1] = device.scale_measurements(pattern.getData(), false);
                    }
                    else if (SHOW_PATTERN)
                    {
                        channel_1[count_channel_1] = device.scale_measurements(pattern.getData(), false);
                    }
                    else
                    {
                        data = ch_1_byte_2;
                        data = (short)(data << 6);
                        channel_1[count_channel_1] = device.scale_measurements((data + ch_1_byte_1), true);
                    }
                    if (count_channel_1 == 0)
                        timeCoordinate[count_channel_1] = 0.010F;
                    else
                    {
                        timeCoordinate[count_channel_1] = (timeCoordinate[count_channel_1 - 1] + 0.010F) % 40;
                    }
                    count_channel_1 = count_channel_1 + 1;
                    if (count_channel_1 >= BUFFER_SIZE)
                        count_channel_1 = 0;
                    data = ch_2_byte_2;
                    data = (short)(data << 6);
                    data = (short)(data + ch_2_byte_1); 
                    bool debug__1 = true;// false;
                    if (debug__1)
                    {
                        if (count_channel_2 != 0)
                        {
                            if (data < 10 && data >= 0)
                                channel_2[count_channel_2] = device.scale_measurements((data), true);
                            else if (Math.Abs(data) < 100 * channel_2[count_channel_2 - 1] && data != 0)
                                channel_2[count_channel_2] = device.scale_measurements((data), true);
                            else if (data == 0)
                                channel_2[count_channel_2] = device.scale_measurements((data), true);
                            else
                                channel_2[count_channel_2] = -1 * device.scale_measurements((channel_2[count_channel_2 - 1]), true);
                        }
                        else
                            channel_2[count_channel_2] = device.scale_measurements((data), true);
                    }
                    else
                        channel_2[count_channel_2] = device.scale_measurements((data), true);
                    count_channel_2 = count_channel_2 + 1;
                    if (count_channel_2 >= BUFFER_SIZE)
                        count_channel_2 = 0;
                }
            }
        }

        public void delete_template()
        {
            if (chart_plotter_template != null)
            {
                chart_plotter_template.DataSource = null;
            }
        }

        public void shift_to_right(object sender, RoutedEventArgs e)
        {
            if (ch_template)
            {
                delete_template();
                shift_template(50);
                plot_template();
            }
        }

        public void shift_template(int step)
        {
            pattern.shift_template(step);
        }

        public void shift_to_left(object sender, RoutedEventArgs e)
        {
            if (ch_template)
            {
                delete_template();
                shift_template(-50);
                plot_template();
            }
        }

        // Вспомогательные функции работы с графиком
        public void delete_signal()
        {
            if (chart_line_source_1.DataSource != null)
            {
                chart_line_source_1.DataSource = null;
            }
        }

        public void plot_signal()
        {
            if (chart_line_source_1.DataSource == null)
                chart_line_source_1 = plotter.AddLineGraph(source1, Color.FromRgb(12, 0, 4), 2, "Измерения");
        }

        public void plot_grid()
        {
            if (plotter.AxisGrid.Plotter == null)
                plotter.AxisGrid.AddToPlotter(plotter);
        }

        public void plot_axis()
        {
            if (plotter.HorizontalAxis.Plotter == null)
            {
                plotter.HorizontalAxis.AddToPlotter(plotter);
                plotter.VerticalAxis.AddToPlotter(plotter);
            }
        }

        public void clear_grid()
        {
            if (plotter.AxisGrid.Plotter != null)
                plotter.AxisGrid.Remove();
        }

        public void clear_axis()
        {
            if (plotter.HorizontalAxis.Plotter != null)
            {
                plotter.HorizontalAxis.Remove();
                plotter.VerticalAxis.Remove();
            }
        }
    }
}
