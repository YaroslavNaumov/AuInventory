using System;
using System.Management;

namespace Agent
{
    public class WmiTimeConverter
    {
        public static string Cnv(String date)
        {

        String res = Convert.ToString(
                    (Int64)
                    (ManagementDateTimeConverter.ToDateTime(date))
                    .Subtract(new DateTime(1970, 1, 1))
                    .TotalMilliseconds);
            return res;
        }
    }
}