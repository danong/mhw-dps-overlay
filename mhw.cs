// Decompiled with JetBrains decompiler
// Type: mhw
// Assembly: mhw_dps_wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9AF2A04E-56DA-4D52-9297-848C4FCE5E85
// Assembly location: C:\Users\Daniel\Desktop\mhw_damage_meter_1_0.exe

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public static class mhw
{
  [DllImport("kernel32.dll")]
  private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

  private static bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer)
  {
    int lpNumberOfBytesRead = 0;
    return mhw.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, lpBuffer.Length, ref lpNumberOfBytesRead);
  }

  private static ulong read_ulong(IntPtr hProcess, IntPtr lpBaseAddress)
  {
    byte[] lpBuffer = new byte[8];
    mhw.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer);
    return BitConverter.ToUInt64(lpBuffer, 0);
  }

  private static uint read_uint(IntPtr hProcess, IntPtr lpBaseAddress)
  {
    byte[] lpBuffer = new byte[4];
    mhw.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer);
    return BitConverter.ToUInt32(lpBuffer, 0);
  }

  private static int dword_to_int(ref byte[] array)
  {
    return (int) array[0] + ((int) array[1] << 8) + ((int) array[2] << 16) + ((int) array[3] << 24);
  }

  private static ulong asm_func1(Process proc, ulong rcx, uint edx)
  {
    int num1 = (int) mhw.read_uint(proc.Handle, (IntPtr) 5428922716L);
    ulong num2 = rcx;
    int num3 = (int) edx;
    rcx = (ulong) (uint) (num1 & num3) * 88UL;
    return num2 + 72UL + rcx;
  }

  public static int[] get_team_dmg(Process proc)
  {
    int[] numArray = new int[4];
    byte[] array = new byte[4];
    byte[] lpBuffer = new byte[8];
    mhw.ReadProcessMemory(proc.Handle, (IntPtr) 5430696680L, lpBuffer);
    ulong num1 = BitConverter.ToUInt64(lpBuffer, 0) + 26288UL;
    mhw.ReadProcessMemory(proc.Handle, (IntPtr) 5430707384L, lpBuffer);
    ulong uint64_1 = BitConverter.ToUInt64(lpBuffer, 0);
    for (int index = 0; index < 4; ++index)
    {
      mhw.ReadProcessMemory(proc.Handle, (IntPtr) ((long) num1 + 4L * (long) index), array);
      uint edx = mhw.read_uint(proc.Handle, (IntPtr) ((long) num1 + 4L * (long) index));
      ulong num2 = mhw.asm_func1(proc, uint64_1, edx);
      if (num2 > 0UL)
      {
        mhw.ReadProcessMemory(proc.Handle, (IntPtr) ((long) num2 + 72L), lpBuffer);
        ulong uint64_2 = BitConverter.ToUInt64(lpBuffer, 0);
        if (uint64_2 > 0UL)
        {
          int num3 = mhw.ReadProcessMemory(proc.Handle, (IntPtr) ((long) uint64_2 + 72L), array) ? 1 : 0;
          int num4 = mhw.dword_to_int(ref array);
          if (num3 != 0 && num4 >= 0 && num4 <= 1048575)
            numArray[index] = num4;
        }
      }
    }
    return numArray;
  }

  public static int get_player_seat_id(Process proc)
  {
    uint num1 = mhw.read_uint(proc.Handle, (IntPtr) 5444288144L);
    uint num2 = mhw.read_uint(proc.Handle, (IntPtr) ((long) (num1 + 600U)));
    int num3 = -1;
    if (num2 > 4096U)
    {
      uint num4 = mhw.read_uint(proc.Handle, (IntPtr) ((long) (num2 + 16U)));
      if (num4 != 0U)
        num3 = (int) mhw.read_uint(proc.Handle, (IntPtr) ((long) (num4 + 49132U)));
    }
    return num3;
  }

  public static string[] get_team_player_names(Process proc)
  {
    string[] strArray = new string[4];
    byte[] array = new byte[40];
    int num = (int) mhw.read_uint(proc.Handle, (IntPtr) 5444288144L) + 346693;
    for (int index1 = 0; index1 < 4; ++index1)
    {
      Array.Resize<byte>(ref array, 40);
      mhw.ReadProcessMemory(proc.Handle, (IntPtr) (num + 33 * index1), array);
      int index2 = Array.FindIndex<byte>(array, (Predicate<byte>) (x => x == (byte) 0));
      switch (index2)
      {
        case -1:
        case 0:
          strArray[index1] = "";
          break;
        default:
          Array.Resize<byte>(ref array, index2);
          strArray[index1] = Encoding.UTF8.GetString(array);
          break;
      }
    }
    return strArray;
  }
}
