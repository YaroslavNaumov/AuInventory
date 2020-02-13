using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Software> softwareList = new List<Software>();

            var reg = new FromRegistry();
            softwareList = reg.getSoftwareList(softwareList);
            var wmi = new FromWMI();
          //  softwareList = wmi.getSoftwareList(softwareList);


            Datum data =new Datum();
            data.Software=softwareList;

            Console.WriteLine("########################");
            // foreach (Software aSoft in allsoft)
            int i = 1;
            foreach (Software aSoft in softwareList)
            {
                Console.WriteLine(i + "#" + aSoft.Name);
                i++;
            }
            Console.WriteLine("########################");


            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues =true, //	Возвращает или задает значение, определяющее, пропускаются ли значения null во время сериализации и десериализации. Значение по умолчанию — false.
                // Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string jsonString = JsonSerializer.Serialize(data, options);
           // Console.WriteLine(jsonString);

            //получение ключа продукта windows
            string Text = KeyDecoder.GetWindowsProductKeyFromRegistry();
            Console.WriteLine(Text);

            //// web client
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
                    Console.WriteLine(Encoding.ASCII.GetString(response));
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.GetType().Name);
                }
            }
            Console.ReadLine();

        }
    }
}