using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace PixelWindowSDL
{
    public struct Color
    {
        public byte red;
        public byte green;
        public byte blue;
        public byte alpha;

        public Color(byte red, byte green, byte blue, byte alpha = 0)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        public Color(int hexColor3byte)
        {
            red =   (byte)(hexColor3byte >> 16);
            green = (byte)(hexColor3byte >> 8);
            blue =  (byte)(hexColor3byte);
            alpha = 0;
        }

        public static explicit operator Color(int hexColor3byte)
        {
            return new Color(hexColor3byte);
        }

        public static readonly Color White = new Color(0xFF, 0xFF, 0xFF);
        public static readonly Color Black = new Color(0, 0, 0);
        public static readonly Color Red = new Color(0xFF, 0, 0);
        public static readonly Color Green = new Color(0, 0xFF, 0);
        public static readonly Color Blue = new Color(0, 0, 0xFF);

        public override bool Equals(object obj)
        {
            if (!(obj is Color))
            {
                return false;
            }
            return (Color)obj == this;
        }

        public static bool operator ==(Color color1, Color color2)
        {
            return color1.red == color2.red &&
                   color1.green == color2.green &&
                   color1.blue == color2.blue &&
                   color1.alpha == color2.alpha;
        }

        public static bool operator !=(Color color1, Color color2)
        {
            return !(color1 == color2);
        }
    }
    public class PixelWindow : IDisposable
    {
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }

        private IntPtr pWindow;
        private IntPtr pRenderer;
        private bool reverseYDirection;
        public PixelWindow(int width, int height, bool reverseYDirection = false)
        {
            ClientWidth = width;
            ClientHeight = height;
            SDL.SDL_CreateWindowAndRenderer(width, height, 0, out pWindow, out pRenderer);
            this.reverseYDirection = reverseYDirection;
        }

        public void Dispose()
        {
            SDL.SDL_DestroyRenderer(pRenderer);
            SDL.SDL_DestroyWindow(pWindow);
            SDL.SDL_Quit();
            GC.SuppressFinalize(this);
        }

        //Finalizer
        ~PixelWindow()
        {
            SDL.SDL_DestroyRenderer(pRenderer);
            SDL.SDL_DestroyWindow(pWindow);
            SDL.SDL_Quit();
        }

        public Color this[int x, int y]
        {
            set
            {
                SetPixel(x, y, value);
            }
        }

        public void SetPixel(int x, int y, Color c)
        {
            if (!IsWithinClient(x, y))
            {
                throw new ArgumentOutOfRangeException("Coordinate is not within the client dimensions");
            }
            SDL.SDL_SetRenderDrawColor(pRenderer, c.red, c.green, c.blue, c.alpha);
            SDL.SDL_RenderDrawPoint(pRenderer, x, this.reverseYDirection ? (ClientHeight - 1) - y : y);
        }

        public bool IsWithinClient(int x, int y)
        {
            return x >= 0 && x < ClientWidth && y >= 0 && y < ClientHeight;
        }

        public void UpdateClient()
        {
            SDL.SDL_RenderPresent(pRenderer);
        }

        //public void GetClientAsBitmap()
        //{
        //    SDL.SDL_RenderCopy(
        //}

        //public void FillRect
    }
}
