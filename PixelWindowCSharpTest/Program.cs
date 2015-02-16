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
            window.Fill(ARGBColor.Red);
            for (int i = 0; i < window.TotalPixels; i++)
            {
                if (window.GetPixel(i) != ARGBColor.Red)
                {
                    Debugger.Break();
                }
            }
            for (int x = 0; x < window.ClientWidth; x++)
            {
                for (int y = 0; y < window.ClientHeight; y++)
                {
                    if (window.GetPixel(x, y) != ARGBColor.Red)
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
                if (i % 2 == 0 ^ (i / window.ClientWidth) % 2 == 0)
                {
                    window.SetPixel(i, ARGBColor.Blue);
                }
                else
                {
                    window.SetPixel(i, ARGBColor.Green);
                }
                if (i % window.ClientWidth == 0)
                {
                    //window.UpdateClient();
                }
            }
            window.UpdateClient();

            Console.WriteLine("Client should be checkerboard green and blue.");
            Console.ReadKey();

            //Test Bitmap thing.
            //window.BackBuffer.Save("test.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Please close the window.");
            while (!window.IsClosed) { };

            bool threwException = false;
            try
            {
                window.UpdateClient();
            }
            catch (InvalidOperationException)
            {
                threwException = true;
            }

            if (!threwException)
            {
                Debugger.Break();
            }

            window.Dispose();
        }
    }
}
