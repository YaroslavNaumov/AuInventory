using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Agent
{
public class Agent
{
    public long id { get; set; }
    public string guid { get; set; }
    public List<Datum> data { get; set; }

    public Agent()
    {
        data = new List<Datum>();
    }
}

public class Datum
{
    // public string Guid { get; set; }
    public string timestamp  { get; set; }

    public string ipAddress { get; set; }
   
    [JsonPropertyName("os")]
    public string operationSystem { get; set; }
    public string msOperationSystemKey { get; set; }
    public string computerName { get; set; }
    // public string ActiveUser { get; set; }
    public List<Software> software { get; set; }
    public string agentLocation { get; set; }
    public string agentVersion { get; set; }

    public Datum()
    {
        // Console.WriteLine("Datum Class Constructor");
    }
}

public class Software
{
    public string name { get; set; }
    public string publisher { get; set; }
    public string version { get; set; }
    public string installationDirectory { get; set; }
    public string installed { get; set; }
    public string comment { get; set; }

    public Software()
    {
  
    }
}
}