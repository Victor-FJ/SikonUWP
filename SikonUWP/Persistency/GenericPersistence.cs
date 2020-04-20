using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SikonUWP.Persistency
{
    public class GenericPersistence<TItem, TKey> where TKey : struct
    {
        private readonly string _uri;

        public GenericPersistence(string uri)
        {
            _uri = uri;
        }

        public async Task<List<TItem>> Get()
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString = await client.GetStringAsync(_uri);
                return JsonConvert.DeserializeObject<List<TItem>>(jsonString);
            }
        }

        public async Task<TItem> Get(TKey key)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonString = await client.GetStringAsync(_uri + "/" + key);
                return JsonConvert.DeserializeObject<TItem>(jsonString);
            }
        }

        public async Task<bool> Post(TItem item)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonStringIn = JsonConvert.SerializeObject(item);
                HttpResponseMessage response = await client.PostAsync(_uri,
                    new StringContent(jsonStringIn, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string jsonStringOut = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(jsonStringOut);
                }

                return false;
            }
        }

        public async Task<bool> Put(TKey key, TItem item)
        {
            using (HttpClient client = new HttpClient())
            {
                string jsonStringIn = JsonConvert.SerializeObject(item);
                HttpResponseMessage response = await client.PutAsync(_uri + "/" + key,
                    new StringContent(jsonStringIn, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string jsonStringOut = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(jsonStringOut);
                }

                return false;
            }
        }

        public async Task<bool> Delete(TKey key)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(_uri + "/" + key);
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
