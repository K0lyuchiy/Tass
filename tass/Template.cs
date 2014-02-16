using System; 

namespace tass
{
    public class Template
    {
        public float time_step = 0.010F;
        public float delay_before_start = 1;
        public float t_pulse = 3;
        public float t_zero = 1;
        public int num_of_pulses = 5;

        public int length = 0;

        public float time_axis = 0;

        public float[] template_x = new float[30000];
        public float[] template_y = new float[30000];
        public int counter = 0;

        public float A = 20;
        public float k = 0;

        public float getData()
        {
            if (counter == length)
                counter = 0;
            counter+=1;
            return template_y[counter - 1];
        }

        public Template()
        {
            create_template();
        }

        public void shift_template(int step_shift)
        {
            for (int i =0;i<length;i++)
                template_x[i]+= ((float)step_shift)/1000;
        }

        public void create_template()
        {
            time_axis = delay_before_start + (t_pulse + t_zero) * num_of_pulses;
            length = (int)(time_axis / time_step);
            k = A / t_pulse;
            int steps_before = (int)(delay_before_start/time_step);
            int steps_pulse = (int)(t_pulse/time_step);
            int steps_pause = (int)(t_zero / time_step);

            for (int i= 0; i < steps_before; i++)
            {
                template_y[i] = 0;
                template_x[i] = (i*time_step);               
            }
            counter = steps_before;
            for (int i = 0; i < num_of_pulses; i++)
            {
                for (int j = 0; j < steps_pulse; j++)
                {
                    template_y[counter + j] = (k * j * time_step);
                    template_x[counter + j] = ((counter + j) * time_step);
                }
                counter = counter + steps_pulse;
                for (int j = 0; j < steps_pulse; j++)
                {
                    template_y[counter + j] = 0;
                    template_x[counter + j] = ((counter + j) * time_step);
                }
                counter = counter + steps_pause;
            }
            counter = 0;     
        }
    }
}
