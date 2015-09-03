using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWindowSDL;

namespace PixelWindowTester
{
    class Program
    {
        static void Main(string[] args)
        {
            PixelWindow window = new PixelWindow(500, 500);
            Random rand = new Random();

            for (int x = 0; x < 500; x++)
            {
                for (int y = 0; y < 500; y++)
                {
                    window.SetPixel(x, y, new Color{
                        red = 255, 
                        green = 0, 
                        blue = 0, 
                        alpha = 0}
                        );
                }
            }

            EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(501, 501, new Color()));
            EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(501, 250, new Color()));
            EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(250, 501, new Color()));
            EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(-1, 1, new Color()));
            EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(-1, -1, new Color()));
            EnsureException<ArgumentOutOfRangeException>(() => window.SetPixel(1, -1, new Color()));

            window.UpdateClient();
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
    }
}
