using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Agent
{
    public class FromWMI
    {
        public List<Software> getSoftwareList(List<Software> softwareList)
        {
            // List<Software> softwareList = new List<Software>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject mo in mos.Get())
            {
                var obj = softwareList.FirstOrDefault(x => x.Name == mo["Name"].ToString());
                if (obj == null) 
                softwareList.Add(new Software() { Name = mo["Name"].ToString(), src_WMI = true });
                else obj.src_WMI = true;
            }
            return softwareList;

        }
    }
}