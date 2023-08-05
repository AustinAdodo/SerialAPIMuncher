
using System.Net.Http;
using System;
using Newtonsoft.Json.Linq;

namespace SerialAPIMuncher.Controllers
{
    public class CrunchController : Controller
    {
        // Socket Exhaustion:Each HttpClient instance has its pool of sockets.
        // Creating new instances for each request could exhaust the available sockets.
        //Stale DNS Entries: //builder.Services.AddhttpClient();
        //HttpClient instances cache DNS entries.If the IP address of a service changes, HttpClient might still point to the old address.
        private readonly HttpClient _client;
        public CrunchController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient(); //transient
        }

        [HttpGet("/test1")]
        public async Task<IActionResult> Quest1()
        {
            //string uri = "https://raw.githubusercontent.com/qualified/challenge-data/master/words_alpha.txt";
            string uri = "https://coderbyte.com/api/challenges/json/age-counting";
            HttpResponseMessage response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string s = await response.Content.ReadAsStringAsync();
            //string query = HttpContext.Request.Query["stem"].ToString();
            JObject jsonObject = JObject.Parse(s);
            string dataValue = jsonObject["data"].ToString();
            string[] Pairs = dataValue.Split(',');
            int count = 0;

            foreach (string kvp in Pairs)
            {
                string[] parts = kvp.Trim().Split('=');
                if (parts.Length == 2 && parts[0].Trim() == "age")
                {
                    if (int.TryParse(parts[1].Trim(), out int age) && age >= 50)
                    {
                        count++;
                    }
                }
            }
            string ans = $"{count}hvblwq79c1";
            string result = string.Join("", ans.Select((ele,i) => (i + 1 <= ans.Length - 1 && ((i + 1) % 3 == 0)) ? "X" : ele.ToString()));
            return Ok(result);
        }

        [HttpGet("/test2", Name = "KVP Counter if age!=1")]
        public async Task<IActionResult> Quest2()
        {
            //string jsonResponse = "{\"data\":\"key=IAfpK, age=2, key=WNVdi, age=1, key=jp9zt, age=47, key=jp9zt, age=1\"}";
            //JObject obj = JObject.Parse(s);
            //string data = obj["data"].ToString();
            string uri = "https://coderbyte.com/api/challenges/json/age-counting";
            HttpResponseMessage response = await _client.GetAsync(uri);
            string s = await response.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(s);
            string? data = doc.RootElement.GetProperty("data").GetString();
            string[] arr = data.Split(", ");
            arr = arr.Where(a => a.StartsWith("age=")).ToArray();
            int count = 0;
            int i = 0;
            foreach (string item in arr)
            {
                if (item.StartsWith("age=") && item.Contains("age=1"))
                {
                    arr[i] = "pass";
                }
                ++i;
            }
            i = 0;
            count = arr.Where(a => a != "pass").Count();
            return Ok($"{count}");
        }

        //Retrive Unique Ids from the Getlogs
        static List<string> ExtractUniqueIds(string logs)
        {
            List<string> ids = new List<string>();
            Dictionary<string, int> idCount = new Dictionary<string, int>();
            // Regular expression pattern to match the IDs
            string pattern = @"\?shareLinkId=(\w+)";
            MatchCollection matches = Regex.Matches(logs, pattern);
            foreach (Match match in matches)
            {
                string id = match.Groups[1].Value;
                if (idCount.ContainsKey(id))
                {
                    idCount[id]++;
                }
                else
                {
                    idCount[id] = 1;
                    ids.Add(id);
                }
            }
            // Append :N to IDs that appear more than once
            for (int i = 0; i < ids.Count; i++)
            {
                if (idCount[ids[i]] > 1)
                {
                    ids[i] = $"{ids[i]}:{idCount[ids[i]]}";
                }
            }
            return ids;
        }

        //Dispatch unique Ids and Duplicates.
        [HttpGet("/test3", Name = "KeysFetch")]
        public async Task<IActionResult> Quest3(string[] args)
        {
            try
            {
                List<string> result = new List<string>();
                string logsUrl = "https://coderbyte.com/api/challenges/logs/web-logs-raw";
                HttpResponseMessage response = await _client.GetAsync(logsUrl);
                response.EnsureSuccessStatusCode();
                string logs = await response.Content.ReadAsStringAsync();
                List<string> uniqueIds = ExtractUniqueIds(logs);
                foreach (var id in uniqueIds)
                {
                    result.Add(id);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(404, "Error: " + ex.Message);
            }
        }
    }
}
