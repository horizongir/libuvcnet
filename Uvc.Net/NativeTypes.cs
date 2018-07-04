using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uvc.Net
{
    abstract class UvcHandle : SafeHandle
    {
        internal UvcHandle(bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }

    class UvcContext : UvcHandle
    {
        internal UvcContext()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.uvc_exit(handle);
            return true;
        }
    }

    class UvcDevice : UvcHandle
    {
        internal UvcDevice()
            : base(true)
        {
        }

        internal UvcDevice(IntPtr devh)
            : base(true)
        {
            SetHandle(devh);
            NativeMethods.uvc_ref_device(this);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.uvc_unref_device(handle);
            return true;
        }
    }

    class UvcDeviceHandle : UvcHandle
    {
        internal UvcDeviceHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.uvc_close(handle);
            return true;
        }
    }

    class UvcStreamHandle : UvcHandle
    {
        internal UvcStreamHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.uvc_stream_close(handle);
            return true;
        }
    }

    enum UvcError
    {
        /** Success (no error) */
        UVC_SUCCESS = 0,
        /** Input/output error */
        UVC_ERROR_IO = -1,
        /** Invalid parameter */
        UVC_ERROR_INVALID_PARAM = -2,
        /** Access denied */
        UVC_ERROR_ACCESS = -3,
        /** No such device */
        UVC_ERROR_NO_DEVICE = -4,
        /** Entity not found */
        UVC_ERROR_NOT_FOUND = -5,
        /** Resource busy */
        UVC_ERROR_BUSY = -6,
        /** Operation timed out */
        UVC_ERROR_TIMEOUT = -7,
        /** Overflow */
        UVC_ERROR_OVERFLOW = -8,
        /** Pipe error */
        UVC_ERROR_PIPE = -9,
        /** System call interrupted */
        UVC_ERROR_INTERRUPTED = -10,
        /** Insufficient memory */
        UVC_ERROR_NO_MEM = -11,
        /** Operation not supported */
        UVC_ERROR_NOT_SUPPORTED = -12,
        /** Device is not UVC-compliant */
        UVC_ERROR_INVALID_DEVICE = -50,
        /** Mode not supported */
        UVC_ERROR_INVALID_MODE = -51,
        /** Resource has a callback (can't use polling and async) */
        UVC_ERROR_CALLBACK_EXISTS = -52,
        /** Undefined error */
        UVC_ERROR_OTHER = -99
    }

    enum UvcVsDescriptorSubtype
    {
        UVC_VS_UNDEFINED = 0x00,
        UVC_VS_INPUT_HEADER = 0x01,
        UVC_VS_OUTPUT_HEADER = 0x02,
        UVC_VS_STILL_IMAGE_FRAME = 0x03,
        UVC_VS_FORMAT_UNCOMPRESSED = 0x04,
        UVC_VS_FRAME_UNCOMPRESSED = 0x05,
        UVC_VS_FORMAT_MJPEG = 0x06,
        UVC_VS_FRAME_MJPEG = 0x07,
        UVC_VS_FORMAT_MPEG2TS = 0x0a,
        UVC_VS_FORMAT_DV = 0x0c,
        UVC_VS_COLORFORMAT = 0x0d,
        UVC_VS_FORMAT_FRAME_BASED = 0x10,
        UVC_VS_FRAME_FRAME_BASED = 0x11,
        UVC_VS_FORMAT_STREAM_BASED = 0x12
    };
}
