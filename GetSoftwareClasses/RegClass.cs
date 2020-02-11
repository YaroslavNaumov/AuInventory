using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Agent
{
    public class RegClass
    {
        public List<Software> getSoftwareList(List<Software> softwareList)
        {
            string displayName = "";
            RegistryKey key;

            // List<Software> softwareList = new List<Software>();


            // search in: CurrentUser
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;

                if (displayName != null && displayName != "")
                {
                    // Console.WriteLine($"CurrentUser | {displayName}");
                    foreach(Software software in softwareList){
                        if(software.name == displayName)
                        {
                            software.src_HKU=true;
                        }
                        else softwareList.Add(new Software() { name = displayName, src_HKU=true});
                    }
                }
                // if (p_name.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
                // {
                //     // return true;
                // }
            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;

                if (displayName != null && displayName != "")
                {
                    // foreach(Software software in softwareList){
                        if(softwareList.Exists(x => x.name == displayName))
                        {
                            // x.src_HKLM=true;
                        }
                        else softwareList.Add(new Software() { name = displayName, src_HKLM=true});
                    // }
                    // softwareList.Add(new Software() { name = displayName, src_HKLM=true});
                }
            }

            // search in: LocalMachine_64
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;

                if (displayName != null && displayName != "")
                {
                    foreach(Software software in softwareList){
                        if(software.name == displayName)
                        {
                            software.src_HKLM=true;
                        }
                        else softwareList.Add(new Software() { name = displayName, src_HKLM=true});
                    }
                    // softwareList.Add(new Software() { name = displayName, src_HKLM=true});
                }

            }
            return softwareList;

        }
    }
}