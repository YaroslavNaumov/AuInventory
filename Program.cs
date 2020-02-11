using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Software> softwareList = new List<Software>();

            var reg = new RegClass();
            // List<Software> allsoft = reg.getSoftwareList();
            softwareList = reg.getSoftwareList(softwareList);
            var wmi = new WmiClass();
            // List<Software> allsoft2 = wmi.getSoftwareList();
            softwareList = wmi.getSoftwareList(softwareList);


            Console.WriteLine("########################");
            // foreach (Software aSoft in allsoft)
            int i=1;
            foreach (Software aSoft in softwareList)
            {
                Console.WriteLine(i+"#"+aSoft.Name + "#"+ aSoft.src_HKU+"#"+ aSoft.src_HKLM+"#"+ aSoft.src_WMI);
                i++;
            }
            Console.WriteLine("########################");
            

            string url = "http://localhost:3000/posts";

            using (var webClient = new WebClient())
            {
                var pars = new NameValueCollection();
                pars.Add("key", "value");
                pars.Add("key2", "value2");
                pars.Add("key3", "value3");

                try
                {
                    var response = webClient.UploadValues(url, pars);
                    // Console.WriteLine(Encoding.ASCII.GetString(response));
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.GetType().Name);
                }
            }

        }
    }
}