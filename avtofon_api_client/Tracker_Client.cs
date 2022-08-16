using System.Text;
using Newtonsoft.Json;
using System.IO.Compression;

namespace avtofon_api_client
{
    public class Body
    {
        public string mid;
        public string start;
        public string end;
        public Body(string mid, string start, string end)
        {
            this.mid = mid;
            this.start = start;
            this.end = end;
        }
    }

    public class Values
    {
        public string id;
        public string ts;
        public string lat;
        public string lng;
        public string v;
        public string az;
        public string tscrd;
        public string on;
        public string pwr;
        public string gps;
        public string gsm;
        public string epwr;
        public string ipwr;
        public string tmp;
        public string mv;
        public string sat;
        public string battery;
    }
    public class Tracker_Client
    {
        string endPoint = "";
        private readonly HttpClient _client;
        public Tracker_Client()
        {
            _client = new HttpClient();

        }
        public async Task<Values?> GetLastUpdates()
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var result = _client.PostAsync(endPoint, content).Result;

            var buffer = await result.Content.ReadAsByteArrayAsync();

            var unzip = Decompress(buffer);

            var responseString = Encoding.UTF8.GetString(unzip, 0, unzip.Length);

            var obj = JsonConvert.DeserializeObject<Values[]>(responseString);

            return obj?.FirstOrDefault();
        }

        private static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
