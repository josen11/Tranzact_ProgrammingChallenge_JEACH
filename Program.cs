using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tranzact_ProgrammingChallenge_JEACH
{
    public class Program
    {
        internal static readonly HttpClient client = new HttpClient();
        internal static List<long> GoogleResult;
        internal static List<long> BingResult;
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No terms were specified for the Search Fight. Please execute again with the search terms.");
                return;
            }
            else
            {
                Console.WriteLine("Let's start a Search Fight....");
                List<long> finalGoogle = await GetResult(args,"Google");
                List<long> finalBing = await GetResult(args, "Bing");
                for (int i = 0; i < args.Length;i++)
                { 
                    Console.WriteLine($"{args[i]}: Google: {finalGoogle[i]} | Bing {finalBing[i]}");
                }

                Console.WriteLine($"Google Winner: {args[finalGoogle.FindIndex(x=> x==finalGoogle.Max())]} ");
                Console.WriteLine($"Bing Winner: {args[finalBing.FindIndex(x => x == finalBing.Max())]} ");
                string winner= finalGoogle.Max() >= finalBing.Max() ? args[finalGoogle.FindIndex(x => x == finalGoogle.Max())]: args[finalBing.FindIndex(x => x == finalBing.Max())];
                Console.WriteLine($"Total Winner: {winner}");
            }
        }
        static async Task <List<long>> GetResult(string[] args, string SearchEngine)
        {
            if (SearchEngine == "Google")
            {
                GoogleResult = new List<long>();
                foreach (string arg in args)
                {
                    var result = await GoogleSearch(arg);
                    GoogleResult.Add(result);
                }
                return GoogleResult;
            }
            else
            {
                BingResult = new List<long>();
                foreach (string arg in args)
                {
                    var result = await BingSearch(arg);
                    BingResult.Add(result);
                }
                return BingResult;
            }
            
        }
        static async Task<long> GoogleSearch(string query)
        {
            long result = 0;
            try
            {
                string uri = GetFromConfiguration("Google.BaseUrl")
                                .Replace("{Key}", GetFromConfiguration("Google.ApiKey"))
                                .Replace("{Context}", GetFromConfiguration("Google.ContextId"))
                                .Replace("{Query}", query);
                string responseBody = await client.GetStringAsync(uri);
                var data = (JObject)JsonConvert.DeserializeObject(responseBody);
                result=(long)data["queries"]["request"][0]["totalResults"];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Caught: {ex.Message}");
            }
            return result;
        }
        static async Task<long> BingSearch(string query)
        {
            long result = 0;
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://bing-web-search1.p.rapidapi.com/search?q=java&mkt=en-us&safeSearch=Off&textFormat=Raw"),
                    Headers =
                    {
                        { "X-BingApis-SDK", "true" },
                        { "X-RapidAPI-Host", GetFromConfiguration("Bing.Host") },
                        { "X-RapidAPI-Key", GetFromConfiguration("Bing.ApiKey") },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var data = (JObject)JsonConvert.DeserializeObject(body);
                    result = (long)data["webPages"]["totalEstimatedMatches"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Caught: {ex.Message}");
            }
            return result;
        }
        public static string GetFromConfiguration(string key)
        {
            string configurationValue = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(configurationValue))
                throw new ConfigurationErrorsException("Missing Value in App.Config: { Key }".Replace("{Key}", key));

            return configurationValue;
        }
    }
}
