using Grpc.Core;
using MagicOnion.Server;
using System;
using System.Threading;
using static System.Console;

namespace UnityRresearch.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);

            var server = new Server
            {
                Services = { service },
                Ports = { new ServerPort("0.0.0.0", 56392, ServerCredentials.Insecure) }
            };

            server.Start();

            WriteLine("Service started");

            while (true)
            {
                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }
    }
}