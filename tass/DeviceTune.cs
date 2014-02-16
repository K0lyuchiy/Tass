using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tass
{
    public class DeviceTune
    {
        public float zero_level = 0;
        public float max_level = 0;
        public float tarirovka_weight = 0;
        public double me_threshold = 3;
        public double rmse_threshold = 4;
        public double trend_error = 0.35;
        public float step = 1;

        public bool BALANCE_DONE = false;
        public bool TARIROVKA_DONE = false;

        public DeviceTune()
        {

        }       

        public float scale_measurements(float measurement, bool sceale_results)
        {
            if (sceale_results)
                if (measurement<=zero_level)
                    return 0;
                else
                    return ((short)((measurement - ((short)zero_level)) * step));
            else
                return measurement;
        }
        
        // функция проверки выполнения балансировки
        public bool balance_check(float [] ch_buffer, int ch_count)
        {
            if (check_ME(ch_buffer, ch_count) && check_D(ch_buffer, ch_count))
            {
                // запись нулевого уровня показания прибора в настройки
                zero_level = get_zero_level(ch_buffer, ch_count);
                BALANCE_DONE = true;
                return true;
            }
            else
            {
                BALANCE_DONE = false;
                return false;
            }
        }

        // функция проверки выполнения балансировки
        public bool tarirovka_check(float[] ch_buffer, int ch_count)
        {
            if (check_trend(ch_buffer, ch_count) && check_D(ch_buffer, ch_count))
            {
                // запись нулевого уровня показания прибора в настройки
                max_level = get_max_level(ch_buffer, ch_count);
                TARIROVKA_DONE = true;
                return true;
            }
            else
            {
                TARIROVKA_DONE = false;
                return false;
            }
        }

        private bool check_trend(float[] ch_buffer, int ch_count)
        {
            int interval = ch_count / 3;
            int residual = ch_count - interval * 3;
            int t1 = interval;
            int t2 = t1;
            int t3 = t2 + residual;

            double me1 = getME(ch_buffer.Skip(0).Take(t1).ToArray(), t1);
            double me2 = getME(ch_buffer.Skip(t1).Take(t2).ToArray(), t2);
            double me3 = getME(ch_buffer.Skip(t2).Take(t3).ToArray(), t3);

            if ((Math.Abs((me1 - me2) / me1) < trend_error) && (Math.Abs((me2 - me3) / me2) < trend_error) && (Math.Abs((me1 - me3) / me3) < trend_error))
                return true;
            else
                return false;
        }

        private float getME(float[] ch_buffer, int ch_count)
        {
            float sum = 0;
            for (int i = 0; i < ch_count; i++)
            {
                sum = sum + ch_buffer[i];
            }
            float me_value = ((float)(((double)sum) / ((double)ch_count)));
            return me_value;
        }

        private double get_rmse(float[] ch_buffer, int ch_count)
        {
            double sum = 0;
            double me_ = getME(ch_buffer,  ch_count);
            for (int i = 0; i < ch_count; i++)
            {
                double temp_sum = (ch_buffer[i] - me_)*(ch_buffer[i] - me_);
                sum = sum + temp_sum;
            }
            double rmse_value = Math.Sqrt(sum / (ch_count  -1));
            return rmse_value;
        }

        private bool check_ME(float[] ch_buffer, int ch_count)
        {
            double me_value = getME(ch_buffer, ch_count);
            if (me_value < me_threshold)
                return true;
            else
                return false;
        }

        private bool check_D(float[] ch_buffer, int ch_count)
        {
            double rmse_ = get_rmse(ch_buffer, ch_count);
            if (rmse_ < rmse_threshold)
                return true;
            else
                return false;
        }

        private float get_max_level(float[] ch_buffer, int ch_count)
        {
            return getME(ch_buffer, ch_count);
        }

        private float get_zero_level(float[] ch_buffer, int ch_count)
        {
            float largest_value = -1;
            var l_val_idx = -1;
            for (var i = 0; i < ch_count; i++)
            {
                if (ch_buffer[i] > largest_value)
                {
                    l_val_idx = i;
                    largest_value = ch_buffer[i];
                }
            }
            return largest_value;
        }
    }
}
