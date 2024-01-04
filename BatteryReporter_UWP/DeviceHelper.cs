using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;

namespace BatteryReporter_UWP
{
    internal class DeviceHelper
    {
        public async Task<IEnumerable<DeviceInformation>> GetDevicesAsync()
        {
            // References:
            // https://learn.microsoft.com/en-us/uwp/api/windows.devices.humaninterfacedevice.hiddevice?view=winrt-22621
            // https://learn.microsoft.com/en-us/uwp/schemas/appxpackage/how-to-specify-device-capabilities-for-hid

            // https://learn.microsoft.com/en-us/uwp/api/windows.devices.humaninterfacedevice.hidoutputreport?view=winrt-22621

            // HID-Compliant Game Controller
            ushort usage_page = 0x0001;
            ushort usage_id = 0x0005;

            // Create the selector.
            string selector = HidDevice.GetDeviceSelector(usage_page, usage_id);
            //string selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);

            var devices_information = (await DeviceInformation.FindAllAsync(selector)).ToList();
            return devices_information.Where(x => x.Name.Contains("Controller"));
        }
    }
}