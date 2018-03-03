using System.Runtime.InteropServices;

namespace NetX
{
    [StructLayout(LayoutKind.Explicit, Size = 10)]
    public partial struct NetPacketHeader
    {
        [FieldOffset(0)]
        public uint size;

        [FieldOffset(4)]
        public ushort cmd;

        [FieldOffset(6)]
        public ushort slCode;

        [FieldOffset(8)]
        public NetX.SaveFlags flags;

        public static NetPacketHeader Create(ushort cmd, ushort slCode)
        {
            return new NetPacketHeader()
            {
                cmd = cmd,
                slCode = slCode
            };
        }

        public static NetPacketHeader Empty
        {
            get
            {
                return new NetPacketHeader
                {
                    size = 0,
                    cmd = 0,
                    slCode = 0
                };
            }
        }
    }
}
