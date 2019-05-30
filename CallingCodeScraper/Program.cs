using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CallingCodeScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World Scraping!");
            try
            {
                string dirPath = "./Output";
                string physicalPath = dirPath + "/insert-callingcodes.sql";

                if (!Directory.Exists("./Output"))
                {
                    Directory.CreateDirectory(dirPath);
                }

                using (var fileStream = File.Create(physicalPath))
                {
                    StreamWriter sw = new StreamWriter(fileStream);

                    WriteSql(sw);

                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open file for writing");
                Console.WriteLine(e.Message);
                return;
            }
        }

        private static void WriteSql(StreamWriter sw)
        {
            sw.WriteLine("use pingaccountservicedb;");
            sw.WriteLine("insert into CallingCode (PrefixCode, CountryName, IsoCode) values");

            // Scraper
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://countrycode.org");
            var Tables = doc.DocumentNode
                .SelectNodes("//table");

            if (Tables != null)
            {
                var trows = Tables[0].SelectNodes("tbody/tr");
                for(int i = 0; i < trows.Count; i++)
                {
                    HtmlNode tr = trows[i];
                    sw.Write(
                        "(" +
                        tr.SelectSingleNode("td[2]").InnerText.Replace("-", "") + ", " +
                        "\"" + tr.SelectSingleNode("td[1]").InnerText + "\", " + 
                        "\"" + tr.SelectSingleNode("td[3]").InnerText + "\"" +
                        ")"
                    );

                    if (i < trows.Count - 1)
                    {
                        sw.Write("," + sw.NewLine);
                    } else
                    {
                        sw.Write(";");
                    }
                }
            }
        }
    }
}
