using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgram
{
    public class Class1
    {
        private const string URL = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin";
        private static string fileName = @"C:\Temp\Cowin.txt";


        private static async Task Main(string[] args)
        {
            string date = DateTime.Now.ToString("dd-MM-yyyy");
            // Type your username and press enter
            Console.WriteLine("Enter pincode:");

            // Create a string variable and get user input from the keyboard and store it in the variable
            string pincode = Console.ReadLine();
            string urlParameters = $"?pincode={pincode}&date={date}";
            // 560020

            // Create a file to write to.

            // sw.WriteLine("Hello Buddy!!");
            bool foundtheslot = false;
            while (foundtheslot != true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{URL}{urlParameters}");
                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response body.
                        Response a = await response.Content.ReadAsAsync<Response>();
                        //Parallel.ForEach(a.centers, line =>
                        //{
                        foreach (Center line in a.centers)
                        {
                            string[] l = { line.name, line.address };
                            File.AppendAllLines(fileName, l);
                            Console.WriteLine(line.name);
                            foreach (Sessions item in line.sessions)
                            {
                                string datetime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

                                if (item.available_capacity > 0 && item.min_age_limit > 18)
                                {
                                    Log($" {datetime} : Hello, 18+ slot available at {line.name} {line.address}!");
                                    Console.Beep();
                                }
                                else
                                {
                                    Log($" {datetime} : No 18+ Slot on {item.date}");
                                }
                            }
                        };
                    }
                    else
                    {
                        Log(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
                    }
                }
                Thread.Sleep(10000); // Cool Down - hit the API every 10Sec
            }

        }

        private static void Log(string v)
        {
            string hello = v;
            string[] lines = { hello };
            File.AppendAllLines(fileName, lines);
            Console.WriteLine(hello);
        }
    }

    internal class Response
    {
        public Center[] centers { get; set; }


    }

    public class Center
    {
        public int center_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string district_name { get; set; }
        public string block_name { get; set; }
        public Sessions[] sessions { get; set; }
    }

    public class Sessions
    {
        public string date { get; set; }
        public int min_age_limit { get; set; }
        public int available_capacity { get; set; }
    }
}