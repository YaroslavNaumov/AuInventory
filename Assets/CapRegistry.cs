using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace Agent
{
    public class User
    {
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string LocalPath { get; set; }
        public string Sid { get; set; }
        public string LastUseTime { get; set; }
    }

    public class CapRegistry
    {
        public List<Software> getSoftwareList(bool HKLM)
        {

            Console.WriteLine("Start registry capture");
            List<Software> softwareList = new List<Software>();

            RegistryKey rk = null;
            List<User> users = new List<User>();
            // search in: Users
            ManagementObjectSearcher user = new ManagementObjectSearcher("SELECT * FROM Win32_UserProfile");

            // ManagementObject o = new ManagementObject("Win32_SID.SID='S-1-5-21-3338502417-1842584666-2817140955-41203'");

            // S-1-5-21-3338502417-1842584666-2817140955-41203
            // Console.WriteLine("Do get user");
            foreach (ManagementObject u in user.Get())
            {
            // Console.WriteLine("Get user");


                if (u["sid"].ToString().Length > 8)
                {
                    string sid = u["sid"].ToString();
                    bool mount = false;
                    ManagementObject usr = new ManagementObject($"Win32_SID.SID='{sid}'");
                    string n = usr["AccountName"].ToString();
                    string d = usr["ReferencedDomainName"].ToString();

                    //  Console.WriteLine("Add user");
                    users.Add(new User()
                    {
                        LocalPath = u["localpath"].ToString(),
                        Sid = sid,
                        LastUseTime = u["lastusetime"].ToString(),
                    });
                    

                    rk = Registry.Users.OpenSubKey(sid + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                    if (rk == null)
                    {
                        string loadpath = u["localpath"].ToString() + "\\NTUSER.DAT";
                        NtUserDat.RegistryInterop.Load(loadpath, sid);
                        mount = true;
                        rk = Registry.Users.OpenSubKey(sid + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                    }

                    if (rk != null)
                    {
                        foreach (String keyName in rk.GetSubKeyNames())
                        {
                            RegistryKey rkSubkey = rk.OpenSubKey(keyName);
                            string displayName = rkSubkey.GetValue("DisplayName") as string;
                            string displayVersion = rkSubkey.GetValue("displayVersion") as string;
                            string publisher = rkSubkey.GetValue("Publisher") as string;
                            string installLocation = rkSubkey.GetValue("InstallLocation") as string;
                            string InstallDate = rkSubkey.GetValue("InstallDate") as string;
                            string uninstallString = rkSubkey.GetValue("UninstallString") as string;

                            if (displayName != null && displayName != "")
                            {
                                var obj = softwareList.FirstOrDefault(x => x.name == displayName);
                                if (obj == null)
                                {
                                    softwareList.Add(new Software()
                                    {
                                        name = displayName + $" ({d}\\{n})",
                                        // SrcHku = true,
                                        version = displayVersion,
                                        publisher = publisher,
                                        installationDirectory = installLocation,
                                        installed = InstallDate,
                                        comment = "HKU"
                                        // UninstallString = uninstallString,
                                    });

                                }
                                // else obj.SrcHku = true;
                            }
                            rkSubkey.Close();
                        }
                        rk.Close();
                    } //else Console.WriteLine("Registry key is null");
                    if (mount)
                    {
                        NtUserDat.RegistryInterop.Unload(sid);
                    }


                }

            }
            // Console.WriteLine("end HKU get");

            if (HKLM == false) return softwareList;


            softwareList = searchHklm(softwareList, RegistryView.Registry32);
            if (Environment.Is64BitOperatingSystem)
            {
                softwareList = searchHklm(softwareList, RegistryView.Registry64);
            }

            if (softwareList.Count > 0)
                softwareList = softwareList.OrderBy(o => o.name).ToList();

            Console.WriteLine("End registry capture");    
            return softwareList;

        }

        private List<Software> searchHklm(List<Software> softwareList, RegistryView registry)
        {
            RegistryKey rkbase = null;
            rkbase = RegistryKey.OpenBaseKey
                            (Microsoft.Win32.RegistryHive.LocalMachine, registry);

            RegistryKey rk = rkbase.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string keyName in rk.GetSubKeyNames())
            {
                // Console.WriteLine("keyName: "+keyName);
                RegistryKey rkSubkey = rk.OpenSubKey(keyName);
                string displayName = rkSubkey.GetValue("DisplayName") as string;
                string displayVersion = rkSubkey.GetValue("displayVersion") as string;
                string publisher = rkSubkey.GetValue("Publisher") as string;
                string installLocation = rkSubkey.GetValue("InstallLocation") as string;
                string InstallDate = rkSubkey.GetValue("InstallDate") as string;
                string uninstallString = rkSubkey.GetValue("UninstallString") as string;
                
                // проверка обновлений
                var ParentKeyName =  rkSubkey.GetValue("ParentKeyName");
                if(ParentKeyName != null) continue;


                if (displayName != null && displayName != "")
                {
                    var obj = softwareList.FirstOrDefault(x => x.name == displayName);
                    if (obj == null)
                    {
                        softwareList.Add(new Software()
                        {
                            name = displayName,
                            version = displayVersion,
                            publisher = publisher != null ? publisher.Replace("\0", string.Empty) : publisher,
                            installationDirectory = installLocation,
                            installed = InstallDate,
                            comment = registry.ToString()
                        });
                    }
                    else obj.comment += " " + registry.ToString();
                }
                rkSubkey.Close();
            }
            rk.Close();
            return softwareList;
        }
    }

}
