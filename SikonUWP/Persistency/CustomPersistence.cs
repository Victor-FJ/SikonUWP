using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SikonUWP.Persistency
{
    public class CustomPersistence
    {
        public static async Task<bool> Delete<T>(T key, string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(uri + key + "/");
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(jsonString);
                }

                return false;
            }
        }
    }
}
