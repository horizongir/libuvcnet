using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uvc.Net
{
    public class Context : IDisposable
    {
        readonly UvcContext handle;

        public Context()
        {
            var error = NativeMethods.uvc_init(out handle, IntPtr.Zero);
            UvcException.ThrowExceptionForUvcError(error);
        }

        void OnFrame(ref Frame frame, IntPtr ptr)
        {
            Console.WriteLine(frame.Sequence);
        }

        public Device FindDevice(int vendorId = 0, int productId = 0, string serialNumber = null)
        {
            UvcDevice device;
            var error = NativeMethods.uvc_find_device(handle, out device, vendorId, productId, serialNumber);
            UvcException.ThrowExceptionForUvcError(error);
            return new Device(device);
        }

        public IEnumerable<Device> FindDevices(int vendorId = 0, int productId = 0, string serialNumber = null)
        {
            IntPtr devices;
            var error = NativeMethods.uvc_find_devices(handle, out devices, vendorId, productId, serialNumber);
            UvcException.ThrowExceptionForUvcError(error);
            try
            {
                int i = 0;
                IntPtr devh;
                while ((devh = Marshal.ReadIntPtr(devices, IntPtr.Size * i++)) != IntPtr.Zero)
                {
                    var device = new UvcDevice(devh);
                    yield return new Device(device);
                }
            }
            finally { NativeMethods.uvc_free_device_list(devices, 1); }
        }

        public IEnumerable<Device> GetDevices()
        {
            IntPtr devices;
            var error = NativeMethods.uvc_get_device_list(handle, out devices);
            UvcException.ThrowExceptionForUvcError(error);
            try
            {
                int i = 0;
                IntPtr devh;
                while ((devh = Marshal.ReadIntPtr(devices, IntPtr.Size * i++)) != IntPtr.Zero)
                {
                    var device = new UvcDevice(devh);
                    yield return new Device(device);
                }
            }
            finally { NativeMethods.uvc_free_device_list(devices, 1); }
        }

        public void TestDevices()
        {
            UvcDevice device;
            var err = NativeMethods.uvc_find_device(handle, out device, 0, 0, null);
            UvcException.ThrowExceptionForUvcError(err);

            UvcDeviceHandle devh;
            err = NativeMethods.uvc_open(device, out devh);
            UvcException.ThrowExceptionForUvcError(err);
            Console.WriteLine(err);

            StreamControl ctrl;
            err = NativeMethods.uvc_get_stream_ctrl_format_size(devh, out ctrl, FrameFormat.Any, 640, 480, 120);
            Console.WriteLine(ctrl.KeyFrameRate);

            err = NativeMethods.uvc_start_streaming(devh, ref ctrl, OnFrame, IntPtr.Zero, 0);
            Console.ReadLine();

            NativeMethods.uvc_stop_streaming(devh);

            NativeMethods.uvc_print_diag(devh, IntPtr.Zero);

            devh.Close();
        }

        public void Dispose()
        {
            handle.Dispose();
        }
    }
}
