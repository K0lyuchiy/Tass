using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace tass
{
    class tassDB
    {
        private SQLiteDatabase db;

        public int get_last_idx_of_templates()
        {
            string max_idx = "0"; ;
            try
            {
                db = new SQLiteDatabase();
                DataTable recipe;
                String query = "SELECT MAX(template_number) FROM settings";
                recipe = db.GetDataTable(query);
                DataRow r = recipe.Rows[0];
                max_idx = r[0].ToString();
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                //this.Close();
            }
            if (max_idx == "")
                return 0;
            else
                return Int32.Parse(max_idx);
        }

        public int get_max_idx()
        {
            string max_idx = "0"; ;
            try
            {
                db = new SQLiteDatabase();
                DataTable recipe;
                String query = "SELECT MAX(num_of_measurement) FROM measurement_results";
                recipe = db.GetDataTable(query);
                DataRow r = recipe.Rows[0];
                max_idx = r[0].ToString();
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
               // this.Close();
            }
            if (max_idx == "")
                return 0;
            else
                return Int32.Parse(max_idx);
        }

        public DataTable query_()
        {
            try
            {
                db = new SQLiteDatabase();
                DataTable recipe;
                String query = "select template_name \"Template_name\", template_number \"Template_number\",";
                query += "f_max  \"F_max\", t_increase \"T_increase\", time_pause \"Time_pause\",";
                query += "number_of_attempts  \"Number_of_attempts\", allow_digit \"Allow_digit\", allow_auto_calculate \"Allow_auto_calculate\"";
                query += "from settings;";
                recipe = db.GetDataTable(query);
                return recipe;
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                // this.Close();
                return null;
            }
        }

        public bool is_template_in_db(string template_num)
        {
            DataTable recipe = new DataTable();
            try
            {
                db = new SQLiteDatabase();
                String query = "select template_name \"Template_name\", template_number \"Template_number\"";

                query += "from settings WHERE template_name='" + template_num + "';";
                recipe = db.GetDataTable(query);
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                return false;
            }
            if (recipe.Rows.Count == 0)
                return false;
            else
                return true;
        }

        public int get_default_settings()
        {
            DataTable recipe = new DataTable();
            try
            {
                db = new SQLiteDatabase();
                String query = "select default_settings \"Default_settings\"";
                query += " from default_set WHERE extra='239';";
                recipe = db.GetDataTable(query);
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                return 0;
            }
            DataRow dr = recipe.Rows[0];
            return Int32.Parse(dr[0].ToString());
        }

        public DataTable query_data()
        {
            try
            {
                db = new SQLiteDatabase();
                DataTable recipe;
                String query = "select name_of_measurement \"Name_of_measurement\", num_of_measurement \"Num_of_measurement\",";
                query += "date  \"Date\"";
                query += "from measurement_results;";
                recipe = db.GetDataTable(query);
                return recipe;   
            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                return null;
            }
        }

        public DataTable query_get_raw(string param)
         {
             DataTable recipe = new DataTable();
             try
             {
                 db = new SQLiteDatabase();
                 String query = "select template_name \"Template_name\", template_number \"Template_number\",";
                 query += "f_max  \"F_max \", t_increase \"T_increase\", time_pause \"Time_pause\",";
                 query += "number_of_attempts  \"Number_of_attempts \", allow_digit \"Allow_digit\", allow_auto_calculate \"Allow_auto_calculate\",";
                 query += "template_color_num \"Template_color_num\", template_line_width \"Template_line_width\",";
                 query += "measurement_color_num \"measurement_color_num\", measurement_line_width \"measurement_line_width\",";
                 query += "grid_color_num \"grid_color_num\", grid_line_width \"grid_line_width\",";
                 query += "axis_color_num \"axis_color_num\", axis_line_width \"axis_line_width\"";

                 query += "from settings WHERE template_number='" + param + "';";
                 recipe = db.GetDataTable(query);
             }
             catch (Exception fail)
             {
                 String error = "The following error has occurred:\n\n";
                 error += fail.Message.ToString() + "\n\n";
                 MessageBox.Show(error);                 
             }
             return recipe;
         }

        public void replace_set(string name_templ)
         {
             db = new SQLiteDatabase();
             Dictionary<String, String> data = new Dictionary<String, String>();
             data.Add("default_settings", name_templ);

             string where_str = "extra = '239'";
             try
             {
                 db.Update("default_set", data, where_str);
             }
             catch (Exception crap)
             {
                 MessageBox.Show(crap.Message);
             }
         }         
        
        public string query_get_path(string idx)
         {
             string path = "0"; ;
             try
             {
                 db = new SQLiteDatabase();
                 DataTable recipe;
                 String query = "SELECT path FROM measurement_results WHERE num_of_measurement = '" + idx + "'";
                 recipe = db.GetDataTable(query);
                 DataRow r = recipe.Rows[0];
                 path = r[0].ToString();
             }
             catch (Exception fail)
             {
                 String error = "The following error has occurred:\n\n";
                 error += fail.Message.ToString() + "\n\n";
                 MessageBox.Show(error);                 
             }
             return path;
         }

        public void SaveMeasuringToSQLite(string prefix, string numOfTemplate, string number, string lbs)
        {
            // Сохранение результатов измерений
            db = new SQLiteDatabase();
            Dictionary<String, String> data = new Dictionary<String, String>();
            //data.Add("num_of_measurement", "1");
            data.Add("name_of_measurement ", prefix);
            data.Add("num_of_settings_template ", numOfTemplate);
            data.Add("date", DateTime.Now.ToString());
            data.Add("path", lbs + prefix + "_" + number + ".xls");
            try
            {
                db.Insert("measurement_results", data);
            }
            catch (Exception crap)
            {
                MessageBox.Show(crap.Message);
            }
        }

        public void replace_row(string name_templ, Dictionary<String, String> dc)
        { 
            db = new SQLiteDatabase();
            string where_str = "template_name = '" + name_templ + "'";
            try
            {
                db.Update("settings", dc, where_str);
            }
            catch (Exception crap)
            {
                MessageBox.Show(crap.Message);
            }
        }

        public void insert_(ProgramSettings program_settings, String template_name)
        {
            db = new SQLiteDatabase();
            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("f_max", program_settings.f_max.ToString());
            data.Add("template_name", template_name);
            data.Add("t_increase", program_settings.time_incr.ToString());
            data.Add("time_pause", program_settings.t_pause.ToString());
            data.Add("number_of_attempts", program_settings.num_of_attempts.ToString());
            data.Add("allow_digit", program_settings.digitization.ToString());
            data.Add("allow_auto_calculate", program_settings.calculation.ToString());

            data.Add("template_color_num", program_settings.template_color_num.ToString());
            data.Add("template_line_width", program_settings.template_line_width.ToString());
            data.Add("measurement_color_num", program_settings.measurement_color_num.ToString());
            data.Add("measurement_line_width", program_settings.measurement_line_width.ToString());
            data.Add("grid_color_num", program_settings.grid_color_num.ToString());
            data.Add("grid_line_width", program_settings.grid_line_width.ToString());
            data.Add("axis_color_num", program_settings.axis_color_num.ToString());
            data.Add("axis_line_width", program_settings.axis_line_width.ToString());

            try
            {
                db.Insert("settings", data);
            }
            catch (Exception crap)
            {
                MessageBox.Show(crap.Message);
            }
        }
    }
}
