using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using System.Windows.Forms;

using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;

// To use the WinRT APIs, add two references:
// C:\Program Files(x86)\Windows Kits\10\UnionMetadata\Windows.winmd
// C:\Program Files(x86)\Reference Assemblies\Microsoft\Framework.NETCore\v4.5\System.Runtime.WindowsRuntime.dll

namespace BLE_App
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // Start the program
           // var program = new Program();

            // Close on key press
            //Console.ReadLine();
        }

       
    }
}
