using System;
using System.Collections.Generic;
using System.Management;

namespace Agent
{
    public class WmiClass
    {
        public List<Software> getSoftwareList(List<Software> softwareList)
        {
            // List<Software> softwareList = new List<Software>();

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject mo in mos.Get())
            {
                // Console.WriteLine("WMI | "+mo["Name"]);
                    foreach(Software software in softwareList){
                        if(software.name == mo["Name"].ToString())
                        {
                            software.src_HKLM=true;
                        }
                        else softwareList.Add(new Software() { name = mo["Name"].ToString(), src_WMI=true});
                    }

            // softwareList.Add(new Software() { name = mo["Name"].ToString(), src_WMI=true});
            }
            return softwareList;

        }
    }
}