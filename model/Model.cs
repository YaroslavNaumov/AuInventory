using System;
using System.Collections.Generic;

namespace Agent
{
// public class Source
// {
//     public bool HKLM ;
//     public bool HKU;
//     public bool WMI;
//     public Source()
//     {
//         HKLM = false;
//         HKU = false;
//         WMI = false;
//     }
// }

public class Software
{
    public string name { get; set; }
    public string publisher { get; set; }
    public string version { get; set; }
    public string installationDirectory { get; set; }
    public string uninstallString { get; set; }
    public string installed { get; set; }
    // public Source source { get; set; }
    public bool src_HKU = false ;
    public bool src_HKLM = false ;
    public bool src_WMI = false ;
}

public class Video
{
    public string type { get; set; }
    public string name { get; set; }
}

public class Storage
{
    public string model { get; set; }
    public string type { get; set; }
    public string @interface { get; set; }
    public string serial { get; set; }
}

public class Network
{
    public string name { get; set; }
    public string Manufacturer { get; set; }
    public string MAC { get; set; }
    public string adapter_type { get; set; }
    public string netConnectionID { get; set; }
    public string netConnectionStatus { get; set; }
    public List<string> IP_address { get; set; }
    public string IP_subnet { get; set; }
    public string defaultGateway { get; set; }
    public List<string> DNS { get; set; }
    public string DHCP { get; set; }
    public string DHCPserver { get; set; }
    public string DNSdomain { get; set; }
    public string DNShostName { get; set; }
}

public class Hardware
{
    public List<string> processor { get; set; }
    public string motherboard { get; set; }
    public List<string> ram { get; set; }
    public List<Video> video { get; set; }
    public List<Storage> storage { get; set; }
    public List<string> printers { get; set; }
    public List<Network> network { get; set; }
}

public class Datum
{
    public string GUID { get; set; }
    public string ipAddress { get; set; }
    public string operationSystem { get; set; }
    public string computerName { get; set; }
    public string activeUser { get; set; }
    public List<Software> software { get; set; }
    public Hardware hardware { get; set; }
    public string agentLocation { get; set; }
    public string agentVersion { get; set; }
}

public class RootObject
{
    public string id { get; set; }
    public string timestamp { get; set; }
    public List<Datum> data { get; set; }
}
}