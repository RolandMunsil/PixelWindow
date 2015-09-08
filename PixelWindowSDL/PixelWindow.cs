using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

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

        public bool EqualsIgnoreAlpha(Color color)
        {
            return this.red == color.red &&
                   this.green == color.green &&
                   this.blue == color.blue;
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
        public bool IsOpen { get; private set; }

        private IntPtr pWindow;
        private IntPtr pRenderer;
        private bool yAxisPointsUpwards;

        private Thread windowThread;
        private bool windowDoneCreating;

        //TODO: use the flags? http://wiki.libsdl.org/SDL_WindowFlags
        public PixelWindow(int width, int height, bool yAxisPointsUpwards = false)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("Invalid width or height");
            }

            ClientWidth = width;
            ClientHeight = height;
            this.yAxisPointsUpwards = yAxisPointsUpwards;
            IsOpen = true;

            windowDoneCreating = false;
            windowThread = new Thread(delegate()
            {
                SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
                //TODO: why does ths work without calling SDL_Init?
                if (-1 == SDL.SDL_CreateWindowAndRenderer(width, height, SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI, out pWindow, out pRenderer))
                {
                    throw new Exception("Window and renderer creation failed.");
                }
                SDL.SDL_SetWindowPosition(pWindow, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);
                windowDoneCreating = true;
                EventLoop();
                //TODO: dispose stuff? Or wait for Dispose() call?
            });
            windowThread.Start();

            while (!windowDoneCreating) ;
        }

        public void Dispose()
        {
            SDL.SDL_DestroyRenderer(pRenderer);
            SDL.SDL_DestroyWindow(pWindow);
            SDL.SDL_Quit();
            this.IsOpen = false;
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
            SDL.SDL_RenderDrawPoint(pRenderer, x, this.yAxisPointsUpwards ? (ClientHeight - 1) - y : y);
        }

        public bool IsWithinClient(int x, int y)
        {
            return x >= 0 && x < ClientWidth && y >= 0 && y < ClientHeight;
        }

        public void UpdateClient()
        {
            SDL.SDL_RenderPresent(pRenderer);
        }

        private void EventLoop()
        {
            while (true)
            {
                SDL.SDL_Event sdlEvent;
                SDL.SDL_PollEvent(out sdlEvent);
                if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    IsOpen = false;
                    return;
                }
            }
        }

        //http://stackoverflow.com/questions/22315980/sdl2-c-taking-a-screenshot
        public void SaveClientToPNG(String filePath)
        {
            IntPtr pSurface = SDL.SDL_CreateRGBSurface(0, ClientWidth, ClientHeight, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000);
            SDL.SDL_Surface surface = (SDL.SDL_Surface)Marshal.PtrToStructure(pSurface, typeof(SDL.SDL_Surface));
            SDL.SDL_Rect rect = new SDL.SDL_Rect { x = 0, y = 0, w = ClientWidth, h = ClientHeight };

            SDL.SDL_RenderReadPixels(pRenderer, ref rect, SDL.SDL_PIXELFORMAT_ARGB8888, surface.pixels, surface.pitch);
            SDL_image.IMG_SavePNG(pSurface, filePath);

            SDL.SDL_FreeSurface(pSurface);
        }

        //public void FillRect
    }
}
