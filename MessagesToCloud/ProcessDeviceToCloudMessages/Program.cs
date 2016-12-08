using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ProcessDeviceToCloudMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHubConnectionString = "HostName=IotLabHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=df+QPsnSz08bgGTzGwEJIVnMC6SSefS1aFk7Q7F0ac0=";
            string iotHubD2cEndpoint = "messages/events";
            StoreEventProcessor.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=iotlabtest;AccountKey=XIhWe0vbsDj8u3JFDXTEssGtjsChfWjX5GK1L+J9vvETEoE1IS5Z/hyhuVx5q3keVcBQxg1wJXpnbcMaNhq9Rg==;";
            StoreEventProcessor.ServiceBusConnectionString = "Endpoint=sb://iotlabservicebus.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=1Iq+gPGIScpbYekgJJ8aUPyNHmHgWHqbrEQmOXEITvA=;EntityPath=d2ctutorial";

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, iotHubD2cEndpoint, EventHubConsumerGroup.DefaultGroupName, iotHubConnectionString, StoreEventProcessor.StorageConnectionString, "messages-events");
            Console.WriteLine("Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<StoreEventProcessor>().Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
