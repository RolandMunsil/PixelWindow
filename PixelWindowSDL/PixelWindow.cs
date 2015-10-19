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

        public Color(byte red, byte green, byte blue, byte alpha = 0xFF)
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
            alpha = 0xFF;
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
    //TODO: mouse pos, etc.
    public class PixelWindow : IDisposable
    {
        public int ClientWidth { get; private set; }
        public int ClientHeight { get; private set; }
        public bool IsOpen { get; private set; }

        private IntPtr pWindow;
        private IntPtr pRenderer;
        private IntPtr pBackBufferSurface;
        private SDL.SDL_Surface backBufferSurface;
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

                //Create window
                pWindow = SDL.SDL_CreateWindow("", 0, 0, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI);
                if (pWindow == IntPtr.Zero) throw new Exception("Window creation failed.");
                SDL.SDL_SetWindowPosition(pWindow, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);

                //Create renderer
                pRenderer = SDL.SDL_CreateRenderer(pWindow, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);
                if (pRenderer == IntPtr.Zero) throw new Exception("Renderer creation failed.");
                SDL.SDL_SetRenderDrawBlendMode(pRenderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

                //Create backbuffer surface
                pBackBufferSurface = SDL.SDL_CreateRGBSurface(0, width, height, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000);
                unsafe
                {
                    backBufferSurface = *((SDL.SDL_Surface*)pBackBufferSurface);
                }
                //Unfortunately, because the 2nd parameter is a ref, I can't just pass in null.
                SDL.SDL_Rect r = new SDL.SDL_Rect { x = 0, y = 0, w = width, h = height };
                //Fill surface with black so that savepng output matches screen. Otherwise all the pixels initialize to alpha=0
                SDL.SDL_FillRect(pBackBufferSurface, ref r, 0xFF000000);
                //backBufferSurface = (SDL.SDL_Surface)Marshal.PtrToStructure(pBackBufferSurface, typeof(SDL.SDL_Surface));

                windowDoneCreating = true;
                EventLoop();
                SDL.SDL_FreeSurface(pBackBufferSurface);
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

            if (this.yAxisPointsUpwards) y = (ClientHeight - 1) - y;
            int colorAsInt = (c.alpha << 24) | (c.red << 16) | (c.green << 8) | (c.blue);

            int pixelIndex = y * backBufferSurface.w + x;
            unsafe
            {
                int* pPixel = ((int*)backBufferSurface.pixels.ToPointer()) + pixelIndex;
                *pPixel = colorAsInt;
            }

        }

        public bool IsWithinClient(int x, int y)
        {
            return x >= 0 && x < ClientWidth && y >= 0 && y < ClientHeight;
        }

        public void UpdateClient()
        {
            //Copy surface to window backbuffer
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(pRenderer, pBackBufferSurface);
            SDL.SDL_RenderCopy(pRenderer, texture, IntPtr.Zero, IntPtr.Zero);
            if(texture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(texture);
            }

            //Present window and clear backbuffer
            SDL.SDL_RenderPresent(pRenderer);
            SDL.SDL_SetRenderDrawColor(pRenderer, 0, 0, 0, 0);
            SDL.SDL_RenderClear(pRenderer);
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

        public void SaveClientToPNG(String filePath)
        {
            SDL_image.IMG_SavePNG(pBackBufferSurface, filePath);
        }

        //public void FillRect
    }
}
