using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Agent
{
    public class User
    {
        public string LocalPath { get; set; }
        public string Sid { get; set; }
        public string LastUseTime { get; set; }
    }

    public class User2
    {

    }
    public class FromRegistry
    {
        public List<Software> getSoftwareList(List<Software> softwareList)
        {

            List<User> users = new List<User>();
            ManagementObjectSearcher user = new ManagementObjectSearcher("SELECT * FROM Win32_UserProfile");
            foreach (ManagementObject u in user.Get())
            {
                if (u["sid"].ToString().Length > 8)
                {
                    users.Add(new User()
                    {
                        LocalPath = u["localpath"].ToString(),
                        Sid = u["sid"].ToString(),
                        LastUseTime = u["lastusetime"].ToString(),
                    });
                }

            }


            // string path = "C:\\Users\\sergey.nikitin\\NTUSER.DAT";
            // string SID = "S-1-5-21-3338502417-1842584666-2817140955-35579";
            // int interror = RegLoadKey((uint)HKEY.USERS, SID, path);
            string v = NtUserDat.RegistryInterop.Load("C:\\Users\\sergey.nikitin\\NTUSER.DAT");
            NtUserDat.RegistryInterop.Unload();




            //


            string displayName = "";
            RegistryKey key;


            // search in: CurrentUser
            try
            {
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;

                    if (displayName != null && displayName != "")
                    {
                        var obj = softwareList.FirstOrDefault(x => x.Name == displayName);
                        if (obj == null)
                        {
                            softwareList.Add(new Software() { Name = displayName, SrcHku = true });

                        }
                        else obj.SrcHku = true;
                    }

                }
            }
            catch
            {

            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;

                if (displayName != null && displayName != "")
                {
                    var obj = softwareList.FirstOrDefault(x => x.Name == displayName);
                    if (obj == null) softwareList.Add(new Software() { Name = displayName, SrcHklm = true });
                    else obj.SrcHklm = true;
                    // softwareList.Add(new Software() { name = displayName, SrcHklm = true });
                }
            }

            // search in: LocalMachine_64
            try
            {
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;

                    if (displayName != null && displayName != "")
                    {
                        var obj = softwareList.FirstOrDefault(x => x.Name == displayName);
                        if (obj == null) softwareList.Add(new Software() { Name = displayName, SrcHklm = true });
                        else obj.SrcHklm = true;
                        // softwareList.Add(new Software() { name = displayName, SrcHklm = true });
                    }

                }
            }
            catch
            {

            }
            return softwareList;

        }
    }
}