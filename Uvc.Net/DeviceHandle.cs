using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uvc.Net
{
    public class DeviceHandle : IDisposable
    {
        readonly UvcDeviceHandle handle;
        FrameCallback cb;

        internal DeviceHandle(UvcDeviceHandle deviceHandle)
        {
            handle = deviceHandle;
        }

        public IEnumerable<FormatDescriptor> GetStreamControlFormats()
        {
            var descs = NativeMethods.uvc_get_format_descs(handle);
            while (descs != IntPtr.Zero)
            {
                // access frame_descs field
                var descriptorSubType = (UvcVsDescriptorSubtype)Marshal.ReadInt32(descs, IntPtr.Size * 3);
                var numFrameDescriptors = Marshal.ReadByte(descs, IntPtr.Size * 3 + 5);
                if (numFrameDescriptors > 0)
                {
                    var frameDescs = Marshal.ReadIntPtr(descs, IntPtr.Size * 3 + 29);
                    while (frameDescs != IntPtr.Zero)
                    {
                        var width = (int)(ushort)Marshal.ReadInt16(frameDescs, IntPtr.Size * 3 + 6);
                        var height = (int)(ushort)Marshal.ReadInt16(frameDescs, IntPtr.Size * 3 + 8);
                        yield return new FormatDescriptor { Width = width, Height = height };

                        // access next field
                        frameDescs = Marshal.ReadIntPtr(frameDescs, IntPtr.Size * 2);
                    }
                }

                // access next field
                descs = Marshal.ReadIntPtr(descs, IntPtr.Size * 2);
            }

            yield break;
        }

        public StreamControl GetStreamControlFormatSize(FrameFormat format, int width, int height, int fps)
        {
            StreamControl control;
            GetStreamControlFormatSize(format, width, height, fps, out control);
            return control;
        }

        public void GetStreamControlFormatSize(FrameFormat format, int width, int height, int fps, out StreamControl control)
        {
            var error = NativeMethods.uvc_get_stream_ctrl_format_size(handle, out control, format, width, height, fps);
            UvcException.ThrowExceptionForUvcError(error);
        }

        public StreamControl ProbeStreamControl()
        {
            StreamControl control;
            ProbeStreamControl(out control);
            return control;
        }

        public void ProbeStreamControl(out StreamControl control)
        {
            var error = NativeMethods.uvc_probe_stream_ctrl(handle, out control);
            UvcException.ThrowExceptionForUvcError(error);
        }

        public void StartStreaming(ref StreamControl control, FrameCallback callback)
        {
            var error = NativeMethods.uvc_start_streaming(handle, ref control, callback, IntPtr.Zero, 0);
            UvcException.ThrowExceptionForUvcError(error);
            cb = callback;
        }

        public void StartIsoStreaming(ref StreamControl control, FrameCallback callback)
        {
            var error = NativeMethods.uvc_start_iso_streaming(handle, ref control, callback, IntPtr.Zero);
            UvcException.ThrowExceptionForUvcError(error);
            cb = callback;
        }

        public void StopStreaming()
        {
            NativeMethods.uvc_stop_streaming(handle);
            cb = null;
        }

        public void Dispose()
        {
            handle.Dispose();
        }
    }
}
