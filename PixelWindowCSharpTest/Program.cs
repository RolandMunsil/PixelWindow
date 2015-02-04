using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWindowCSharp;
using System.Threading;

namespace PixelWindowCSharpTest
{
    class Program
    {
        public static void Main(String[] args)
        {
            PixelWindow window = new PixelWindow(1280, 720, "It works!");
            //Just to test if class methods are working.
            Console.WriteLine(window.Width);
            Thread.Sleep(10000);
        }
    }
}
