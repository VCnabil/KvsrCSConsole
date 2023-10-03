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
        // When called CheckForError will check for and print any error.
        // Return true if an error has occured.
        static public bool CheckForError(string cmd, Canlib.canStatus stat)
        {
            if (stat != Canlib.canStatus.canOK)
            {
                Canlib.canGetErrorText(stat, out string buf);
                Console.WriteLine("[{0}] {1}: failed, stat={2}", cmd, buf, (int)stat);
                return true;
            }
            return false;
        }
        // ListChannels prints a list of all connected CAN interfaces.
        static public void ListChannels()
        {
            Canlib.canStatus stat;
            // Get number channels
            stat = Canlib.canGetNumberOfChannels(out int number_of_channels);
            if (CheckForError("canGetNumberOfChannels", stat))
                return;
            Console.WriteLine("Found {0} channels", number_of_channels);
            // Loop and print all channels
            for (int i = 0; i < number_of_channels; i++)
            {
                stat = Canlib.canGetChannelData(i, Canlib.canCHANNELDATA_DEVDESCR_ASCII, out object device_name);
                if (CheckForError("canGetChannelData", stat))
                    return;
                stat = Canlib.canGetChannelData(i, Canlib.canCHANNELDATA_CHAN_NO_ON_CARD, out object device_channel);
                if (CheckForError("canGetChannelData", stat))
                    return;
                Console.WriteLine("Found channel: {0} {1} {2}", i, device_name, ((UInt32)device_channel + 1));
            }
        }

        private static void doListChanels()
        {
            Canlib.canInitializeLibrary();
            ListChannels();
            //  Press any key to continue
            Console.ReadKey(true);
        }
        // The check method takes a canStatus (which is an enumerable) and the method
        // name as a string argument. If the status is an error code, it will print it.
        // Most Canlib method return a status, and checking it with a method like this
        // is a useful practice to avoid code duplication.
        private static void CheckStatus(Canlib.canStatus status, string method)
        {
            if (status < 0)
            {
                string errorText;
                Canlib.canGetErrorText(status, out errorText);
                Console.WriteLine(method + " failed: " + errorText);
            }
        }
        private static void DoSendMessage()
        {
            // Holds a handle to the CAN channel
            int handle;
            // Status returned by the Canlib calls
            Canlib.canStatus status;
            // The CANlib channel number we would like to use
            int channelNumber = 0;
            // The msg will be the body of the message we send on the CAN bus.
            byte[] msg = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Initializing Canlib");
            // Initialize the Canlib library with a call to
            // Canlib.initializeLibrary(). This always needs to be done before
            // doing anything with the library.
            Canlib.canInitializeLibrary();
            Console.WriteLine("Opening channel {0}", channelNumber);
            // Next, we open up the channel and receive a handle to
            // it. Depending on what devices you have connected to your
            // computer, you might want to change the channel number. The
            // canOPEN_ACCEPT_VIRTUAL flag means that it is ok to open the
            // selected channel, even if it is on a virtual device.
            handle = Canlib.canOpenChannel(channelNumber, Canlib.canOPEN_ACCEPT_VIRTUAL);
            CheckStatus((Canlib.canStatus)handle, "canOpenChannel");
            Console.WriteLine("Setting channel bitrate");
            // Once we have successfully opened a channel, we need to set its bitrate. We
            // do this using canSetBusParams. CANlib provides a set of predefined bus parameter
            // settings in the form of canBITRATE_XXX constants. For other desired bus speeds
            // bus paramters have to be set manually.
            // See CANlib documentation for more information on parameter settings.
            status = Canlib.canSetBusParams(handle, Canlib.canBITRATE_250K, 0, 0, 0, 0);
            CheckStatus(status, "canSetBusParams");
            Console.WriteLine("Going on bus");
            // Next, take the channel on bus using the canBusOn method. This
            // needs to be done before we can send a message.
            status = Canlib.canBusOn(handle);
            CheckStatus(status, "canBusOn");
            Console.WriteLine("Writing a message to the channel");
            // We send the message using canWrite. This method takes five
            // parameters: the channel handle, the message identifier, the
            // message body, the message length (in bytes) and optional flags.
            status = Canlib.canWrite(handle, 123, msg, 8, 0);
            CheckStatus(status, "canWrite");
            Console.WriteLine("Waiting for the message to be transmitted");
            // After sending, we wait for at most 1000 ms for the message to be sent, using
            // canWriteSync.
            status = Canlib.canWriteSync(handle, 1000);
            CheckStatus(status, "canWriteSync");
            Console.WriteLine("Going off bus");
            // Once we are done using the channel, we go off bus using the
            // canBusOff method. It take the handle as the only argument.
            status = Canlib.canBusOff(handle);
            CheckStatus(status, "canBusOff");
            Console.WriteLine("Closing channel {0}", channelNumber);
            // We also close the channel using the canCloseChannel method,
            // which take the handle as the only argument.
            status = Canlib.canClose(handle);
            CheckStatus(status, "canClose");
            // Wait for the user to press a key before exiting, in case the
            // console closes automatically on exit.
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }
        static void Main(string[] args)
        {
            doListChanels();

            // DoSendMessage();
        }
    }
}
