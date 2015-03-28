using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelWindowCSharp
{
    //TODO: add xml documentation

    [StructLayout(LayoutKind.Explicit)]
    public struct ARGBColor
    {
        [FieldOffset(3)]
        public byte reserved;
        [FieldOffset(2)]
        public byte red;
        [FieldOffset(1)]
        public byte green;
        [FieldOffset(0)]
        public byte blue;

        //TODO: better name?
        [FieldOffset(0)]
        public uint asUint32;

        public static readonly ARGBColor Red = (ARGBColor)0xFFFF0000;
        public static readonly ARGBColor Green = (ARGBColor)0xFF00FF00;
        public static readonly ARGBColor Blue = (ARGBColor)0xFF0000FF;
        public static readonly ARGBColor Black = (ARGBColor)0xFF000000;
        public static readonly ARGBColor White = (ARGBColor)0xFFFFFFFF;

        public ARGBColor(byte red, byte green, byte blue, byte reserved = 0)
        {
            //So that all the variables are assigned
            this.asUint32 = 0;

            this.red = red;
            this.green = green;
            this.blue = blue;
            this.reserved = reserved;
        }

        public ARGBColor(uint uint32Color)
        {
            //So that all the variables are assigned
            this.red = 0;
            this.green = 0;
            this.blue = 0;
            this.reserved = 0;

            this.asUint32 = uint32Color;
        }

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
            return asUint32.GetHashCode();
        }

        public override string ToString()
        {
            return "0x" + asUint32.ToString("X8");
        }

        public static bool operator ==(ARGBColor color1, ARGBColor color2)
        {
            return color1.asUint32 == color2.asUint32;
        }

        public static bool operator !=(ARGBColor color1, ARGBColor color2)
        {
            return !(color1 == color2);
        }

        public static explicit operator ARGBColor(uint i)
        {
            return new ARGBColor(i);

            //unsafe
            //{
            //    //TODO: Is this a bad idea? Is it actually faster than setting each byte? Does it actually compile to nothing?
            //    return *((ARGBColor*)(&i));
            //}
        }

        public static explicit operator uint(ARGBColor color)
        {
            return color.asUint32;
        }

        public static explicit operator ARGBColor(System.Drawing.Color color)
        {
            return (ARGBColor)unchecked((uint)color.ToArgb());
        }

        public static explicit operator System.Drawing.Color(ARGBColor color)
        {
            return System.Drawing.Color.FromArgb(unchecked((int)color.asUint32));
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

        //TODO: should this be IsOpen instead of IsClosed?
        public bool IsClosed
        {
            get
            {
                return HasBeenClosed(pPixelWindow);
            }
        }

        public System.Drawing.Bitmap BackBuffer
        {
            get
            {
                return System.Drawing.Image.FromHbitmap(GetHBackBufferBitmap(pPixelWindow));
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

        public PixelWindow(int clientWidth, int clientHeight, String windowTitle, int x = 0, int y = 0)
        {
            IntPtr hInstance = Process.GetCurrentProcess().Handle;
            pPixelWindow = CreatePixelWindow(hInstance, windowTitle, clientWidth, clientHeight, x, y);
        }

        public void Dispose()
        {
            DisposePixelWindow(pPixelWindow);
            GC.SuppressFinalize(this);
        }

        //Finalizer
        ~PixelWindow()
        {
            DisposePixelWindow(pPixelWindow);
        }

        public void SetPixel(int x, int y, ARGBColor color)
        {
            bool success = SetPixel(pPixelWindow, x, y, color);
            if (!success)
            {
                throw new ArgumentOutOfRangeException("Coordinates are outside of the client's bounds");
            }
        }
        public void SetPixel(int index, ARGBColor color)
        {
            bool success = SetPixel(pPixelWindow, index, color);
            if (!success)
            {
                throw new ArgumentOutOfRangeException("Index is outside of the client's bounds");
            }
        }

        public ARGBColor GetPixel(int x, int y)
        {
            ARGBColor color;
            if (!GetPixel(pPixelWindow, x, y, out color))
            {
                throw new ArgumentOutOfRangeException("Coordinates are outside of the client's bounds");
            }
            return color;
        }
        public ARGBColor GetPixel(int index)
        {
            ARGBColor color;
            if (!GetPixel(pPixelWindow, index, out color))
            {
                throw new ArgumentOutOfRangeException("Index is outside of the client's bounds");
            }
            return color;
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

        public bool UpdateClient()
        {
            return UpdateClient(pPixelWindow);
        }

        //dumpbin /exports "C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\Debug\PixelWindow.dll"

        [DllImport("PixelWindow.dll", 
            CallingConvention = CallingConvention.Cdecl, 
            CharSet = CharSet.Unicode)]
        static private extern IntPtr CreatePixelWindow(IntPtr hInstance, string windowText, int clientWidth, int clientHeight, int windowX, int windowY);

        [DllImport("PixelWindow.dll", 
            CallingConvention = CallingConvention.Cdecl, 
            CharSet = CharSet.Unicode)]
        static private extern void DisposePixelWindow(IntPtr pPixelWindow);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?HasBeenClosed@PixelWindow@@QAE_NXZ",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool HasBeenClosed(IntPtr pClassObject);



        [DllImport("PixelWindow.dll",
            EntryPoint = "?SetPixel@PixelWindow@@QAE_NHHI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return : MarshalAs(UnmanagedType.U1)]
        static private extern bool SetPixel(IntPtr pClassObject, int x, int y, ARGBColor color);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?SetPixel@PixelWindow@@QAE_NHI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool SetPixel(IntPtr pClassObject, int index, ARGBColor color);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?GetPixel@PixelWindow@@QAE_NHHPAI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool GetPixel(IntPtr pClassObject, int x, int y, out ARGBColor color);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?GetPixel@PixelWindow@@QAE_NHPAI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool GetPixel(IntPtr pClassObject, int index, out ARGBColor color);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?Fill@PixelWindow@@QAEXI@Z",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern void Fill(IntPtr pClassObject, ARGBColor color);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?IsWithinClient@PixelWindow@@QAE_NHH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool IsWithinClient(IntPtr pClassObject, int x, int y);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?IsWithinClient@PixelWindow@@QAE_NH@Z",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool IsWithinClient(IntPtr pClassObject, int index);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?UpdateClient@PixelWindow@@QAE_NXZ",
            CallingConvention = CallingConvention.ThisCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        static private extern bool UpdateClient(IntPtr pClassObject);

        [DllImport("PixelWindow.dll", 
            EntryPoint = "?GetClientWidth@PixelWindow@@QAEHXZ", 
            CallingConvention = CallingConvention.ThisCall)]
        static private extern int GetClientWidth(IntPtr pClassObject);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?GetClientHeight@PixelWindow@@QAEHXZ",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern int GetClientHeight(IntPtr pClassObject);

        [DllImport("PixelWindow.dll",
            EntryPoint = "?GetTotalPixels@PixelWindow@@QAEHXZ",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern int GetTotalPixels(IntPtr pClassObject);

        [DllImport("PixelWindow.dll",
            EntryPoint="?GetHBackBufferBitmap@PixelWindow@@QAEPAUHBITMAP__@@XZ",
            CallingConvention = CallingConvention.ThisCall)]
        static private extern IntPtr GetHBackBufferBitmap(IntPtr pClassObject);
    }
}
