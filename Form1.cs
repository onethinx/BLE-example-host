using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using System.Reflection;
using System.Diagnostics;

// To use the WinRT APIs, add two references:
// C:\Program Files(x86)\Windows Kits\10\UnionMetadata\Windows.winmd
// C:\Program Files(x86)\Reference Assemblies\Microsoft\Framework.NETCore\v4.5\System.Runtime.WindowsRuntime.dll

namespace BLE_App
{
    public partial class Form1 : Form
    {
        const string AppName = "Onethinx BLE Demo App v1.1";

        Thread BLEscanThread;
        int scanningActiveTime;
        Thread connectWatcherThread;
        List<scannedDeviceInfo> scannedDevices;
        bool buttonConnected = false;
        float progressBar1 = 0, progressBar2 = 0;
        bool stopWatcher = false;
        private Thread _animThread;
        private bool _running;

        public static class deviceSpecs
        {
            public static ulong BluetoothAddressFirst = 0x00A050000000;
            public static ulong BluetoothAddressLast = 0x01A05000FFFF;
            public static Guid DataInOutUuid = new Guid("CB7A2F6D-8C08-4F86-8B4C-879F858EF397");
            public static Guid DataOutUuid = new Guid("310FE7D9-C1F5-4880-BFA6-FD1C3507B39D");
            public static Guid DataInUuid = new Guid("70A6F431-7542-44C4-9B45-A3C0D55FC027");
        }

        public static class connectedDevice
        {
            public static BluetoothLEDevice device;
            public static IReadOnlyList<GattDeviceService> GattServices;
            public static GattCharacteristic dataInCharacteristic;
            public static GattCharacteristic dataOutCharacteristic;
        }

        public class GUIitem
        {
            public GUIset GuiSet;
            public string value = "";
            public GUIitem(GUIset GuiSet)
            {
                this.GuiSet = GuiSet;
            }
            public GUIitem(GUIset GuiSet, string value)
            {
                this.value = value;
                this.GuiSet = GuiSet;
            }
        }
        public enum GUIset
        {
            text_black,
            text_red,
            text_green,
            scan_idle,
            scan_active,
            scanlist_clear,
            scanlist_additem,
            connect_unable,
            connect_disconnected,
            connect_connected,
            send_disabled,
            send_enabled,
            in_sent,
            out_received
        }

        public enum BLEcommand
        {
            uart_send = 0x01,
            lorawan_join = 0x10,
            lorawan_send = 0x11,

            host_message = 0x81,
            host_error = 0x82,
            host_beep = 0x83
        }

        public static class BLEBuffer
        {
            public static BLEcommand command;
            public static byte[] data;
            public static byte[] rawdata
            {
                get
                {
                    return (new byte[] { (byte)command }).Concat(data).ToArray();
                }
                set
                {
                    command = (BLEcommand)value[0];
                    data = value.Skip(1).ToArray();
                }
            }
            public static string message
            {
                get
                {
                    return Encoding.UTF8.GetString(data);
                }
                set
                {
                    data = Encoding.ASCII.GetBytes(value);
                }
            }
            public static string hexString
            {
                get
                {
                    return BitConverter.ToString(data);
                }
            }
        }
            
        public class scannedDeviceInfo
        {
            public ulong Address;
            public string Name;
            public string AddressName
            {
                get
                {
                    return $"{Address:X12} {Name}";
                }
            }
        }
        protected virtual void ThreadSafe(MethodInvoker method)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(method);
                else
                    method();
            }
            catch { }
        }

        public Form1()
        {
            InitializeComponent();
            this.Text = AppName;
            SetGUI(new GUIitem(GUIset.text_black, AppName + "\r\n"));
        }

        private void btBLEscan_Click(object sender, EventArgs e)
        {
            
            if (BLEscanThread == null || !BLEscanThread.IsAlive)
                (BLEscanThread = new Thread(() => BLEscanner())).Start();
        }
        private void BLEscanner()
        {
            scannedDevices = new List<scannedDeviceInfo>();
            SetGUI(new GUIitem(GUIset.scanlist_clear));

            // Create Bluetooth Listener
            var watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Only activate the watcher when we're recieving values >= -80
           // watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // Stop watching if the value drops below -90 (user walked away)
           // watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();
            SetGUI(new GUIitem(GUIset.scan_active), new GUIitem(GUIset.text_black, "Scanning for devices..."));
            scanningActiveTime = 100;
            do
            {
                Thread.Sleep(100);

            } while (--scanningActiveTime > 0);
            watcher.Stop();
            SetGUI(new GUIitem(GUIset.scan_idle), new GUIitem(GUIset.text_black, $"Scanning done, total {scannedDevices.Count} devices including {cbScannedDevices.Items.Count} connectable.\r\n"));
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            if (scannedDevices.Select(i => i.Address).Contains(eventArgs.BluetoothAddress)) return;

            bool connectable = eventArgs.BluetoothAddress >= deviceSpecs.BluetoothAddressFirst && eventArgs.BluetoothAddress <= deviceSpecs.BluetoothAddressLast;

            // Tell the user we see an advertisement and print some properties
            SetGUI(new GUIitem(connectable ? GUIset.text_green : GUIset.text_red, $"Advertisement received: {eventArgs.BluetoothAddress:X12} {eventArgs.RawSignalStrengthInDBm:d3}dBi {eventArgs.Advertisement.LocalName}"));

            scannedDeviceInfo foundDevice = new scannedDeviceInfo { Address = eventArgs.BluetoothAddress, Name = eventArgs.Advertisement.LocalName };
            scannedDevices.Add(foundDevice);

            if (!connectable) return;
            SetGUI(new GUIitem(GUIset.scanlist_additem, foundDevice.AddressName));
        }

        private async void btConnect_Click(object sender, EventArgs e)
        {
            /* Start BLE connection watcher thread if not running */
            if (connectWatcherThread == null || !connectWatcherThread.IsAlive) (connectWatcherThread = new Thread(() => connectWatcher())).Start();
            SetGUI(new GUIitem(GUIset.connect_unable));
            if (!buttonConnected) // Connect
            {
                scanningActiveTime = 0; // Stop scanning if active
                ulong t = Convert.ToUInt64(cbScannedDevices.Text.Substring(0, 12), 16);
                bool success = await BLEconnect(t);
                if (success) SetGUI(new GUIitem(GUIset.text_green, "Ready."));
                else SetGUI(new GUIitem(GUIset.text_red, "Connection failed!"), new GUIitem(GUIset.connect_disconnected));
            }
            else // Disconnect
            {
                bool success = await DisconnectDevice();
               // SetGUI(success ? new GUIitem(GUIset.connect_disconnected) : new GUIitem(GUIset.connect_connected));
            }
        }

        private async Task<bool> DisconnectDevice()
        {
            try
            {
                GattCommunicationStatus status = await connectedDevice.dataOutCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                connectedDevice.dataOutCharacteristic.ValueChanged -= Charac_ValueChanged;
                foreach (var ser in connectedDevice.GattServices)
                {
                    ser?.Dispose();
                }
                connectedDevice.device?.Dispose();
                connectedDevice.device = null;
                GC.Collect();
            }
            catch (Exception err)
            {
                SetGUI(new GUIitem(GUIset.text_red, $"Error: {err.Message}"));
                return false;
            }
            return true;
        }
        private void connectWatcher()
        {
            string deviceName = "";
            
            while (!stopWatcher)
            {
                Thread.Sleep(500);
                /* Check active connection */
                if (connectedDevice.device != null && connectedDevice.device.ConnectionStatus != BluetoothConnectionStatus.Disconnected)
                {
                    deviceName = string.Copy(connectedDevice.device.Name);
                    if (!buttonConnected)
                        SetGUI(new GUIitem(GUIset.text_green, $"{deviceName}: Connected"), new GUIitem(GUIset.connect_connected));
                }
                else if (buttonConnected)
                    SetGUI(new GUIitem(GUIset.text_red, $"{deviceName}: Disconnected\r\n"), new GUIitem(GUIset.connect_disconnected), new GUIitem(GUIset.send_disabled));
            }
        }

        private async Task<bool> BLEconnect(ulong BLEaddress)
        {
            try
            {
                connectedDevice.device = await BluetoothLEDevice.FromBluetoothAddressAsync(BLEaddress);

                if (connectedDevice.device == null) throw new Exception($"Unable to connect to {BLEaddress}");

                SetGUI(new GUIitem(GUIset.text_black, $"{connectedDevice.device.Name}: Connecting..."));

                // Read Services
                var gatt = await connectedDevice.device.GetGattServicesAsync();
                if (gatt.Status != GattCommunicationStatus.Success) throw new Exception($"Cannot read services from {BLEaddress}");
                SetGUI(new GUIitem(GUIset.connect_connected), new GUIitem(GUIset.send_enabled), new GUIitem(GUIset.text_black, $"Services: {gatt.Services.Count}, {gatt.Status}"));

                connectedDevice.GattServices = gatt.Services;
                foreach (var service in gatt.Services)  SetGUI(new GUIitem(GUIset.text_black, $"  Service UUID: {service.Uuid}"));

                // Read Characteristics
                var Characteristics = await gatt.Services.Single(s => s.Uuid == deviceSpecs.DataInOutUuid).GetCharacteristicsAsync();
                connectedDevice.dataInCharacteristic = Characteristics.Characteristics.Single(c => c.Uuid == deviceSpecs.DataInUuid);
                connectedDevice.dataOutCharacteristic = Characteristics.Characteristics.Single(c => c.Uuid == deviceSpecs.DataOutUuid);

                // Subscribe to the GATT characteristic's notification
                var status = await connectedDevice.dataOutCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status == GattCommunicationStatus.Success)
                {
                    SetGUI(new GUIitem(GUIset.text_black, "Subscribing to the Indication/Notification"));
                    connectedDevice.dataOutCharacteristic.ValueChanged -= Charac_ValueChanged;
                    connectedDevice.dataOutCharacteristic.ValueChanged += Charac_ValueChanged;
                }
                else
                {
                    throw new Exception($"{status}");
                }
            }
            catch (Exception err)
            {
                SetGUI(new GUIitem(GUIset.text_red, $"Error: {err.Message}"));
                return false;
            }
            return true;
        }

        private void Charac_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            SetGUI(new GUIitem(GUIset.text_green, $"{sender.CharacteristicProperties} received: {reader.UnconsumedBufferLength} bytes"), new GUIitem(GUIset.out_received));

            
            byte[] input = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(input);
            BLEBuffer.rawdata = input;
            switch (BLEBuffer.command)
            {
                case BLEcommand.host_message:
                    SetGUI(new GUIitem(GUIset.text_green, $"{BLEBuffer.message} ({BLEBuffer.hexString})"));
                    break;
                case BLEcommand.host_error:
                    SetGUI(new GUIitem(GUIset.text_red, $"{BLEBuffer.message}"));
                    break;
                case BLEcommand.host_beep:
                    byte tone = BLEBuffer.data[0];
                    byte time = BLEBuffer.data[1];
                    /// <param name="tone">Semitone offset from C4 (0⇒C2, 12⇒C3, etc.).</param>
                    /// <param name="time">Duration in 10 ms units (e.g., 30 ⇒ 300 ms).</param>
                    double baseFreq = 65.41; // C2
                    double freq = baseFreq * Math.Pow(2.0, tone / 12.0);
                    Console.Beep((int)Math.Round(freq), time * 10);
                    break;

            }
        }

        private void btJoin_Click(object sender, EventArgs e)
        {
            BLESend(BLEcommand.lorawan_join, "");
        }

        private void btSendLoRa_Click(object sender, EventArgs e)
        {
            BLESend(BLEcommand.lorawan_send, tbSend.Text);
        }

        private void btSend_Click(object sender, EventArgs e)
        {
            BLESend(BLEcommand.uart_send, tbSend.Text);
        }

        private async void BLESend(BLEcommand command, string message)
        {
            try
            {
                BLEBuffer.message = message;
                BLEBuffer.command = command;
                IBuffer buffer = CryptographicBuffer.CreateFromByteArray(BLEBuffer.rawdata);
                var status = await connectedDevice.dataInCharacteristic.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);
                if (status == GattCommunicationStatus.Success)
                {
                    string MessageString = message.Length > 0 ? $"-'{message}' ({BLEBuffer.hexString})" : "";
                    SetGUI(new GUIitem(GUIset.text_black, $"Success sending: 0x{(byte) command:X2}{MessageString}"), new GUIitem(GUIset.in_sent));
                }
                else throw new Exception($"{status}");
            }
            catch (Exception err)
            {
                SetGUI(new GUIitem(GUIset.text_red, $"Error: {err.Message}"));
            }
        }

        public void SetGUI(params GUIitem[] items)
        {
            ThreadSafe(delegate
            {
                foreach (var item in items)
                {
                    switch (item.GuiSet)
                    {
                        case GUIset.text_black:
                            rtbInfo.AppendText(item.value + "\r\n", Color.Black);
                            break;
                        case GUIset.text_red:
                            rtbInfo.AppendText(item.value + "\r\n", Color.Red);
                            break;
                        case GUIset.text_green:
                            rtbInfo.AppendText(item.value + "\r\n", Color.Green);
                            break;
                        case GUIset.scan_idle:
                            btBLEscan.Image = null;
                            btBLEscan.Text = "Scan for BLE devices";
                            break;
                        case GUIset.scan_active:
                            btBLEscan.Image = global::OTX_BLE_App.Properties.Resources.scanning;
                            btBLEscan.Text = "Scanning...";
                            break;
                        case GUIset.scanlist_clear:
                            if (!buttonConnected)           // Only clear list if not connected
                            {
                                cbScannedDevices.Items.Clear();
                                btConnect.Enabled = false;
                            }
                            break;
                        case GUIset.scanlist_additem:
                            cbScannedDevices.Items.Add(item.value);
                            if (cbScannedDevices.Items.Count == 1)
                            {
                                cbScannedDevices.SelectedIndex = 0;
                                cbScannedDevices.Enabled = true;
                                btConnect.Enabled = true;
                            }
                            break;
                        case GUIset.connect_unable:
                            btConnect.Enabled = false;
                            cbScannedDevices.Enabled = false;
                            break;
                        case GUIset.connect_disconnected:
                            btConnect.Text = "Connect";
                            cbScannedDevices.Enabled = true;
                            btConnect.Enabled = true;
                            break;
                        case GUIset.connect_connected:
                            btConnect.Text = "Disconnect";
                            cbScannedDevices.Enabled = false;
                            btConnect.Enabled = true;
                            break;
                        case GUIset.send_disabled:
                            pnlBLE.Enabled = false;
                            break;
                        case GUIset.send_enabled:
                           // btConnect.Text = "Disconnect";
                            pnlBLE.Enabled = true;
                            break;
                        case GUIset.in_sent:
                           // progressBar1 = pnlProgress.Size.Width;
                           // progressTimer.Enabled = true;
                            StartProgress(true, false);
                            break;
                        case GUIset.out_received:
                            // progressBar2 = pnlProgress.Size.Width;
                            // progressTimer.Enabled = true;
                            StartProgress(false, true);
                            break;
                    }
                }
                buttonConnected = btConnect.Text[0] != 'C';
            });
        }

        private void StartProgress(bool bar1, bool bar2)
        {
            if (bar1) progressBar1 = pnlProgress.Size.Width;
            if (bar2) progressBar2 = pnlProgress.Size.Width;
            if (_running && _animThread != null && _animThread.IsAlive) return; // already running
            pnlProgress.Invoke((MethodInvoker)(() => pnlProgress.Invalidate()));
            _running = true;
            _animThread = new Thread(ProgressLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            _animThread.Start();
        }

        private void ProgressLoop()
        {
            float shrinkStep = 0.005f * pnlProgress.ClientSize.Width;
            while (_running)
            {
                if (progressBar1 > 0f) progressBar1 -= shrinkStep + progressBar1 * 0.03f;
                if (progressBar2 > 0f) progressBar2 -= shrinkStep + progressBar2 * 0.03f;
                pnlProgress.Invoke((MethodInvoker)(() => pnlProgress.Invalidate()));
                if (progressBar1 <= 0f && progressBar2 <= 0f)
                {
                    _running = false;
                    break;
                }
                Thread.Sleep(10);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopWatcher = true;
        }

        private void pnlProgress_Paint(object sender, PaintEventArgs e)
        {
            bool reversed = false;
            using (Brush brsh1 = new SolidBrush(Color.LightBlue))
            using (Brush brsh2 = new SolidBrush(Color.LightSalmon))
            using (Brush brshf = new SolidBrush(Color.White))
            {
                Rectangle rct1 = new Rectangle(0, 0, (int)progressBar1, pnlProgress.Size.Height);
                Rectangle rct2 = new Rectangle(0, 0, (int)progressBar2, pnlProgress.Size.Height);
                e.Graphics.FillRectangle(brshf, pnlProgress.DisplayRectangle);
                if (progressBar1 > 0 && progressBar2 > 0 && progressBar2 > progressBar1) reversed = true;
                if (!reversed && progressBar1 != 0) e.Graphics.FillRectangle(brsh1, rct1);
                if (progressBar2 != 0) e.Graphics.FillRectangle(brsh2, rct2);
                if (reversed && progressBar1 != 0) e.Graphics.FillRectangle(brsh1, rct1);
            }
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }
    }

    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            // Turn on double buffering:
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
    }

}
