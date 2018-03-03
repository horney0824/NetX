using System.Collections;
using System.Collections.Generic;
using System;

using System.Text;

using System.Runtime.InteropServices;

public class NoSufficentSpaceException: System.Exception
{
    public NoSufficentSpaceException(int requiredSize, int begin, int end):
        base(string.Format("SIZE = {0}, BEGIN = {1}, END = {2}", requiredSize, begin, end))
    {
    }

    public NoSufficentSpaceException(string message) :
        base(message)
    {
    }

    public NoSufficentSpaceException() : base()
    {
    }
}

public static class BitX
{
    public static int SetBool(int value, int where, bool check)
    {
        var mask = ~(1 << where);

        int v = check ? 1 : 0;

        var newValue = (value & mask) | (v << where);

        return newValue;
    }

    public static bool GetBool(int value, int where)
    {
        var mask = 1 << where;

        var result = value & mask;

        return result != 0;
    }

    public static byte SaturateAdd(byte value, int add, byte max = byte.MaxValue)
    {
        int retValue = (int)value + add;

        if(retValue > max)
        {
            retValue = max;
        }
        else if(retValue < 0)
        {
            retValue = 0;
        }

        return (byte)retValue;
    }

    public static int WriteBytes(int value, byte[] byteArray, int begin)
    {
        byteArray[begin + 0] = (byte)(value);
        byteArray[begin + 1] = (byte)(value >> 8);
        byteArray[begin + 2] = (byte)(value >> 16);
        byteArray[begin + 3] = (byte)(value >> 24);

        return begin + 4;
    }

#if ENABLE_UNSFAE
    public static unsafe void CopyMemory(byte* srcPtr, byte* dstPtr, int length)
    {
        const int u32Size = sizeof(UInt32);
        const int u64Size = sizeof(UInt64);

        byte* srcEndPtr = srcPtr + length;

        int platformWordSize = IntPtr.Size * 8;

        if (platformWordSize == u32Size) {
            // 32-bit
            while (srcPtr + u64Size <= srcEndPtr) {
                *(UInt32*) dstPtr = *(UInt32*) srcPtr;
                dstPtr += u32Size;
                srcPtr += u32Size;
                *(UInt32*) dstPtr = *(UInt32*) srcPtr;
                dstPtr += u32Size;
                srcPtr += u32Size;
            }
        } else if (platformWordSize == u64Size) {
            // 64-bit            
            const int u128Size = sizeof(UInt64) * 2;
            while (srcPtr + u128Size <= srcEndPtr) {
                *(UInt64*) dstPtr = *(UInt64*) srcPtr;
                dstPtr += u64Size;
                srcPtr += u64Size;
                *(UInt64*) dstPtr = *(UInt64*) srcPtr;
                dstPtr += u64Size;
                srcPtr += u64Size;
            }
            if (srcPtr + u64Size <= srcEndPtr) {
                *(UInt64*) dstPtr ^= *(UInt64*) srcPtr;
                dstPtr += u64Size;
                srcPtr += u64Size;
            }
        }

        if (srcPtr + u32Size <= srcEndPtr) {
            *(UInt32*) dstPtr = *(UInt32*) srcPtr;
            dstPtr += u32Size;
            srcPtr += u32Size;
        }

        if (srcPtr + sizeof(UInt16) <= srcEndPtr) {
            *(UInt16*) dstPtr = *(UInt16*) srcPtr;
            dstPtr += sizeof(UInt16);
            srcPtr += sizeof(UInt16);
        }

        if (srcPtr + 1 <= srcEndPtr) {
            *dstPtr = *srcPtr;
        }
    }
#endif
}

public class ByteStepper
{
    float _oneStepInSeconds;

    float _remainTime;

    public ByteStepper(float oneStepInSeconds = 1f/255f)
    {
        _oneStepInSeconds = oneStepInSeconds;
    }

    public int GenerateInrement(float deltaTime)
    {
        _remainTime += deltaTime;

        var ratio = _remainTime / _oneStepInSeconds;

        var step = (int)Math.Floor(ratio);

        _remainTime = _remainTime - step * _oneStepInSeconds;

        return step;
    }
}