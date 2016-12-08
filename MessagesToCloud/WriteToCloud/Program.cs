
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.IO;

namespace WriteToCloud
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "IotLabHub.azure-devices.net";
        static string deviceKey = "l+0kLmb8F1J72Gu78ChtMGL6fUUNgZAeQwhIxZFKu/U=";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("FirstTaskDevice", deviceKey));

            SendDeviceToCloudMessagesAsync();
            SendDeviceToCloudInteractiveMessagesAsync();
            ReceiveC2dAsync();
            SendToBlobAsync();
            Console.ReadLine();
        }
        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = "FirstTaskDevice",
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Task.Delay(1000).Wait();
            }
        }
        private static async void SendDeviceToCloudInteractiveMessagesAsync()
        {
            while (true)
            {
                var interactiveMessageString = "Alert message!";
                var interactiveMessage = new Message(Encoding.ASCII.GetBytes(interactiveMessageString));
                interactiveMessage.Properties["messageType"] = "interactive";
                interactiveMessage.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(interactiveMessage);
                Console.WriteLine("{0} > Sending interactive message: {1}", DateTime.Now, interactiveMessageString);

                Task.Delay(10000).Wait();
            }
        }
        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
        private static async void SendToBlobAsync()
        {
            string fileName = "azure-logo.jpg";
            Console.WriteLine("Uploading file: {0}", fileName);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (var sourceData = new FileStream(@"azure-logo.jpg", FileMode.Open))
            {
                await deviceClient.UploadToBlobAsync(fileName, sourceData);
            }

            watch.Stop();
            Console.WriteLine("Time to upload file: {0}ms\n", watch.ElapsedMilliseconds);
        }
    }
}
