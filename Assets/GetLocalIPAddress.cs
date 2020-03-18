using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;

namespace Agent
{
    public class GetLocalIPAddress
    {
        public static string GetAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string addresses = "";
            StringCollection ipCol = new StringCollection();
            
            
            foreach (var ip in host.AddressList)
            {
                ipCol.Add(ip.AddressFamily.ToString()+": "+ip.ToString());
                addresses = ip.ToString() + " " + addresses;
                // if (ip.AddressFamily == AddressFamily.InterNetwork)
                // {
                //     return ip.ToString();
                // }
            }
            return addresses.Trim();
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}