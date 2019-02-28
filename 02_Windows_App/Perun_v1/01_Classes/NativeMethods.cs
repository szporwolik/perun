﻿// this class just wraps some Win32 stuff that we're going to use for blocking of multiple instances
using System;
using System.Runtime.InteropServices;

internal class NativeMethods
{
    public const int HWND_BROADCAST = 0xffff;
    public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
    [DllImport("user32")]
    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
    [DllImport("user32")]
    public static extern int RegisterWindowMessage(string message);
}