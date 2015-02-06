﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelWindowCSharp
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct ARGBColor
    {
        public byte reserved;
        public byte red;
        public byte green;
        public byte blue;

        public override bool Equals(object obj)
        {
            if (!(obj is ARGBColor))
            {
                return false;
            }
            return (ARGBColor)obj == this;
        }

        public override int GetHashCode()
        {
            //Maybe this is not the best idea?
            return ((uint)this).GetHashCode();
        }

        public override string ToString()
        {
            return "0x" + ((uint)this).ToString("X8");
        }

        public static bool operator ==(ARGBColor color1, ARGBColor color2)
        {
            return color1.red == color2.red &&
                   color1.green == color2.green &&
                   color1.blue == color2.blue &&
                   color1.reserved == color2.reserved;
        }

        public static bool operator !=(ARGBColor color1, ARGBColor color2)
        {
            return !(color1 == color2);
        }

        public static explicit operator ARGBColor(uint i)
        {
            unsafe
            {
                //TODO: Is this a bad idea? Is it actually faster than setting each byte? Does it actually compile to nothing?
                return *((ARGBColor*)(&i));
            }
        }

        public static explicit operator uint(ARGBColor color)
        {
            unsafe
            {
                //TODO: Is this a bad idea? Is it actually faster than setting each byte? Does it actually compile to nothing?
                return *((uint*)(&color));
            }
        }
    }

    public class PixelWindow : IDisposable
    {
        private IntPtr pPixelWindow;

        public int ClientWidth
        {
            get
            {
                //TODO: should these be cached?
                return GetClientWidth(pPixelWindow);
            }
        }

        public int ClientHeight
        {
            get
            {
                return GetClientHeight(pPixelWindow);
            }
        }

        public int TotalPixels
        {
            get
            {
                return GetTotalPixels(pPixelWindow);
            }
        }

        public ARGBColor this[int index]
        {
            get
            {
                return GetPixel(index);
            }
            set
            {
                SetPixel(index, value);
            }
        }

        public ARGBColor this[int x, int y]
        {
            get
            {
                return GetPixel(x, y);
            }
            set
            {
                SetPixel(x, y, value);
            }
        }

        public PixelWindow(int windowWidth, int windowHeight, String windowTitle, int x = 0, int y = 0)
        {
            IntPtr hInstance = Process.GetCurrentProcess().Handle;
            pPixelWindow = CreatePixelWindow(hInstance, windowTitle, windowWidth, windowHeight, x, y);
        }

        public void Dispose()
        {
            DisposePixelWindow(pPixelWindow);
            GC.SuppressFinalize(this);
        }

        //Finalizer
        ~PixelWindow()
        {
            Dispose();
        }

        public void SetPixel(int x, int y, ARGBColor color)
        {
            bool success = SetPixel(pPixelWindow, x, y, color);
            if (!success)
            {
                throw new ArgumentOutOfRangeException("Coordinate is outside of the client's bounds");
            }
        }
        public void SetPixel(int index, ARGBColor color)
        {
            bool success = SetPixel(pPixelWindow, index, color);
            if (!success)
            {
                throw new ArgumentOutOfRangeException("Coordinate is outside of the client's bounds");
            }
        }

        public ARGBColor GetPixel(int x, int y)
        {
            return GetPixel(pPixelWindow, x, y);
        }
        public ARGBColor GetPixel(int index)
        {
            return GetPixel(pPixelWindow, index);
        }

        public void Fill(ARGBColor color)
        {
            Fill(pPixelWindow, color);
        }

        public bool IsWithinClient(int x, int y)
        {
            return IsWithinClient(pPixelWindow, x, y);
        }
        public bool IsWithinClient(int index)
        {
            return IsWithinClient(pPixelWindow, index);
        }

        public void UpdateClient()
        {
            UpdateClient(pPixelWindow);
        }

        //TODO: update the paths once testing is done
        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll", 
            CallingConvention = CallingConvention.Cdecl, 
            CharSet = CharSet.Unicode)]
        static private extern IntPtr CreatePixelWindow(IntPtr hInstance, string windowText, int windowWidth, int windowHeight, int windowX, int windowY);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll", 
            CallingConvention = CallingConvention.Cdecl, 
            CharSet = CharSet.Unicode)]
        static private extern void DisposePixelWindow(IntPtr pPixelWindow);



        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?SetPixel@PixelWindow@@QAE_NHHI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return : MarshalAs(UnmanagedType.U1)]
        static private extern bool SetPixel(IntPtr pClassObject, int x, int y, ARGBColor color);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?SetPixel@PixelWindow@@QAE_NHI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool SetPixel(IntPtr pClassObject, int index, ARGBColor color);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?GetPixel@PixelWindow@@QAEIHH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern ARGBColor GetPixel(IntPtr pClassObject, int x, int y);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?GetPixel@PixelWindow@@QAEIH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern ARGBColor GetPixel(IntPtr pClassObject, int index);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?Fill@PixelWindow@@QAEXI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern void Fill(IntPtr pClassObject, ARGBColor color);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?IsWithinClient@PixelWindow@@QAE_NHH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool IsWithinClient(IntPtr pClassObject, int x, int y);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?IsWithinClient@PixelWindow@@QAE_NH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool IsWithinClient(IntPtr pClassObject, int index);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?UpdateClient@PixelWindow@@QAEXXZ",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern void UpdateClient(IntPtr pClassObject);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll", 
            EntryPoint = "?GetClientWidth@PixelWindow@@QAEHXZ", 
            CallingConvention = CallingConvention.ThisCall)]
        static private extern int GetClientWidth(IntPtr pClassObject);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?GetClientHeight@PixelWindow@@QAEHXZ",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern int GetClientHeight(IntPtr pClassObject);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll",
            EntryPoint = "?GetTotalPixels@PixelWindow@@QAEHXZ",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern int GetTotalPixels(IntPtr pClassObject);
    }
}