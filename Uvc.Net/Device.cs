using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uvc.Net
{
    public class Device : IDisposable
    {
        readonly UvcDevice handle;
        readonly ushort vendorId;
        readonly ushort productId;
        readonly ushort complianceLevel;
        readonly string serialNumber;
        readonly string manufacturer;
        readonly string product;

        internal Device(UvcDevice device)
        {
            handle = device;
            IntPtr descriptor;
            var error = NativeMethods.uvc_get_device_descriptor(handle, out descriptor);
            UvcException.ThrowExceptionForUvcError(error);
            try
            {
                vendorId = (ushort)Marshal.ReadInt16(descriptor);
                productId = (ushort)Marshal.ReadInt16(descriptor, 2);
                complianceLevel = (ushort)Marshal.ReadInt16(descriptor, 4);
                serialNumber = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(descriptor, 6));
                manufacturer = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(descriptor, 6 + IntPtr.Size));
                product = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(descriptor, 6 + IntPtr.Size * 2));
            }
            finally { NativeMethods.uvc_free_device_descriptor(descriptor); }
        }

        public ushort VendorId
        {
            get { return vendorId; }
        }

        public ushort ProductId
        {
            get { return productId; }
        }

        public ushort ComplianceLevel
        {
            get { return complianceLevel; }
        }

        public string SerialNumber
        {
            get { return serialNumber; }
        }

        public string Manufacturer
        {
            get { return manufacturer; }
        }

        public string Product
        {
            get { return product; }
        }

        public DeviceHandle Open()
        {
            UvcDeviceHandle devh;
            var error = NativeMethods.uvc_open(handle, out devh);
            UvcException.ThrowExceptionForUvcError(error);
            return new DeviceHandle(devh);
        }

        public void Dispose()
        {
            handle.Dispose();
        }
    }
}
