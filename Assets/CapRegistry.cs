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

        public List<Software> getSoftwareList(List<Software> softwareList, bool HKLM)
        {
            RegistryKey rk;
            List<User> users = new List<User>();
            // search in: Users
            ManagementObjectSearcher user = new ManagementObjectSearcher("SELECT * FROM Win32_UserProfile");

            // ManagementObject o = new ManagementObject("Win32_SID.SID='S-1-5-21-3338502417-1842584666-2817140955-41203'");

            // S-1-5-21-3338502417-1842584666-2817140955-41203
            foreach (ManagementObject u in user.Get())
            {
                if (u["sid"].ToString().Length > 8)
                {
                    string sid = u["sid"].ToString();
                    Boolean mount = false;
                    ManagementObject usr = new ManagementObject($"Win32_SID.SID='{sid}'");
                    string n = usr["AccountName"].ToString();
                    string d = usr["ReferencedDomainName"].ToString();

                    users.Add(new User()
                    {
                        LocalPath = u["localpath"].ToString(),
                        Sid = sid,
                        LastUseTime = u["lastusetime"].ToString(),
                    });
                    // Console.WriteLine(User);

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


            var rand = new Random();

            if(HKLM == false) return softwareList;

            // search in: LocalMachine_32
            rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
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
                            name = displayName,
                            // SrcHku = true,
                            version = displayVersion,
                            publisher = publisher,
                            installationDirectory = installLocation,
                            installed = InstallDate,
                            comment = "LocalMachine_32"
                            // UninstallString = uninstallString,
                        });

                    }
                    // else obj.SrcHklm = true;
                    // softwareList.Add(new Software() { name = displayName, SrcHklm = true });
                }
                rkSubkey.Close();
            }
            rk.Close();

            // search in: LocalMachine_64
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
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
                                name = displayName,
                                // SrcHku = true,
                                version = displayVersion,
                                publisher = publisher,
                                installationDirectory = installLocation,
                                installed = InstallDate,
                                comment = "LocalMachine_64"
                                // UninstallString = uninstallString,
                            });

                        }
                        // else obj.SrcHklm = true;
                        // softwareList.Add(new Software() { name = displayName, SrcHklm = true });
                    }
                    rkSubkey.Close();
                }
                rk.Close();
            }
            catch
            {

            }
            return softwareList;
        }
    }
}