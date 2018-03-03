using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace NetX {
public partial struct NetPacketHeader
{
	[FieldOffset(0)]
	public byte	b00;
	[FieldOffset(1)]
	public byte	b01;
	[FieldOffset(2)]
	public byte	b02;
	[FieldOffset(3)]
	public byte	b03;
	[FieldOffset(4)]
	public byte	b04;
	[FieldOffset(5)]
	public byte	b05;
	[FieldOffset(6)]
	public byte	b06;
	[FieldOffset(7)]
	public byte	b07;
	[FieldOffset(8)]
	public byte	b08;
	[FieldOffset(9)]
	public byte	b09;

	public int Write(byte[] byteArray, int begin, int end) {
		if(end - begin + 1 < 10)
			throw new NoSufficentSpaceException(10, begin, end);
		byteArray[begin + 0] = b00;
		byteArray[begin + 1] = b01;
		byteArray[begin + 2] = b02;
		byteArray[begin + 3] = b03;
		byteArray[begin + 4] = b04;
		byteArray[begin + 5] = b05;
		byteArray[begin + 6] = b06;
		byteArray[begin + 7] = b07;
		byteArray[begin + 8] = b08;
		byteArray[begin + 9] = b09;
		return begin + 10;
	}

	public int Read(byte[] byteArray, int begin, int end) {
		if(end - begin + 1 < 10)
			throw new NoSufficentSpaceException(10, begin, end);
		b00 = byteArray[begin + 0];
		b01 = byteArray[begin + 1];
		b02 = byteArray[begin + 2];
		b03 = byteArray[begin + 3];
		b04 = byteArray[begin + 4];
		b05 = byteArray[begin + 5];
		b06 = byteArray[begin + 6];
		b07 = byteArray[begin + 7];
		b08 = byteArray[begin + 8];
		b09 = byteArray[begin + 9];
		return begin + 10;
	}

	public int ReadPartial(byte[] byteArray, int offset, int begin, int end) {
		if(begin > end)
			throw new NoSufficentSpaceException(1, begin, end);
		int count = 0;
		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 0) {
			b00 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 1) {
			b01 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 2) {
			b02 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 3) {
			b03 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 4) {
			b04 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 5) {
			b05 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 6) {
			b06 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 7) {
			b07 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 8) {
			b08 = byteArray[begin + count];
			count++;
		}

		if(begin + count > end) return begin + count;
		if(offset + count >= 10) return begin + count;
		if(offset <= 9) {
			b09 = byteArray[begin + count];
			count++;
		}

		return begin + count;
	}

	public static int WriteArray(NetPacketHeader[] sValArray, byte[] byteArray, int begin, int end) {
		if(end - begin + 1 < 4) 
			throw new NoSufficentSpaceException(4, begin, end);
		int length = sValArray.Length;
		int size = length * 10;
		byteArray[begin + 0] = (byte)size;
		byteArray[begin + 1] = (byte)(size >> 8);
		byteArray[begin + 2] = (byte)(size >> 16);
		byteArray[begin + 3] = (byte)(size >> 24);
		begin += 4;

		if(end - begin + 1 < size)
			throw new NoSufficentSpaceException(10 * sValArray.Length, begin, end);
		int offset = begin;
		for(int i = 0; i < sValArray.Length; ++i) {
			var value = sValArray[i];
			byteArray[offset + 0] = value.b00;
			byteArray[offset + 1] = value.b01;
			byteArray[offset + 2] = value.b02;
			byteArray[offset + 3] = value.b03;
			byteArray[offset + 4] = value.b04;
			byteArray[offset + 5] = value.b05;
			byteArray[offset + 6] = value.b06;
			byteArray[offset + 7] = value.b07;
			byteArray[offset + 8] = value.b08;
			byteArray[offset + 9] = value.b09;
			offset += 10;
		}
		return offset;
	}

	public static int ReadArray(ref NetPacketHeader[] sValArray, byte[] byteArray, int begin, int end) {
		if(end - begin + 1 < 4) 
			throw new NoSufficentSpaceException(4, begin, end);
		int size = System.BitConverter.ToInt32(byteArray, begin);
		int length = size / 10;
		sValArray = new NetPacketHeader[length];
		begin += 4;

		if(end - begin + 1 < size)
			throw new NoSufficentSpaceException(10 * sValArray.Length, begin, end);
		int offset = begin;
		for(int i = 0; i < sValArray.Length; ++i) {
			var value = new NetPacketHeader();
			value.b00 = byteArray[offset + 0];
			value.b01 = byteArray[offset + 1];
			value.b02 = byteArray[offset + 2];
			value.b03 = byteArray[offset + 3];
			value.b04 = byteArray[offset + 4];
			value.b05 = byteArray[offset + 5];
			value.b06 = byteArray[offset + 6];
			value.b07 = byteArray[offset + 7];
			value.b08 = byteArray[offset + 8];
			value.b09 = byteArray[offset + 9];
			sValArray[i] = value;
			offset += 10;
		}
		return offset;
	}

	public static int WriteList(List<NetPacketHeader>sValArray, byte[] byteArray, int begin, int end) {
		if(end - begin + 1 < 4) 
			throw new NoSufficentSpaceException(4, begin, end);
		int length = sValArray.Count;
		int size = length * 10;
		byteArray[begin + 0] = (byte)size;
		byteArray[begin + 1] = (byte)(size >> 8);
		byteArray[begin + 2] = (byte)(size >> 16);
		byteArray[begin + 3] = (byte)(size >> 24);
		begin += 4;

		if(end - begin + 1 < size)
			throw new NoSufficentSpaceException(10 * sValArray.Count, begin, end);
		int offset = begin;
		for(int i = 0; i < sValArray.Count; ++i) {
			var value = sValArray[i];
			byteArray[offset + 0] = value.b00;
			byteArray[offset + 1] = value.b01;
			byteArray[offset + 2] = value.b02;
			byteArray[offset + 3] = value.b03;
			byteArray[offset + 4] = value.b04;
			byteArray[offset + 5] = value.b05;
			byteArray[offset + 6] = value.b06;
			byteArray[offset + 7] = value.b07;
			byteArray[offset + 8] = value.b08;
			byteArray[offset + 9] = value.b09;
			offset += 10;
		}
		return offset;
	}

	public static int ReadList(List<NetPacketHeader> sValArray, byte[] byteArray, int begin, int end) {
		if(end - begin + 1 < 4) 
			throw new NoSufficentSpaceException(4, begin, end);
		int size = System.BitConverter.ToInt32(byteArray, begin);
		int length = size / 10;
		sValArray.Clear();
		sValArray.Capacity = length;
		begin += 4;

		if(end - begin + 1 < size)
			throw new NoSufficentSpaceException(10 * length, begin, end);
		int offset = begin;
		for(int i = 0; i < length; ++i) {
			var value = new NetPacketHeader();
			value.b00 = byteArray[offset + 0];
			value.b01 = byteArray[offset + 1];
			value.b02 = byteArray[offset + 2];
			value.b03 = byteArray[offset + 3];
			value.b04 = byteArray[offset + 4];
			value.b05 = byteArray[offset + 5];
			value.b06 = byteArray[offset + 6];
			value.b07 = byteArray[offset + 7];
			value.b08 = byteArray[offset + 8];
			value.b09 = byteArray[offset + 9];
			sValArray.Add(value);
			offset += 10;
		}
		return offset;
	}
}

}

