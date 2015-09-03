﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelWindowCSharpSDL;

namespace PixelWindowCSharpSDLTester
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
                        r = 255, 
                        g = 0, 
                        b = 0, 
                        a = 0}
                        );
                }
            }

            window.SetPixel(501, 501, new Color());

            window.UpdateClient();
            Console.ReadKey();
        }
    }
}