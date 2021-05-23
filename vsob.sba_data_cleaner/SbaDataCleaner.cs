using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ChoETL;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace vsob.sba_data_cleaner
{
    public static class SbaDataCleaner
    {
        public static void CreateJson()
        {
            var appRoot = AppContext.BaseDirectory.Substring(0,AppContext.BaseDirectory.LastIndexOf("/bin"));
            appRoot = appRoot.Substring(0,appRoot.LastIndexOf("/")+1);
            var path = Path.Combine(appRoot, "vsob.sba_data_cleaner/sample-data.csv");

            JArray records;
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                 records = (JArray)JToken.ReadFrom(reader);
            }


            
            foreach (var record in records)
            {
                foreach (var property in record.Children<JProperty>().ToArray())
                {
                    if (property.Name.StartsWith("View"))
                        property.Remove();
                    if (property.Name.StartsWith("bad"))
                        property.Remove();
                    

                    if (property.Name.StartsWith("Name of Firm"))
                        property.Replace(new JProperty("company_name", property.Value)) ;
                    if (property.Name.StartsWith("Trade Name"))
                        property.Replace(new JProperty("trade_name", property.Value)) ;
                    if (property.Name.StartsWith("unknown"))
                        property.Replace(new JProperty("owner_name", property.Value)) ;
                    if (property.Name.StartsWith("Address, line 1"))
                        property.Replace(new JProperty("address1", property.Value)) ;
                    if (property.Name.StartsWith("Address, line 2"))
                        property.Replace(new JProperty("address2", property.Value)) ;
                    if (property.Name.StartsWith("City"))
                        property.Replace(new JProperty("city", property.Value)) ;
                    if (property.Name.StartsWith("State"))
                        property.Replace(new JProperty("state", property.Value)) ;
                    if (property.Name.StartsWith("Zip"))
                        property.Replace(new JProperty("zip", property.Value));
                    if (property.Name.StartsWith("Capabilities Narrative"))
                        property.Replace(new JProperty("capabilities", property.Value));
                    if (property.Name.StartsWith("CAGE Code"))
                        property.Replace(new JProperty("cage_code", property.Value));
                    if (property.Name.StartsWith("DUNS Number"))
                        property.Replace(new JProperty("duns_number", property.Value));
                    if (property.Name.StartsWith("E-mail Address"))
                        property.Replace(new JProperty("email", property.Value));
                    
                    if (property.Name.StartsWith("In SAM?"))
                        property.Replace(new JProperty("in_sam", property.Value));
                    
                    if (property.Name.StartsWith("WWW Page URL"))
                        property.Replace(new JProperty("url", property.Value));
                }

                var publishDate = Newtonsoft.Json.JsonConvert
                    .SerializeObject(DateTime.Now).Replace("\"", String.Empty);
                ((JObject)record).Add(new JProperty("published_at",
                   publishDate));
                
                ((JObject)record).Add(new JProperty("created_by",
                    "glenn"));
                ((JObject)record).Add(new JProperty("updated_by",
                    ""));
            }
            
            Console.WriteLine(records.ToString());
            
            var writePath = Path.Combine(appRoot, "vsob.sba_data_cleaner/output-data.csv");
            File.WriteAllText(writePath, records.ToString());

           
            
            foreach (var record in records.Children())
            {

                var result = PostAsync((JObject)record).Result;

            }
        }

        static async Task<string> PostAsync(JObject json)
        {
            HttpClient client = new HttpClient();
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MSwiaWF0IjoxNjIxNzk1MDI4LCJleHAiOjE2MjQzODcwMjh9.9r8HGrmqw2vUaPcgck0Rio45iPRXJq_mFqZWtF0wjZA");
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(
                "https://quiet-sands-11562.herokuapp.com/vosbs", content);
           
            var contents = await response.Content.ReadAsStringAsync();
            return contents;
        }
    }
    
    
}