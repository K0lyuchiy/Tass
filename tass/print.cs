using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace tass
{
    class print
    {
        public void printing_2(Grid grid_table_print)
        {
            DocumentViewer documentViewer1 = new DocumentViewer();
            FixedDocument fixedDoc = new FixedDocument();
            PageContent pgc = new PageContent();
            FixedPage fxp = new FixedPage();
            //A4
            fxp.Width = 11.69 * 96;
            fxp.Height = 8.27 * 96;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panel.Width = (11.69 * 96) * 0.9;
            panel.Orientation = Orientation.Vertical;
            Thickness margin = panel.Margin;
            margin.Bottom = 0;
            margin.Left = 50;
            margin.Top = 50;
            margin.Right = 25;
            panel.Margin = margin;
            BitmapImage bmp_ = new BitmapImage();
            Label test_lb = new Label();
            test_lb.Content = "\n\n\t\t\tРежим расчетов \n \tОцифровка в автоматическом режиме";
            margin = test_lb.Margin;
            margin.Bottom = 50;
            margin.Left = 50;
            margin.Top = 50;
            margin.Right = 0;
            test_lb.BorderThickness = margin;

            panel.Children.Add(test_lb);

            ImageSource imageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\file.jpg"));

            Image img = new Image();
            img.Source = imageSource;
            panel.Children.Add(img);

            Grid grid_table_print_copy = new Grid();// { DataContext = grid_table_print.DataContext };
            string gridXaml = XamlWriter.Save(grid_table_print);
            StringReader stringReader = new StringReader(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            grid_table_print_copy = (Grid)XamlReader.Load(xmlReader);

            panel.Children.Add(grid_table_print_copy);

            fxp.Children.Add(panel);

            ((System.Windows.Markup.IAddChild)pgc).AddChild(fxp);
            fixedDoc.Pages.Add(pgc);

            documentViewer1.Document = fixedDoc;
            Window ShowWindow = new Window();
            ShowWindow.Width = 850;
            ShowWindow.Height = 850;
            ShowWindow.Content = documentViewer1;
            ShowWindow.Show();
        }

        public void printing_3(Grid grid_table_print_2)
        { 
            DocumentViewer documentViewer1 = new DocumentViewer();
            //documentViewer1.Width = 800;
            ///documentViewer1.Height = 800;
            FixedDocument fixedDoc = new FixedDocument();
            PageContent pgc = new PageContent();
            FixedPage fxp = new FixedPage();
            //A4
            fxp.Width = 11.69 * 96;
            fxp.Height = 8.27 * 96;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            panel.Width = (11.69 * 96) * 0.9;

            panel.Orientation = Orientation.Vertical;
            Thickness margin = panel.Margin;
            margin.Bottom = 0;
            margin.Left = 50;
            margin.Top = 50;
            margin.Right = 25;
            panel.Margin = margin;
            BitmapImage bmp_ = new BitmapImage();
            Label test_lb = new Label();
            test_lb.Content = "\n\n\t\t\tРежим расчетов \n \tОцифровка в автоматическом режиме";
            margin = test_lb.Margin;
            margin.Bottom = 50;
            margin.Left = 50;
            margin.Top = 50;
            margin.Right = 0;
            test_lb.BorderThickness = margin;

            panel.Children.Add(test_lb);

            ImageSource imageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\file.jpg"));

            Image img = new Image();
            img.Source = imageSource;

            panel.Children.Add(img);

            Grid grid_table_print_copy = new Grid();
            string gridXaml = XamlWriter.Save(grid_table_print_2);
            StringReader stringReader = new StringReader(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            grid_table_print_copy = (Grid)XamlReader.Load(xmlReader);

            panel.Children.Add(grid_table_print_copy);

            fxp.Children.Add(panel);

            ((System.Windows.Markup.IAddChild)pgc).AddChild(fxp);
            fixedDoc.Pages.Add(pgc);

            documentViewer1.Document = fixedDoc;

            Window ShowWindow = new Window();
            ShowWindow.Width = 850;
            ShowWindow.Height = 850;
            ShowWindow.Content = documentViewer1;
            ShowWindow.Show();
        }

        public void print_measures( )
        {
            DocumentViewer documentViewer1 = new DocumentViewer();
            //documentViewer1.Width = 800;
            ///documentViewer1.Height = 800;
            FixedDocument fixedDoc = new FixedDocument();
            PageContent pgc = new PageContent();
            FixedPage fxp = new FixedPage();

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            panel.Orientation = Orientation.Vertical;

            Thickness margin = panel.Margin;
            margin.Bottom = 0;
            margin.Left = 50;
            margin.Top = 50;
            margin.Right = 25;
            panel.Margin = margin;

            BitmapImage bmp_ = new BitmapImage();
            Label test_lb = new Label();
            test_lb.Content = "\n\nРезультаты измерений";
            margin = test_lb.Margin;
            margin.Bottom = 50;
            margin.Left = 50;
            margin.Top = 50;
            margin.Right = 0;
            test_lb.BorderThickness = margin;

            panel.Children.Add(test_lb);

            ImageSource imageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\file.jpg"));

            Image img = new Image();
            img.Source = imageSource;

            panel.Children.Add(img);

            fxp.Children.Add(panel);

            ((System.Windows.Markup.IAddChild)pgc).AddChild(fxp);
            fixedDoc.Pages.Add(pgc);
            documentViewer1.Document = fixedDoc;
            Window ShowWindow = new Window();
            ShowWindow.Width = 850;
            ShowWindow.Height = 850;
            ShowWindow.Content = documentViewer1;
            ShowWindow.Show();
        }       
    }
}
