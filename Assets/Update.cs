// using Internal;
using System.Net;
using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Agent
{
    public class Updater
    {

        public bool Update(string srvUrl)
        {
            Console.WriteLine("Check agent update");
            var client = new WebClient();
            string srvVer = client.DownloadString(srvUrl + "/agent/md5");
            if (!IsMD5(srvVer))
            {
                Console.WriteLine("It is impossible to get information about the new version");
                return false;
            }
            // Console.WriteLine("Agent version on server - " + srvVer);

            string localVer = GetMD5(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            // Console.WriteLine("Agent local version - " + localVer);
            string updateFileName = "WinAgent.update";

            if (!localVer.Equals(srvVer))
            {
                Console.WriteLine("New version aviable\r\nStart download new version");
                try
                {
                    client.DownloadFile(srvUrl + "/WinAgent.exe", updateFileName);
                    Console.WriteLine("Download complite");
                }
                catch (Exception e)
                {
                    if (File.Exists(updateFileName)) { File.Delete(updateFileName); }
                    Console.WriteLine(e);
                    return false;
                }

                if (GetMD5(updateFileName).Equals(srvVer))
                {

                    TaskScheduler ts = new TaskScheduler();
                    string appPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    ts.CreateTask("SWCTRL updater",
                                appPath + "\\Updater.exe",
                                "\"" + updateFileName + "\" \"" + Process.GetCurrentProcess().ProcessName + "\"",
                                appPath,
                                DateTime.Now.Hour,
                                DateTime.Now.Minute + 5
                                );
                    // Process updater = Process.Start("Updater.exe", "\"" + updateFileName + "\" \"" + Process.GetCurrentProcess().ProcessName + "\"");
                    // System.Environment.Exit(1);
                }
                else
                {
                    File.Delete(updateFileName);
                    return false;
                }

            }
            else
            {
                Console.WriteLine("No new version aviable");

            }



            return true;
        }

        private string GetMD5(string filename)
        {
            var md5 = MD5.Create();
            // string filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            var stream = File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            // string localVer = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        private static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }
            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
    }
}