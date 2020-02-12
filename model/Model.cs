using System;
using System.Collections.Generic;

namespace Agent
{
public partial class Agent
{
    public long Id { get; set; }
    public long Timestamp { get; set; }
    public string Guid { get; set; }
    public List<Datum> Data { get; set; }
}

public partial class Datum
{
    public string GuidPc { get; set; }
    public string IpAddress { get; set; }
    public string OperationSystem { get; set; }
    public string ComputerName { get; set; }
    public string ActiveUser { get; set; }
    public List<Software> Software { get; set; }
    public Hardware Hardware { get; set; }
    public string AgentLocation { get; set; }
    public string AgentVersion { get; set; }
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
    public string Name { get; set; }
    public string Publisher { get; set; }
    public string Version { get; set; }
    public string InstallationDirectory { get; set; }
    public string UninstallString { get; set; }
    public string Installed { get; set; }
    public Dictionary<string, bool> Source { get; set; }
    public bool SrcHku { get; set; }
    public bool SrcHklm { get; set; }
    public bool SrcWmi { get; set; }
}
}