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
            using (PixelWindow window = new PixelWindow(1280, 720, "Testing"))
            {
                if (window.ClientWidth != 1280)
                {
                    Debugger.Break();
                }
                if (window.ClientHeight != 720)
                {
                    Debugger.Break();
                }

                //Test everything

                //Test color getting//filling
                window.Fill(ARGBColor.Red);
                window.UpdateClient();
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

                
                Console.WriteLine("Client should be completely red.");
                Console.ReadKey();

                if (window.IsWithinClient(-1))
                {
                    Debugger.Break();
                }
                if (window.IsWithinClient(window.TotalPixels))
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

                //Test exception throwing
                BreakIfNoException<ArgumentOutOfRangeException>(() => window.SetPixel(-1, ARGBColor.Black));
                BreakIfNoException<ArgumentOutOfRangeException>(() => window.SetPixel(-1, -1, ARGBColor.Black));
                BreakIfNoException<ArgumentOutOfRangeException>(() => window.GetPixel(-1));
                BreakIfNoException<ArgumentOutOfRangeException>(() => window.GetPixel(-1, -1));

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

                //Test backbuffer getting
                System.Drawing.Bitmap bitmap = window.BackBuffer;
                if (bitmap.Width != window.ClientWidth)
                {
                    Debugger.Break();
                }
                if (bitmap.Height != window.ClientHeight)
                {
                    Debugger.Break();
                }
                for (int x = 0; x < window.ClientWidth; x++)
                {
                    for (int y = 0; y < window.ClientHeight; y++)
                    {
                        if (bitmap.GetPixel(x, (window.ClientHeight - 1) - y) != (System.Drawing.Color)window[x, y])
                        {
                            Debugger.Break();
                        }
                    }
                }

                window.UpdateClient();

                Console.WriteLine("Client should be checkerboard green and blue.");
                Console.ReadKey();

                //Test Bitmap thing.
                //window.BackBuffer.Save("test.png", System.Drawing.Imaging.ImageFormat.Png);

                Console.WriteLine("Please close the window.");
                while (!window.IsClosed) { };

                //Exeption should not be thrown.
                try
                {
                    window.UpdateClient();
                }
                catch (Exception)
                {
                    Debugger.Break();
                }
            }
        }

        static void BreakIfNoException<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                Debugger.Break();
            }
            catch (TException) { }
        }
    }
}
