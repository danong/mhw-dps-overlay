using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class memory
{
    [DllImport("kernel32.dll")]
    private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out memory.MEMORY_BASIC_INFORMATION64 lpBuffer, uint dwLength);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    public static bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
    {
        int lpNumberOfBytesRead = 0;
        return memory.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, ref lpNumberOfBytesRead);
    }

    private static List<int> byte_find(byte[] src, byte[] pattern)
    {
        List<int> intList = new List<int>();
        if (src.Length < pattern.Length)
            return intList;
        for (int index1 = 0; index1 < src.Length - pattern.Length + 1; ++index1)
        {
            bool flag = true;
            for (int index2 = 0; index2 < pattern.Length; ++index2)
            {
                if ((int)src[index1 + index2] != (int)pattern[index2])
                    flag = false;
            }
            if (flag)
                intList.Add(index1);
        }
        return intList;
    }

    private static int byte_find_first(byte[] src, byte?[] pattern)
    {
        List<int> intList = new List<int>();
        if (src.Length < pattern.Length)
            return -1;
        for (int index1 = 0; index1 < src.Length - pattern.Length + 1; ++index1)
        {
            bool flag = true;
            for (int index2 = 0; index2 < pattern.Length; ++index2)
            {
                if (pattern[index2].HasValue)
                {
                    int num = (int)src[index1 + index2];
                    byte? nullable1 = pattern[index2];
                    int? nullable2 = nullable1.HasValue ? new int?((int)nullable1.GetValueOrDefault()) : new int?();
                    int valueOrDefault = nullable2.GetValueOrDefault();
                    if ((num == valueOrDefault ? (!nullable2.HasValue ? 1 : 0) : 1) != 0)
                        flag = false;
                }
            }
            if (flag)
                return index1;
        }
        return -1;
    }

    public static ulong[] find_patterns(Process proc, IntPtr start_from, IntPtr end_at, List<byte?[]> patterns)
    {
        IntPtr lpAddress = start_from;
        ulong[] numArray1 = new ulong[patterns.Count];
        int count = patterns.Count;
        do
        {
            memory.MEMORY_BASIC_INFORMATION64 lpBuffer;
            if (memory.VirtualQueryEx(proc.Handle, lpAddress, out lpBuffer, (uint)Marshal.SizeOf(typeof(memory.MEMORY_BASIC_INFORMATION64))) > 0 && lpBuffer.RegionSize > 0UL)
            {
                byte[] numArray2 = new byte[(int)lpBuffer.RegionSize];
                memory.ReadProcessMemory(proc.Handle, (IntPtr)((long)lpBuffer.BaseAddress), numArray2);
                for (int index = 0; index < patterns.Count; ++index)
                {
                    if (numArray1[index] <= 0UL)
                    {
                        int first = memory.byte_find_first(numArray2, patterns[index]);
                        if (first > 0)
                        {
                            numArray1[index] = lpBuffer.BaseAddress + (ulong)(uint)first;
                            --count;
                        }
                    }
                }
            }
            lpAddress = (IntPtr)((long)lpBuffer.BaseAddress + (long)lpBuffer.RegionSize);
        }
        while ((ulong)(long)lpAddress < (ulong)(long)end_at && count > 0);
        return numArray1;
    }

    public struct MEMORY_BASIC_INFORMATION64
    {
        public ulong BaseAddress;
        public ulong AllocationBase;
        public int AllocationProtect;
        public int __alignment1;
        public ulong RegionSize;
        public int State;
        public int Protect;
        public int Type;
        public int __alignment2;
    }
}
