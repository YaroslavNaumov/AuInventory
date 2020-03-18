using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Agent1
{
public partial class Agent
{
    public long id { get; set; }
    public string guid { get; set; }
    public List<Datum> data { get; set; }

    public Agent()
    {
        data = new List<Datum>();
    }
}

public partial class Datum
{
    // public string Guid { get; set; }
    public long timestamp  { get; set; }

    public string ipAddress { get; set; }
   
    [JsonPropertyName("os")]
    public string operationSystem { get; set; }
    public string msOperationSystemKey { get; set; }
    public string computerName { get; set; }
    // public string ActiveUser { get; set; }
    public List<Software> software { get; set; }
    public Hardware hardware { get; set; }
    public string agentLocation { get; set; }
    public string agentVersion { get; set; }

    public Datum()
    {
        software = new List<Software>();
    }
}

public partial class Hardware
{
    public List<string> Processor { get; set; }
    public string Motherboard { get; set; }
    public List<long> Ram { get; set; }
    public List<Video> Video { get; set; }
    public List<Storage> Storage { get; set; }
    public List<string> Printers { get; set; }
    public List<Network> Network { get; set; }
}

public partial class Network
{
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string Mac { get; set; }
    public string AdapterType { get; set; }
    public string NetConnectionId { get; set; }
    public string NetConnectionStatus { get; set; }
    public List<string> IpAddress { get; set; }
    public string IpSubnet { get; set; }
    public string DefaultGateway { get; set; }
    public List<string> Dns { get; set; }
    public string Dhcp { get; set; }
    public string DhcPserver { get; set; }
    public string DnSdomain { get; set; }
    public string DnShostName { get; set; }
}

public partial class Storage
{
    public string Model { get; set; }
    public string Type { get; set; }
    public string Interface { get; set; }
    public string Serial { get; set; }
}

public partial class Video
{
    public string Type { get; set; }
    public string Name { get; set; }
}

public partial class Software
{
    public string name { get; set; }
    public string publisher { get; set; }
    public string version { get; set; }
    public string installationDirectory { get; set; }
    // public string UninstallString { get; set; }
    public string installed { get; set; }
    // public Dictionary<string, bool> Source { get; set; }
    // public bool SrcHku { get; set; }
    // public bool SrcHklm { get; set; }
    // public bool SrcWmi { get; set; }
    public string comment { get; set; }
}
}