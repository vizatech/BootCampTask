﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace ReadFileUploadNotification
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=IotLabHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=df+QPsnSz08bgGTzGwEJIVnMC6SSefS1aFk7Q7F0ac0=";
        static void Main(string[] args)
        {
            Console.WriteLine("Receive file upload notifications\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            ReceiveFileUploadNotificationAsync().Wait();
            Console.ReadLine();
        }
        private async static Task ReceiveFileUploadNotificationAsync()
        {
            var notificationReceiver = serviceClient.GetFileNotificationReceiver();

            Console.WriteLine("\nReceiving file upload notification from service");
            while (true)
            {
                var fileUploadNotification = await notificationReceiver.ReceiveAsync();
                if (fileUploadNotification == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received file upload noticiation: {0}", string.Join(", ", fileUploadNotification.BlobName));
                Console.ResetColor();

                await notificationReceiver.CompleteAsync(fileUploadNotification);
            }
        }
    }
}
