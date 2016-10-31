using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using ServicesInterfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClient
{
    class Program
    {
        static int Main(string[] args)
        {
            var config = ClientConfiguration.LocalhostSilo();
            try
            {
                InitializeWithRetries(config, initializeAttemptsBeforeFailing: 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

                Console.ReadLine();
                return 1;
            }

            DoClientWork().Wait();
            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();
            return 0;
        }

        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }

        private static async Task DoClientWork()
        {
            var grainFactory = GrainClient.GrainFactory;

            var e1 = grainFactory.GetGrain<IEmployee>(Guid.NewGuid());
            var e2 = grainFactory.GetGrain<IEmployee>(Guid.NewGuid());
            var e3 = grainFactory.GetGrain<IEmployee>(Guid.NewGuid());
            var e4 = grainFactory.GetGrain<IEmployee>(Guid.NewGuid());


            var e0 = GrainClient.GrainFactory.GetGrain<IEmployee>(Guid.NewGuid());
            var m1 = GrainClient.GrainFactory.GetGrain<IManager>(Guid.NewGuid());

            var m0 = grainFactory.GetGrain<IManager>(Guid.NewGuid());

            var m0e = m0.AsEmployee().Result;
            var m1e = m1.AsEmployee().Result;

            m0e.Promote(10).Wait();
            m1e.Promote(11).Wait();

            m0.AddDirectReport(e0).Wait();
            m0.AddDirectReport(e1).Wait();
            m0.AddDirectReport(e2).Wait();

            m1.AddDirectReport(m0e).Wait();
            m1.AddDirectReport(e3).Wait();

            m1.AddDirectReport(e4).Wait();

            m1.AddDirectReport(e0).Wait();

            Console.WriteLine("Client is running.\nPress Enter to terminate...");
            Console.ReadLine();
        }

    }
}
