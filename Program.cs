using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                Console.WriteLine("Application path: "+appPath);
                // Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                // Ini file
                string configFileName = appPath + "\\config.ini";
                if (!File.Exists(@configFileName))
                {
                    throw new System.ArgumentException("Config file " + configFileName + " not found", appPath);
                    // configFileName = @"config.ini";

                }
                IniFile ini = new IniFile(@configFileName);
                string localStorageFileName = ini.ReadINI("Agent", "localStorageFileName");
                string localStoragePath = appPath + "\\" + localStorageFileName;
                Console.WriteLine("localStoragePath: " + localStoragePath);

                string url = ini.ReadINI("Agent", "url");
                Console.WriteLine("server url: " + url);

                bool validateCert = false;
                validateCert = bool.Parse(ini.ReadINI("Agent", "validateCert"));

                int localStorageSize = int.Parse(ini.ReadINI("Agent", "localStorageSize"));
                Console.WriteLine("localStorageSize: " + localStorageSize.ToString());

                bool createTask = true;
                createTask = bool.Parse(ini.ReadINI("Task", "createTask"));

                string taskName = ini.ReadINI("Task", "taskName");
                Console.WriteLine("taskName: " + taskName);

                int startAtHour = int.Parse(ini.ReadINI("Task", "startAtHour"));
                Console.WriteLine("startAtHour: " + startAtHour.ToString());
                
                int rndMinutes = int.Parse(ini.ReadINI("Task", "rndMinutes"));
                Console.WriteLine("rndMinutes: " + rndMinutes.ToString());

                string nowTimestamp = Convert.ToString((Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
                Console.WriteLine("nowTimestamp: " + nowTimestamp);



                //-start----Task
                if(createTask != false){
                    string exec = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    TaskScheduler task = new TaskScheduler();
                    task.CreateTask(taskName, exec,"send", appPath, startAtHour, rndMinutes);
                }

                //-end------Task


                // Command line args
                string[] arguments = Environment.GetCommandLineArgs();

                string myJsonString = null;
                try
                {
                    using (var reader = File.OpenText(localStoragePath))
                    {
                        myJsonString = reader.ReadToEnd();
                    }
                }
                catch { }
                Agent agent;
                try
                {
                    agent = (Agent)JsonSerializer.Deserialize(myJsonString, typeof(Agent));

                    if (agent.data.Count() >= localStorageSize)
                    {
                        agent.data.RemoveAt(0);
                    }
                    Console.WriteLine("Local data (" + agent.data.Count() + ") :");
                    foreach (var data in agent.data)
                    {
                        Console.WriteLine(data.timestamp);
                    }

                }
                catch
                {
                    agent = new Agent();
                }


                var reg = new CapRegistry();
                List<Software> registrySoftwareList = reg.getSoftwareList(true);
                Console.WriteLine("registrySoftwareList " + registrySoftwareList.Count);


                Dictionary<String, Software> registryHashMap = new Dictionary<String, Software>();
                foreach (Software sw in registrySoftwareList)
                {
                    if (!registryHashMap.ContainsKey(sw.name + sw.publisher))
                    {
                        registryHashMap.Add(sw.name + sw.publisher, sw);
                    }
                }


                var wmi = new CapWMI();
                List<Software> wmiSoftwareList = new List<Software>();
                wmiSoftwareList = wmi.getSoftwareList();
                Console.WriteLine("wmiSoftwareList " + wmiSoftwareList.Count);

                Dictionary<String, Software> wmiHashMap = new Dictionary<String, Software>();
                foreach (Software sw in wmiSoftwareList)
                {
                    if (!wmiHashMap.ContainsKey(sw.name + sw.publisher))
                    {
                        wmiHashMap.Add(sw.name + sw.publisher, sw);
                    }
                }


                Dictionary<String, Software> softwareListhHashMap = new Dictionary<String, Software>(registryHashMap);
                foreach (KeyValuePair<String, Software> kvp in wmiHashMap)
                {
                    if (!softwareListhHashMap.ContainsKey(kvp.Key))
                    {
                        softwareListhHashMap.Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        softwareListhHashMap.GetValueOrDefault(kvp.Key).comment += " wmi";
                    }
                }

                List<Software> softwareList = new List<Software>();

                softwareList.AddRange(softwareListhHashMap.Values);



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

                // DateTime instOs = (ManagementDateTimeConverter.ToDateTime( moOS["Installdate"].ToString()));
                string osInstalled = Convert.ToString(
                    (Int64)
                    (ManagementDateTimeConverter.ToDateTime( moOS["Installdate"].ToString()))
                    .Subtract(new DateTime(1970, 1, 1))
                    .TotalMilliseconds);

                softwareList.Add(new Software() {
                                        name = moOS["Caption"].ToString().TrimEnd(),
                                        publisher = moOS["Manufacturer"].ToString(),
                                        version =  moOS["Version"].ToString().TrimEnd(),
                                        installationDirectory = moOS["WindowsDirectory"].ToString().TrimEnd(),
                                        installed = osInstalled,
                                    });


                agent.data.Add(new Datum()
                {
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
                //string jsonString = JsonSerializer.Serialize(agent, options);

                byte[] json = JsonSerializer.SerializeToUtf8Bytes(agent, options);
                //Console.WriteLine(jsonString);



                if (arguments.Contains("send"))
                {
                    var cli = new WebClient();
                    cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                    try
                    {


                        if (validateCert == false)
                        {
                            Console.WriteLine("config.ini pram: validateCert=false");
                            ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(
                                delegate
                                { return true; }
                                );
                        }
                        else
                        {
                            Console.WriteLine("config.ini pram: validateCert=true");
                        }

                        //string response = cli.UploadString(url, jsonString);
                        byte[] response = cli.UploadData(url, json);
                        string result = System.Text.Encoding.UTF8.GetString(response);

                        Console.WriteLine("================ RESPONSE AREA ================");
                        Console.WriteLine(result);
                        File.AppendAllText(appPath + "\\response.log", "[" + DateTime.Now.ToString() + "] " + result.ToString() + Environment.NewLine);

                        SrvResp objResp;
                        try
                        {
                            objResp = (SrvResp)JsonSerializer.Deserialize(result, typeof(SrvResp));

                            foreach (var item in objResp.arrTimestamps)
                            {
                                Console.WriteLine(item + " - preraring to remove");

                                var dataToRemove = agent.data.SingleOrDefault(s => s.timestamp == item);
                                if (dataToRemove.timestamp != null)
                                {
                                    Console.WriteLine(item + " - removed from local storage");
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

                        File.AppendAllText(appPath + "\\error.log", "[" + DateTime.Now.ToString() + "] " + e.GetType().Name + ": " + e.Message.ToString() + Environment.NewLine);
                        // File.AppendAllText("error.log", "["+ DateTime.Now.ToString() +"] "+e.GetType().Name+"\n");
                        Console.WriteLine("[" + DateTime.Now.ToString() + "] " + e.GetType().Name + ": " + e.Message.ToString());
                        // Console.WriteLine(e.GetType().Name);
                    }
                }


                // Create a file to write to.
                string saveData = JsonSerializer.Serialize(agent, options);
                File.WriteAllText(localStoragePath, saveData);
                Console.WriteLine("end");
                //Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                string appPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                File.AppendAllText(appPath + "\\error.log", "[" + DateTime.Now.ToString() + "] " + e.Message.ToString() + Environment.NewLine);
            }
        }
    }
}