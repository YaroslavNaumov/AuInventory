using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Agent
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            Console.WriteLine(Environment.CurrentDirectory);
            // Ini file
            IniFile ini = new IniFile("config.ini");
            // string localStoragePath = ini.ReadINI("Agent", "localStoragePath");
            string localStorageFileName = ini.ReadINI("Agent", "localStorageFileName");
            string url = ini.ReadINI("Agent", "url");
            string nowTimestamp = Convert.ToString((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);


            // Command line args
            string[] arguments = Environment.GetCommandLineArgs();

            string localStoragePath = Environment.GetEnvironmentVariable("windir") + "\\" + localStorageFileName;

            Console.WriteLine(localStoragePath);
            // Agent agent = new Agent();

            string myJsonString = null;
            try
            {
                // myJsonString = File.ReadAllText(localStoragePath);

                using (var reader = File.OpenText(localStoragePath))
                {
                    myJsonString = await reader.ReadToEndAsync();
                    // return fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                }


            }
            catch { }
            Agent agent;
            try
            {
                agent = (Agent)JsonSerializer.Deserialize(myJsonString, typeof(Agent));
            }
            catch
            {
                agent = new Agent();
            }


            List<Software> softwareList = new List<Software>();
            var reg = new CapRegistry();
            softwareList = reg.getSoftwareList(softwareList, true);
            var wmi = new CapWMI();
            softwareList = wmi.getSoftwareList(softwareList);



            //Convert install date
            foreach (Software sw in softwareList)
            {
                if (sw.installed != null)
                {
                    sw.installed =
                    Convert.ToString(
                        (Int64)(DateTime
                        .ParseExact(sw.installed, "yyyyMMdd", CultureInfo.InvariantCulture)
                        .Subtract(new DateTime(1970, 1, 1)))
                        .TotalMilliseconds
                        );
                }
                else
                {
                    if (sw.installationDirectory != null)
                    {

                        DateTime dt = Directory.GetCreationTime(sw.installationDirectory);
                        // Console.WriteLine(dt);
                        // Console.WriteLine((Int64)dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
                        sw.installed = Convert.ToString((Int64)dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
                    }
                    // else sw.installed = nowTimestamp;
                    // Console.WriteLine(sw.name);
                }
            }
            // Console.WriteLine(Convert.ToString((Int64)(DateTime.ParseExact("20200302", "yyyyMMdd", CultureInfo.InvariantCulture).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds));


            ManagementObject moOS = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>().FirstOrDefault();
            ManagementObject moGuid = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct").Get().OfType<ManagementObject>().FirstOrDefault();
            // for(var i =1; i<700 ; i++){



            agent.data.Add(new Datum()
            {
                // timestamp = DateTime.Now.ToFileTime(), //132267579376458433  
                // timestamp = (DateTime.Now.ToFileTime()/100000).ToString(), //132267579376458433  
                timestamp = nowTimestamp,
                ipAddress = GetLocalIPAddress.GetAddress(),
                operationSystem = moOS["Caption"].ToString().TrimEnd(),
                //получение ключа продукта windows
                msOperationSystemKey = KeyDecoder.GetWindowsProductKeyFromRegistry(),
                computerName = moOS["CSName"].ToString(),
                software = softwareList,
                agentLocation = "",
                agentVersion = "",
            });



            // }

            agent.guid = moGuid["UUID"].ToString();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // IgnoreNullValues = true, //	Возвращает или задает значение, определяющее, пропускаются ли значения null во время сериализации и десериализации. Значение по умолчанию — false.
                // Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            string jsonString = JsonSerializer.Serialize(agent, options);
            // Console.WriteLine(jsonString);



            if (arguments.Contains("send"))
            {
                var cli = new WebClient();
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                try
                {
                    string response = cli.UploadString(url, jsonString);

                    Console.WriteLine("================ RESPONSE AREA ================");
                    // Console.WriteLine(response);

                    SrvResp objResp;
                    try
                    {
                        objResp = (SrvResp)JsonSerializer.Deserialize(response, typeof(SrvResp));

                        foreach (var item in objResp.arrTimestamps)
                        {
                            Console.WriteLine(item);

                            var dataToRemove = agent.data.SingleOrDefault(s => s.timestamp == item);
                            if (dataToRemove.timestamp != null)
                            {
                                Console.WriteLine("Removed - " + item);
                                agent.data.Remove(dataToRemove);
                            }
                        }

                    }
                    catch
                    {
                        objResp = new SrvResp();
                    }

                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.GetType().Name);
                }
            }


            // Create a file to write to.
            string saveData = JsonSerializer.Serialize(agent, options);
            File.WriteAllText(localStoragePath, saveData);
            Console.WriteLine("end");

        }
    }
}