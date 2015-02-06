using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWindowCSharp;
using System.Threading;
using System.Diagnostics;

namespace PixelWindowCSharpTest
{
    class Program
    {
        public static void Main(String[] args)
        {
            PixelWindow window = new PixelWindow(1280, 720, "It works!");

            //Test everything

            //Test width and height getters
            Console.WriteLine("Client width: " + window.ClientWidth);
            Console.WriteLine("Client height: " + window.ClientHeight);
            //Console.ReadKey();

            //Test color getting//filling
            window.Fill((RGBAColor)0xFF0000FF);
            for (int i = 0; i < window.TotalPixels; i++)
            {
                if (window.GetPixel(i) != (RGBAColor)0xFF0000FF)
                {
                    Debugger.Break();
                }
            }
            for (int x = 0; x < window.ClientWidth; x++)
            {
                for (int y = 0; y < window.ClientHeight; y++)
                {
                    if (window.GetPixel(x, y) != (RGBAColor)0xFF0000FF)
                    {
                        Debugger.Break();
                    }
                }
            }
            window.UpdateClient();
            Console.WriteLine("Client should be completely red.");
            Console.ReadKey();

            if (window.IsWithinClient(-1))
            {
                Debugger.Break();
            }
            if (window.IsWithinClient(-1, -1))
            {
                Debugger.Break();
            }
            if (window.IsWithinClient(-1, 1))
            {
                Debugger.Break();
            }
            if (window.IsWithinClient(1, -1))
            {
                Debugger.Break();
            }
            if (window.IsWithinClient(window.ClientWidth, window.ClientHeight))
            {
                Debugger.Break();
            }
            if (window.IsWithinClient(0, window.ClientHeight))
            {
                Debugger.Break();
            }
            if (window.IsWithinClient(window.ClientWidth, 0))
            {
                Debugger.Break();
            }

            if (!window.IsWithinClient(0, 0))
            {
                Debugger.Break();
            }
            if (!window.IsWithinClient(window.ClientWidth - 1, 0))
            {
                Debugger.Break();
            }
            if (!window.IsWithinClient(0, window.ClientHeight - 1))
            {
                Debugger.Break();
            }
            if (!window.IsWithinClient(window.ClientWidth - 1, window.ClientHeight - 1))
            {
                Debugger.Break();
            }

            for (int i = 0; i < window.TotalPixels; i++)
            {
                if (i % 2 == 0)
                {
                    window.SetPixel(i, (RGBAColor)0x00FF00FF);
                }
                else
                {
                    window.SetPixel(i, (RGBAColor)0x0000FFFF);
                }
                window.UpdateClient();
            }

            Console.WriteLine("Client should be checkerbord green and blue.");
            Console.ReadKey();
        }
    }
}
