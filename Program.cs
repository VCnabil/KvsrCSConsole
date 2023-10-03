using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kvaser.CanLib;
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
            canManager.SendMessage(0x18FF8C00, message);

            canManager.ReceiveMessage();

            canManager.GoOffBus();
            canManager.CloseChannel();
        }
    }
}
