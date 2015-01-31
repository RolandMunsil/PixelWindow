#pragma once

#include "PixelWindow.h"


//TODO: figure out whether UINT32 or RGBQUAD or even a custom struct should be used.

//class PixelWindow
//{
	/*HWND hWindow;
	RGBQUAD* pBackBufferPixels;
	HDC hBackBufferDC;

	int width;
	int height;

	HANDLE hWindowCreatedEvent;*/

	LRESULT CALLBACK PixelWindow::WindowProcedure(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
	{
		//TODO: do we actually need this special case?
		switch (uMsg)
		{
			case WM_DESTROY:
				PostQuitMessage(0);
				return 0;
		}

		return DefWindowProc(hwnd, uMsg, wParam, lParam);
	}

	//TODO: look up more about creating window. There are probably loads of problems with this.
	void PixelWindow::CreateWindowAndRunMessageLoop(HINSTANCE hInstance, RECT windowRect, LPCWSTR windowText)
	{
		// Register the window class.
		LPCWSTR CLASS_NAME = L"Sample Window Class";

		WNDCLASS wc = {};

		wc.lpfnWndProc = WindowProcedure;
		wc.hInstance = hInstance;
		wc.lpszClassName = CLASS_NAME;

		RegisterClass(&wc);

		//Note: AdjustWindowRectEx doesn't seem to actually work.
		
		/*RECT desiredClientRect = {};
		desiredClientRect.left = 0;
		desiredClientRect.top = 0;
		desiredClientRect.right = windowRect.right - windowRect.left;
		desiredClientRect.bottom = windowRect.bottom - windowRect.top;*/

		/*RECT adjClientRect = desiredClientRect;*/
		DWORD extendedWindowStyles = 0;
		DWORD windowStyles = WS_OVERLAPPEDWINDOW;
		HMENU hMenu = NULL;
		/*if (!AdjustWindowRectEx(&adjClientRect, windowStyles, hMenu == NULL, extendedWindowStyles))
		{
			throw std::exception("AdjustWindowRectEx failed");
		}*/

		// Create the window.
		hWindow = CreateWindowEx(
			extendedWindowStyles,           // Optional window styles.
			CLASS_NAME,                     // Window class
			windowText,				        // Window text
			windowStyles,                   // Window style

			// Size and position
			windowRect.left, windowRect.top, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top,

			NULL,       // Parent window    
			hMenu,      // Menu
			hInstance,  // Instance handle
			NULL        // Additional application data
			);

		if (hWindow == NULL)
		{
			throw std::exception("Window creation failed");
		}

		RECT actualRect;
		GetClientRect(hWindow, &actualRect);
		width = actualRect.right - actualRect.left;
		height = actualRect.bottom - actualRect.top;

		//DC = device context = bitmap + info needed to draw onto bitmap
		hBackBufferDC = CreateCompatibleDC(GetDC(hWindow));
		HBITMAP hOffscreenBitmap = CreateCompatibleBitmap(GetDC(hWindow), width, height);

		BITMAPINFO* pBitmapInfo = (BITMAPINFO*)malloc(sizeof(BITMAPINFOHEADER) + sizeof(RGBQUAD) * 256);
		ZeroMemory(pBitmapInfo, sizeof(BITMAPINFOHEADER) + sizeof(RGBQUAD) * 256);
		pBitmapInfo->bmiHeader.biSize = sizeof(BITMAPINFOHEADER);

		//It is VERY IMPORTANT that this is called twice! Removing one call will cause the BITMAPINFO to be incomplete.
		GetDIBits(hBackBufferDC, hOffscreenBitmap, 0, 0, NULL, pBitmapInfo, DIB_RGB_COLORS);
		GetDIBits(hBackBufferDC, hOffscreenBitmap, 0, 0, NULL, pBitmapInfo, DIB_RGB_COLORS);

		//DIB = device-independent bitmap
		//A DIB section is a DIB in memory
		HBITMAP hOffscreenDIBitmap = CreateDIBSection(GetDC(hWindow), pBitmapInfo, DIB_RGB_COLORS, (void**)&pBackBufferPixels, NULL, 0);
		SelectObject(hBackBufferDC, hOffscreenDIBitmap);

		//Use this to get the actual bitmap
		/*HBITMAP hbmp = (HBITMAP)GetCurrentObject(hBackBufferDC, OBJ_BITMAP);
		GetObject(hbmp, sizeof(BITMAP), &mainBitmap);*/

		ShowWindow(hWindow, SW_SHOWNORMAL);

		//Tell the constructor it can return.
		SetEvent(hWindowCreatedEvent);

		// Run the message loop.
		MSG msg = {};
		while (GetMessage(&msg, NULL, 0, 0))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	PixelWindow::PixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int windowWidth, int windowHeight, int windowX, int windowY)
	{
		RECT windowRect;
		windowRect.left = windowX;
		windowRect.top = windowY;
		windowRect.right = windowX + windowWidth;
		windowRect.bottom = windowY + windowHeight;

		hWindowCreatedEvent = CreateEvent(
			NULL,                       // default security attributes
			TRUE,                       // manual-reset event
			FALSE,                      // initial state is nonsignaled
			TEXT("WindowCreatedEvent")  // object name
			);

		messageLoopThread = std::thread(&PixelWindow::CreateWindowAndRunMessageLoop, this, hInstance, windowRect, windowText);

		WaitForSingleObject(hWindowCreatedEvent, INFINITE);
	}

	void PixelWindow::Destroy()
	{
		SendMessage(hWindow, WM_CLOSE, 0, 0);
		messageLoopThread.join();
	}


	bool PixelWindow::SetPixel(int x, int y, RGBQUAD color)
	{
		if (IsWithinClient(x, y))
		{
			SetPixelNoCheck(x, y, color);
			return true;
		}
		else
		{
			return false;
		}
	}

	bool PixelWindow::SetPixel(int index, RGBQUAD color)
	{
		if (IsWithinClient(index))
		{
			SetPixelNoCheck(index, color);
			return true;
		}
		else
		{
			return false;
		}
	}

	void PixelWindow::SetPixelNoCheck(int x, int y, RGBQUAD color)
	{
		int numPixels = x + (y * width);

		RGBQUAD* writeTo = pBackBufferPixels + numPixels;
		*writeTo = color;
	}

	void PixelWindow::SetPixelNoCheck(int index, RGBQUAD color)
	{
		RGBQUAD* writeTo = pBackBufferPixels + index;
		*writeTo = color;
	}

	RGBQUAD PixelWindow::GetPixel(int x, int y)
	{
		if (IsWithinClient(x, y))
		{
			return GetPixelNoCheck(x, y);
		}
		else
		{
			throw std::out_of_range("Coordinates are outside of the client's bounds");
		}
	}

	RGBQUAD PixelWindow::GetPixel(int index)
	{
		if (IsWithinClient(index))
		{
			return GetPixelNoCheck(index);
		}
		else
		{
			throw std::out_of_range("Coordinates are outside of the client's bounds");
		}
	}

	RGBQUAD PixelWindow::GetPixelNoCheck(int x, int y)
	{
		int numPixels = x + (y * width);

		RGBQUAD* pColor = pBackBufferPixels + numPixels;
		return *reinterpret_cast<RGBQUAD*>(pColor);
	}

	RGBQUAD PixelWindow::GetPixelNoCheck(int index)
	{
		RGBQUAD* pColor = pBackBufferPixels + index;
		return *reinterpret_cast<RGBQUAD*>(pColor);
	}

	void PixelWindow::Fill(RGBQUAD color)
	{
		for (int i = 0; i < (width * height); i++)
		{
			RGBQUAD* pPixel = pBackBufferPixels + i;
			*pPixel = color;
		}
	}

	bool PixelWindow::IsWithinClient(int x, int y)
	{
		return (x >= 0) && (y >= 0) && (x < width) && (y < height);
	}

	bool PixelWindow::IsWithinClient(int index)
	{
		return index >= 0 && index < (width * height);
	}

	//TODO: is there a better name?
	void PixelWindow::UpdateClient()
	{
		BitBlt(GetDC(hWindow), 0, 0, width, height, hBackBufferDC, 0, 0, SRCCOPY);
	}

	//Getters
	int PixelWindow::GetClientWidth()
	{
		return width;
	}

	int PixelWindow::GetClientHeight()
	{
		return height;
	}

	int PixelWindow::GetTotalPixels()
	{
		return width * height;
	}

//};