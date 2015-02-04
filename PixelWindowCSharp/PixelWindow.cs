using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelWindowCSharp
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct RGBAColor
    {
        byte red;
        byte green;
        byte blue;
        byte reserved;
    }

    public class PixelWindow
    {
        IntPtr pPixelWindow;

        public int Width
        {
            get
            {
                return GetClientWidth(pPixelWindow);
            }
        }

        public int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int TotalPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public RGBAColor this[int index]
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

        public RGBAColor this[int x, int y]
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

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public bool SetPixel(int x, int y, RGBAColor color)
        {
            throw new NotImplementedException();
        }
        public bool SetPixel(int index, RGBAColor color)
        {
            throw new NotImplementedException();
        }

        public void SetPixelNoCheck(int x, int y, RGBAColor color)
        {
            throw new NotImplementedException();
        }
        public void SetPixelNoCheck(int index, RGBAColor color)
        {
            throw new NotImplementedException();
        }

        public RGBAColor GetPixel(int x, int y)
        {
            throw new NotImplementedException();
        }
        public RGBAColor GetPixel(int index)
        {
            throw new NotImplementedException();
        }

        public RGBAColor GetPixelNoCheck(int x, int y)
        {
            throw new NotImplementedException();
        }
        public RGBAColor GetPixelNoCheck(int index)
        {
            throw new NotImplementedException();
        }

        public void Fill(RGBAColor color)
        {
            throw new NotImplementedException();
        }

        public bool IsWithinClient(int x, int y)
        {
            throw new NotImplementedException();
        }
        public bool IsWithinClient(int index)
        {
            throw new NotImplementedException();
        }

        public void UpdateClient()
        {
            throw new NotImplementedException();
        }

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static private extern IntPtr CreatePixelWindow(IntPtr hInstance, string windowText, int windowWidth, int windowHeight, int windowX, int windowY);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static private extern void DisposePixelWindow(IntPtr pPixelWindow);

        [DllImport(@"C:\Users\Roland\Documents\Visual Studio 2013\Projects\PixelWindow\PixelWindow\Debug\PixelWindow.dll", EntryPoint = "?GetClientWidth@PixelWindow@@QAEHXZ", CallingConvention = CallingConvention.ThisCall)]
        static public extern int GetClientWidth(IntPtr pClassObject);
    }
}
