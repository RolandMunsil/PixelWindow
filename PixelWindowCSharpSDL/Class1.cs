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
    }
    public class PixelWindow : IDisposable
    {
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }

        private IntPtr pWindow;
        private IntPtr pRenderer;
        public PixelWindow(int width, int height)
        {
            ClientWidth = width;
            ClientHeight = height;
            SDL.SDL_CreateWindowAndRenderer(width, height, 0, out pWindow, out pRenderer);
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
            SDL.SDL_RenderDrawPoint(pRenderer, x, y);
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
