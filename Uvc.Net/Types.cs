using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Uvc.Net
{
    public enum FrameFormat
    {
        Unknown = 0,
        /** any supported format */
        Any = 0,
        Uncompressed,
        Compressed,
        /** yuyv/yuv2/yuv422: yuv encoding with one luminance value per pixel and
         * one uv (chrominance) pair for every two pixels.
         */
        Yuyv,
        Uyvy,
        /** 24-bit rgb */
        Rgb,
        Bgr,
        /** motion-jpeg (or jpeg) encoded images */
        Mjpeg,
        /** greyscale images */
        Gray8,
        Gray16,
        /* raw colour mosaic images */
        BY8,
        BA81,
        Sgrbg8,
        Sgbrg8,
        Srggb8,
        Sbggr8
    };

    public struct FormatDescriptor
    {
        public FrameFormat Format;
        public int Width;
        public int Height;
        public int Fps;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StreamControl
    {
        public ushort Hint;
        public byte FormatIndex;
        public byte FrameIndex;
        public uint FrameInterval;
        public ushort KeyFrameRate;
        public ushort FrameRate;
        public ushort CompressionQuality;
        public ushort CompressionWindowSize;
        public ushort Delay;
        public uint MaxVideoFrameSize;
        public uint MaxPayloadTransferSize;
        public uint ClockFrequency;
        public byte FramingInfo;
        public byte PreferredVersion;
        public byte MinVersion;
        public byte MaxVersion;
        public byte InterfaceNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Frame
    {
        public IntPtr Data;
        public IntPtr DataBytes;
        public uint Width;
        public uint Height;
        public FrameFormat FrameFormat;
        public IntPtr Step;
        public uint Sequence;
        public long TvSec;
        public long TvUsec;
        public IntPtr Source;
        public byte LibraryOwnsData;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FrameCallback(ref Frame frame, IntPtr user_ptr);
}
