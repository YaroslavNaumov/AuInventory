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
            foreach (ManagementObject mo in mos.Get())
            {
                var obj = softwareList.FirstOrDefault(x => x.name == mo["Name"].ToString());
                if (obj == null)
                {
                    // softwareList.Add(new Software() { Name = mo["Name"].ToString(),  = true });
                    softwareList.Add(obj = new Software());
                    // {
                    //     Name = mo["Name"].ToString(),
                    //     Version = mo["Version"].ToString(),
                    //     Publisher = mo["Vendor"].ToString(),
                    //     InstallationDirectory = mo["InstallLocation"].ToString(),
                    //     // UninstallString = mo["Name"].ToString(),
                    //     Installed = mo["InstallDate"].ToString(),
                    //     SrcWmi = true,
                    // });
                    obj.name = mo["Name"].ToString();
                    obj.version = mo["Version"].ToString();
                    obj.publisher = mo["Vendor"].ToString();
                    if(mo["InstallLocation"]!=null) obj.installationDirectory = mo["InstallLocation"].ToString();
                    // UninstallString = mo["Name"].ToString(),
                    obj.installed = mo["InstallDate"].ToString();
                    // obj.SrcWmi = true;

                }
                else
                {
                    obj.installed = mo["InstallDate"].ToString();
                    if (obj.installationDirectory == "" && mo["InstallLocation"] != null)
                        obj.installationDirectory = mo["InstallLocation"].ToString();
                    // obj.SrcWmi = true;
                }
            }
            return softwareList;

        }
    }
}