using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.UI.Xaml;

namespace BatteryReporter_UWP
{
    internal class Main
    {
        public ObservableCollection<ObservableController> Controllers { get; }

        private readonly DispatcherTimer timer;
        private readonly DeviceHelper device_helper;

        public Main()
        {
            Controllers = new ObservableCollection<ObservableController>();

            timer = new DispatcherTimer();
            timer.Tick += OnTimerTick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            device_helper = new DeviceHelper();
        }

        private void OnTimerTick(object sender, object args)
        {
            RefreshDeviceListAsync();
            UpdateControllerData();
        }

        private async void RefreshDeviceListAsync()
        {
            var devices = await device_helper.GetDevicesAsync();
            if (devices == null) return;

            // remove any old devices from collection
            var toRemove = new List<ObservableController>();
            foreach (var controller in Controllers)
            {
                if (!devices.Any(x => x.Id == controller.Id))
                {
                    toRemove.Add(controller);
                }
            }
            foreach (var controller in toRemove)
            {
                Controllers.Remove(controller);
            }


            // add any devices to collection
            foreach (var device in devices)
            {
                if (!Controllers.Any(x => x.Id == device.Id))
                {
                    var controller = new ObservableController(device.Id, await HidDevice.FromIdAsync(device.Id, FileAccessMode.Read))
                    {
                        Name = device.Name,
                        Battery = "???",
                        Latency = "???"
                    };
                    Controllers.Add(controller);
                }
            }
        }

        private void UpdateControllerData()
        {
            foreach (var controller in Controllers)
            {
                controller.UpdateObservableData();
            }
        }
    }
}