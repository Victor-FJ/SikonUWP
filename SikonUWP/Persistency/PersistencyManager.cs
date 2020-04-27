using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using Newtonsoft.Json;
using SikonUWP.Common;

namespace SikonUWP.Persistency
{
    public class PersistencyManager
    {
        public const string Uri = "http://localhost:52415/api/Manage/";

        private const string FileName = "DatabaseConnection";

        private static StorageFile _connectionFile;

        public static async Task<bool> Tester()
        {
            try
            {
                if (await GetConnection())
                    return true;
                string connString = await MessageDialogUtil.TextInputDialogAsync(FileName, "Indsæt databasens connection string");
                bool succeeded = await WriteConnection(connString);
                string message = succeeded
                    ? "Fik succesfuldt forbundet med givene string"
                    : "Fejl: Kunne ikke forbinde med givene string";
                await MessageDialogUtil.MessageDialogAsync(FileName, message);
                return succeeded;
            }
            catch (HttpRequestException)
            {
                await MessageDialogUtil.MessageDialogAsync(FileName, "Fejl: Kunne ikke forbinde til rest api'et");
            }
            return false;
        }

        public static async Task<bool> GetConnection()
        {
            _connectionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string connString = await FileIO.ReadTextAsync(_connectionFile);

            return await ValidateConn(connString);
        }

        public static async Task<bool> WriteConnection(string connString)
        {
            bool succeeded = await ValidateConn(connString);
            if (succeeded)
                await FileIO.WriteTextAsync(_connectionFile, connString);
            return succeeded;
        }

        private static async Task<bool> ValidateConn(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                return false;
            using (HttpClient client = new HttpClient())
            {
                string jsonStringIn = JsonConvert.SerializeObject(connectionString);
                HttpResponseMessage response = await client.PostAsync(Uri, new StringContent(jsonStringIn, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string jsonStringOut = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(jsonStringOut);
                }
                return false;
            }

        }
    }
}
