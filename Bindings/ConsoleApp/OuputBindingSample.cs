using Dapr.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class OuputBindingSample : Example
    {
        private static readonly string bindingName = "bindingeventdemo";

        // Allow ouput binding operations: create, get, delete, list
        private static readonly string operation = "create";

        public override string DisplayName => "Using de Output Binding";

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            using var client = new DaprClientBuilder( )
                .UseHttpEndpoint("http://localhost:3500")
                .UseGrpcEndpoint("http://localhost:50000")
              .Build();

            var data = new WeatherForecast()
            {
                Date = DateTime.Now,
                TemperatureC = 12,
                Summary = "Sunny",
                Data = Encoding.UTF8.GetBytes("二进制数据传输，当 kubectl 基于非 ASCII 或 UTF-8 的输入创建 ConfigMap 时， 该工具将这些输入放入 ConfigMap 的 binaryData 字段，而不是 data 中。 同一个 ConfigMap 中可同时包含文本数据和二进制数据源。")

            };

            await client.InvokeBindingAsync(
                            bindingName,
                            operation,
                            data,
                            cancellationToken: cancellationToken);

            Console.WriteLine("Message has been sent !");
        }
    }
}
