using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;

namespace tass
{
    class Excel_Interface
    {
        public void save_to_excel_file(string file_name, float[] channel_2, int count_channel_2, float[] timeCoordinate, ProgramSettings program_settings)
        {
            DataSet ds = new DataSet("DataSet");            
            DataTable dt = new DataTable("Cигнал");
            
            string[] str_arr_2 = new string[2] { "Сила, [Н]", "Время, [c]" };
            for (int c = 0; c < 2; c++)
                dt.Columns.Add(str_arr_2[c], typeof(double));

            double[,] array = new double[count_channel_2, 2];
            for (int i = 0; i < count_channel_2; i++)
                array[i, 0] = channel_2[i];
            for (int i = 0; i < count_channel_2; i++)
                array[i, 1] = timeCoordinate[i];
            
            dt.BeginLoadData();
            DataTable dt_ = new DataTable("Параметры шаблона");
            string[] str_arr = new string[3] { "Сила, [Н]", "Длительность прикладываемого усилия, [c]", "Количество попыток" };
            for (int c = 0; c < 3; c++)
                dt_.Columns.Add(str_arr[c], typeof(double));
             
            object[] newRow_ = new object[3]; 
            newRow_[0] = program_settings.f_max;
            newRow_[1] = program_settings.time_incr;
            newRow_[2] = program_settings.num_of_attempts;
             
            dt_.LoadDataRow(newRow_, true);

            for (int r = 0; r < count_channel_2; r++)
            { 
                object[] newRow = new object[2]; 
                newRow[0] = array[r, 0];
                newRow[1] = array[r, 1];  
                dt.LoadDataRow(newRow, true); 
            } 
            ds.Tables.Add(dt);
            ds.Tables.Add(dt_); 
            ExcelLibrary.DataSetHelper.CreateWorkbook(file_name + ".xls", ds);
            MessageBox.Show("Результаты измерений сохранены");
        }

        public void load_from_excel_file(string path, int bufferSize, out float[] channel_2, out float[] timeCoordinate, out int count_channel_2)
        {
            var workbook = ExcelLibrary.SpreadSheet.Workbook.Load(path);
            var worksheet = workbook.Worksheets[0];
            var cells = worksheet.Cells;
            var dataTable = new DataTable("datatable");
            // добавить столбцы в таблицу
            dataTable.Columns.Add("force");
            dataTable.Columns.Add("time");
            // добавить строки в таблицу
            for (int rowIndex = cells.FirstRowIndex + 1; rowIndex <= cells.LastRowIndex; rowIndex++)
            {
                var values = new List<string>();
                foreach (var cell in cells.GetRow(rowIndex))
                {
                    values.Add(cell.Value.StringValue);
                }

                dataTable.LoadDataRow(values.ToArray(), true);
            }

            DataRow[] dr = dataTable.Select();
            channel_2 = new float[bufferSize];
            timeCoordinate = new float[bufferSize];
            count_channel_2 = 0;
            
            foreach (DataRow r in dr)
            {
                channel_2[count_channel_2] = Int16.Parse((string)r[0]);
                timeCoordinate[count_channel_2] = Single.Parse((string)r[1]);
                count_channel_2++;
            }
        }
    }
}
