using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Agent
{
    public class CapWMI
    {
        public List<Software> getSoftwareList()
        {
            Console.WriteLine("Start WMI capture");
            List<Software> softwareList = new List<Software>();


            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            if (Environment.Is64BitOperatingSystem)
            {
                scope.Options.Context.Add("__ProviderArchitecture", 64);
                scope.Options.Context.Add("__RequiredArchitecture", true);
            }
            scope.Connect();

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            mos.Scope = scope;
            // Console.WriteLine("do Get wmi object");
            if (mos.Get().Count > 0)
            {
                foreach (ManagementObject mo in mos.Get())
                {
                    if (mo["Name"] != null && mo != null)
                    {
                        Software newSoftware = new Software();
                        // Console.WriteLine(mo["Name"].ToString());
                        newSoftware.name = mo["Name"].ToString();
                        if (mo["Version"] != null) newSoftware.version = mo["Version"].ToString();
                        if (mo["Vendor"] != null) newSoftware.publisher = mo["Vendor"].ToString();
                        if (mo["InstallLocation"] != null) newSoftware.installationDirectory = mo["InstallLocation"].ToString();
                        if (mo["InstallDate"] != null) newSoftware.installed = mo["InstallDate"].ToString();

                        newSoftware.comment = "WMI";

                        softwareList.Add(newSoftware);
                    }
                }
                softwareList = softwareList.OrderBy(o => o.name).ToList();
            }

            Console.WriteLine("End WMI capture");
            return softwareList;
        }
    }
}