using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ChoETL;

namespace vsob.sba_data_cleaner
{
    public static class SbaDataCleaner
    {
        public static void CreateJson()
        {
            StringBuilder sb = new StringBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "\\sample_data.csv");

            using (var p = new ChoCSVReader(path)
                .WithFirstLineHeader()
            )
            {
                using (var w = new ChoJSONWriter(sb))
                    w.Write(p);
            }

            Console.WriteLine(sb.ToString());
        }
    }
    
    
}