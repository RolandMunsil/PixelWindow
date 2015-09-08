using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWindowSDL;
using System.IO;

namespace PixelWindowTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // ============ Test Color ============
            Assert(new Color(0xFFEEDD) == new Color(0xFF, 0xEE, 0xDD));

            Color c = new Color(10, 20, 30, 40);
            Assert(c.red == 10);
            Assert(c.green == 20);
            Assert(c.blue == 30);
            Assert(c.alpha == 40);
            Assert(c == new Color(10, 20, 30, 40));

            Assert(new Color(0, 0, 0, 100).EqualsIgnoreAlpha(new Color(0, 0, 0, 200)));


            //========= Test PixelWindow ==========
            EnsureException<ArgumentOutOfRangeException>(() => new PixelWindow(-1, -1));
            EnsureException<ArgumentOutOfRangeException>(() => new PixelWindow(0, 0));
            EnsureException<ArgumentOutOfRangeException>(() => new PixelWindow(-1, 500));
            EnsureException<ArgumentOutOfRangeException>(() => new PixelWindow(500, -1));
            EnsureException<ArgumentOutOfRangeException>(() => new PixelWindow(0, 500));
            EnsureException<ArgumentOutOfRangeException>(() => new PixelWindow(500, 0));

            using (PixelWindow window = new PixelWindow(1000, 500))
            {
                Assert(window.ClientWidth == 1000);
                Assert(window.ClientHeight == 500);
                Assert(window.IsOpen);

                EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(1001, 501, new Color()));
                EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(1001, 250, new Color()));
                EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(250, 501, new Color()));
                EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(-1, 1, new Color()));
                EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(-1, -1, new Color()));
                EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(1, -1, new Color()));

                for (int x = 0; x < window.ClientWidth; x++)
                {
                    for (int y = 0; y < window.ClientHeight; y++)
                    {
                        window.SetPixel(x, y, Color.Red);
                    }
                }
                window.UpdateClient();

                Console.WriteLine("Client should be completely red.");
                Console.ReadKey();

                Console.WriteLine("Testing png saving...");
                String filePath = "test image.png";
                window.SaveClientToPNG(filePath);
                Assert(File.Exists(filePath));
                using (System.Drawing.Bitmap bm = new System.Drawing.Bitmap(filePath))
                {
                    Assert(bm.Width == window.ClientWidth);
                    Assert(bm.Height == window.ClientHeight);
                    for (int x = 0; x < window.ClientWidth; x++)
                    {
                        for (int y = 0; y < window.ClientHeight; y++)
                        {
                            var pix = bm.GetPixel(x, y);
                            Assert(bm.GetPixel(x, y) == System.Drawing.Color.FromArgb(0xFF, 0xFF, 0, 0));
                        }
                    }
                }
                File.Delete(filePath);

                for (int i = 0; i < 400; i++)
                {
                    window.SetPixel(i, i, Color.Green);
                }
                window.UpdateClient();

                Console.WriteLine("Client should have a diagonal green line extending from the top left corner.");
                Console.ReadKey();



                Console.WriteLine("Waiting for window to close...");
                while (window.IsOpen) ;
                Console.WriteLine("Window closed");
            }

            using (PixelWindow window = new PixelWindow(1000, 500, true))
            {
                for (int i = 0; i < 400; i++)
                {
                    window.SetPixel(i, i, Color.Green);
                }
                window.UpdateClient();

                Console.WriteLine("Client should have a diagonal green line extending from the bottom left corner.");
                Console.ReadKey();
            }

            Console.ReadKey();
        }

        static void EnsureException<TException>(Action action) where TException : Exception
        {
            bool exceptionThrown = false;
            try
            {
                action();
            }
            catch (TException)
            {
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                throw new Exception("action did not throw exception");
            }
        }

        static void Assert(bool expr)
        {
            if (!expr)
            {
                throw new Exception("assertion failed");
            }
        }
    }
}
