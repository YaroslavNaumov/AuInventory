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
                    obj.Name = mo["Name"].ToString();
                    obj.Version = mo["Version"].ToString();
                    obj.Publisher = mo["Vendor"].ToString();
                    if(mo["InstallLocation"]!=null) obj.InstallationDirectory = mo["InstallLocation"].ToString();
                    // UninstallString = mo["Name"].ToString(),
                    obj.Installed = mo["InstallDate"].ToString();
                    obj.SrcWmi = true;

                }
                else
                {
                    obj.Installed = mo["InstallDate"].ToString();
                    if (obj.InstallationDirectory == "" && mo["InstallLocation"] != null)
                        obj.InstallationDirectory = mo["InstallLocation"].ToString();
                    obj.SrcWmi = true;
                }
            }
            return softwareList;

        }
    }
}