using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tass
{
    public class ProgramSettings
    {
        public string name_of_set_template;
        public short num_of_set_template;

        public float f_max = 100;
        public float time_incr = 3;
        public float t_pause = 1;
        public short t_power = 1;
        public short num_of_attempts = 5;
        public bool digitization = false;
        public bool calculation = false;

        public float calibr_weight =0;

        public float zero_level = 0;
        public float max_level = 0;

        public float step = 1;

        public short num_of_template = 0;
        // Цвет и толщина линий
        public int template_color_num = 0;
        public int template_line_width = 0;

        public int measurement_color_num = 0;
        public int measurement_line_width = 0;

        public int grid_color_num = 0;
        public int grid_line_width = 0;

        public int axis_color_num = 0;
        public int axis_line_width = 0;

        public void setSettings(string name, short num, float m_f_max, short m_t_power, short m_num_of_attempts, bool m_digitization, bool m_calculation)
        {
            name_of_set_template = name;
            num_of_set_template = num;
            f_max = m_f_max;
            t_power = m_t_power;
            num_of_attempts = m_num_of_attempts;
            digitization = m_digitization;
            calculation = m_calculation;
        }

        public void calulate_scale()
        {
            if (zero_level != 0 && max_level != 0)
                step = ((float)(((double)calibr_weight*10 )/ ((double)((double)max_level - zero_level))));           
        }

        public void set_parameters(ref DeviceTune device)
        {
            calibr_weight = device.tarirovka_weight;
            max_level = device.max_level;
            zero_level = device.zero_level;
            calulate_scale();
            device.step = step;
        }
    }
}
