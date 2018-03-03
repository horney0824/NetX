using System;

namespace NetX
{
    public struct NetBuffer
    {
#if ENABLE_NET_DIAGNOSTICS
    public int      index;
#endif

        public byte[] byteArray;
        public int offset;
        public int size;

        public NetBuffer Begin
        {
            get
            {
                return new NetBuffer
                {
                    byteArray = byteArray,
                    offset = 0,
                    size = byteArray != null ? byteArray.Length : 0
                };
            }
        }

        public static NetBuffer Create(byte[] byteArray)
        {
            return new NetBuffer()
            {
                byteArray = byteArray,
                offset = 0,
                size = byteArray != null ? byteArray.Length : 0
            };
        }

        public static NetBuffer Empty
        {
            get
            {
                return new NetBuffer
                {
                    byteArray = null,
                    offset = 0,
                    size = 0
                };
            }
        }

        public void Consume(int bytes)
        {
            offset += bytes;
            size -= bytes;
        }

        public byte[] Extract()
        {
            var extractedByteArray = new byte[size];

            Buffer.BlockCopy(byteArray, offset, extractedByteArray, 0, size);

            return extractedByteArray;
        }
    }

}
