using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Agent
{
    public class CapWMI
    {
        public List<Software> getSoftwareList(List<Software> softwareList)
        {
            // List<Software> softwareList = new List<Software>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            if(mos.Get().Count>0) 

            {
                Console.WriteLine("mos.Get().Count "+mos.Get().Count);
            
            }


            foreach (ManagementObject mo in mos.Get())
            {
                Software obj = null;
                string moName=mo["Name"].ToString();
                Console.WriteLine("FirstOrDefault 1");
                obj = softwareList.FirstOrDefault(x => x.name == moName );
                Console.WriteLine("FirstOrDefault 2");
                if (obj == null)
                {
                    Console.WriteLine("FirstOrDefault 3");
                    softwareList.Add(obj = new Software());
                    obj.name = mo["Name"].ToString();
                    obj.version = mo["Version"].ToString();
                    obj.publisher = mo["Vendor"].ToString();
                    if (mo["InstallLocation"] != null) obj.installationDirectory = mo["InstallLocation"].ToString();
                    obj.installed = mo["InstallDate"].ToString();
                    obj.comment = "WMI";

                }
                else
                {
                    Console.WriteLine("FirstOrDefault 4");
                    obj.installed = mo["InstallDate"].ToString();
                    if (obj.installationDirectory == "" && mo["InstallLocation"] != null)
                    {
                        Console.WriteLine("FirstOrDefault 5");
                        obj.installationDirectory = mo["InstallLocation"].ToString();
                        obj.comment += " WMI";
                    }
                }
            }
            return softwareList;
        }
    }
}