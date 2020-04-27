using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SikonUWP.Persistency
{
    public class ImagePersistence
    {
        private const string Uri = " http://localhost:52415/api/Image/";

        public static async Task<List<string>> GetNames()
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString = await client.GetStringAsync(Uri);
                return JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
        }

        public static async Task<byte[]> Get(string name)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonStringOut = await client.GetStringAsync(Uri + name + "/");
                return JsonConvert.DeserializeObject<byte[]>(jsonStringOut);
            }
        }

        public static async Task<bool> Post(string name, byte[] pixelBytes)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonStringIn = JsonConvert.SerializeObject(pixelBytes);
                HttpResponseMessage response = await client.PostAsync(Uri + name + "/", new StringContent(jsonStringIn, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string jsonStringOut = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(jsonStringOut);
                }
                return false;
            }
        }

        public static async Task<bool> Delete(string name)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(Uri + name + "/");
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
