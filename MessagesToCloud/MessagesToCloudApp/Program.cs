using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesToCloudApp
{
    class Program
    {
        static string connectionString = "HostName=IotLabHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=df+QPsnSz08bgGTzGwEJIVnMC6SSefS1aFk7Q7F0ac0=";
        static string iotlabqueueEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotlabqueueEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());
        }
        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            }
        }
    }
}
