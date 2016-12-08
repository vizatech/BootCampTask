﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace ProcessD2CInteractiveMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Process D2C Interactive Messages app\n");

            string connectionString = "Endpoint=sb://iotlabservicebus.servicebus.windows.net/;SharedAccessKeyName=listen;SharedAccessKey=JMFPoPE9K42cErf00KByKsl9K6RrRArWFmbTRwYQaV4=;EntityPath=d2ctutorial";
            QueueClient Client = QueueClient.CreateFromConnectionString(connectionString);

            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            Client.OnMessage((message) =>
            {
                try
                {
                    var bodyStream = message.GetBody<Stream>();
                    bodyStream.Position = 0;
                    var bodyAsString = new StreamReader(bodyStream, Encoding.ASCII).ReadToEnd();

                    Console.WriteLine("Received message: {0} messageId: {1}", bodyAsString, message.MessageId);

                    message.Complete();
                }
                catch (Exception)
                {
                    message.Abandon();
                }
            }, options);

            Console.WriteLine("Receiving interactive messages from SB queue...");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
