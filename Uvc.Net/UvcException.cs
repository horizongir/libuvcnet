using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Uvc.Net
{
    [Serializable]
    public class UvcException : Exception
    {
        public UvcException()
        {
        }

        public UvcException(string message)
            : base(message)
        {
        }

        public UvcException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected UvcException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal static void ThrowExceptionForUvcError(UvcError error)
        {
            if (error != UvcError.UVC_SUCCESS)
            {
                var msgPtr = NativeMethods.uvc_strerror(error);
                var message = Marshal.PtrToStringAnsi(msgPtr);
                throw new UvcException(message);
            }
        }
    }
}
