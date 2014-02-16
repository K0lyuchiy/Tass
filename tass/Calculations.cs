using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tass
{
    class Calculations
    {
        public int []time_force_array;
        public int threshold = 10;

        public int[] get_smoothed_data(int length, float[] array)
        {
            int LEV = 5;
            int[] smoothed_array = new int[length];
            for (int i = LEV; i < (length - LEV+1); i++ )
            {
                int temp_val = 0;
                for (int m = i - LEV; m < (i + LEV + 1); m++)
                {
                    temp_val = (int)(temp_val + array[m]);
                }
                smoothed_array[i] = (int)(temp_val/(LEV*2+1));
            }
            return smoothed_array;
        }
        
        public int[] get_derivative(int length, int[] array)
        {
            int [] derivative = new int[length];
            for (int i = 0; i < (length - 3); i++)
            {
                derivative[i] = (int)((array[i + 3] - array[i]) / 3);
            }
            return derivative;
        }

        public int[] get_points(int length, int[] array)
        {
            int[] vlazh_points = new int[3000];
            int i = 0;
            int []der = get_derivative(length,  array);
            int m = 1;
            vlazh_points[0] = 1;
            while (i < length)
            {
                if (der[i] > 0)//2
                {
                    vlazh_points[m] = (short)i;
                    m++;
                    for (int l = i; l < length; l++)
                    {
                        if (der[l] <= -3)//-5
                        {
                            vlazh_points[m] = (short)(l - 1);
                            m++;
                            for (int n = l - 1; n < length; n++)
                            {
                                if (der[n] == 0)
                                {
                                    i = n;
                                    break;
                                }
                            }
                            break;
                        }
                    }                    
                }
                else
                    i++;
            }
            return vlazh_points;
        }

        public short[] get_max_min(int length, short[] array)
        {
            short[] max_min = new short[50]; 
            int j = 0;
            int k = 0;

            int m = 0;

            j = get_count_incr(j, array, length);
            if (j != -1)
            {
                while (j < length)
                {
                    k = get_count_decr(j, array, length);
                    if (k != -1)
                    {
                        max_min[m] = (short)get_max(j, k, array);
                        m++;
                        j = get_count_incr(k + 5, array, length);
                        if (j != -1)
                        {
                            max_min[m] = (short)get_min(k + 5, j, array);
                            m++;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
            } 
            return max_min;
        }

        public int get_count_incr(int count, short[] array, int length)
        {
            for (int i = count; i < length; i++)
            {
                if (array[i] >= threshold)
                {
                    return i;
                }
            }
            return -1;
        }

        public int get_count_decr(int count, short[] array, int length)
        {
            for (int i = count; i < length; i++)
            {
                if (array[i] <= threshold)
                {
                    return i;
                }
            }
            return -1;
        }

        public  int get_max(int j, int k, short[] array)
        {
            int max = array[j];
            int idx_max = j;
            for (int i = j; i < k; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                    idx_max = i;
                }
            }
            return idx_max;
        }

        public int get_min(int j, int k, short[] array)
        {
            int min = array[j];
            int idx_min = j;
            for (int i = j; i < k; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                    idx_min = i;
                }
            }
            return idx_min;
        }
    }
}
