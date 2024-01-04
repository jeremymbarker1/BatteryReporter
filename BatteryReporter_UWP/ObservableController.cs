using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.HumanInterfaceDevice;

namespace BatteryReporter_UWP
{
    internal class ObservableController : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _battery;
        public string Battery
        {
            get => _battery;
            set => SetProperty(ref _battery, value);
        }
        private string _latency;
        public string Latency
        {
            get => _latency;
            set => SetProperty(ref _latency, value);
        }

        public string Id { get; }
        public HidDevice HidDevice { get; }

        private (int, int) battery_from_report; // (current, max)

        private readonly Stopwatch stopwatch;
        private double time_since_last_update_ms;
        private int ticks_since_last_update;

        public ObservableController(string id, HidDevice hid_device)
        {
            Id = id;

            HidDevice = hid_device;
            hid_device.InputReportReceived += OnInputReportRecieved;

            stopwatch = new Stopwatch();
            time_since_last_update_ms = 0d;
            ticks_since_last_update = 0;
        }

        ~ObservableController()
        {
            HidDevice.InputReportReceived -= OnInputReportRecieved;
        }

        public void UpdateObservableData()
        {
            Battery = string.Format("{0}/{1}🔋", battery_from_report.Item1, battery_from_report.Item2);
            Latency = string.Format("{0:F2} ms", time_since_last_update_ms / ticks_since_last_update);

        }

        private void OnInputReportRecieved(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            if (sender == null) return;

            if (stopwatch.IsRunning)
            {
                time_since_last_update_ms += stopwatch.Elapsed.TotalMilliseconds;
                ticks_since_last_update++;
                stopwatch.Restart();
            }
            else
            {
                stopwatch.Start();
            }

            var input_report = args.Report;
            var data = input_report.Data.ToArray();

            // dualshock 4
            if (sender.VendorId == 1356 && sender.ProductId == 2508)
            {
                // tuple of (current, max)
                //   usb:      0-11
                //   wireless: 0-8
                battery_from_report = data[32] > 16 ? (data[32] - 16, 11) : (data[32], 8);
            }
        }
    }
}