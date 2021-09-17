using Dapr.Client;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sample = new OuputBindingSample();
            await sample.RunAsync();
        }
    }
}
