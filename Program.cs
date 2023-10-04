using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kvaser.CanLib;
using System.Threading;
namespace KVSR_Console_framework_4p7p2
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            CanManager canManager = new CanManager();

            canManager.ListChannels();
            canManager.OpenChannel(0);
            canManager.SetBusParams();
            canManager.GoOnBus();

            byte[] message = { 0, 1, 2, 3, 4, 5, 6, 7 };
            canManager.SendMessage(0xFF, message);
            for (int i = 0; i < 10; i++)  // Loop 10 times
            {
                message[0] = (byte)i;
                canManager.SendMessage(0x18FF8C12, message);
                Thread.Sleep(1000);  // Wait for 1 second (1000 milliseconds)
            }
            //  canManager.ReceiveMessage();

            canManager.GoOffBus();
            canManager.CloseChannel();
        }
    }
}
