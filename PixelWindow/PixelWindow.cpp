#pragma once

#include "PixelWindow.h"

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
void PixelWindow::CreateWindowAndRunMessageLoop(HINSTANCE hInstance, POINT windowTopLeft, SIZE clientDimensions, LPCWSTR windowText)
{
	// Register the window class.
	LPCWSTR CLASS_NAME = L"Sample Window Class";

	WNDCLASS wc = {};

	wc.lpfnWndProc = WindowProcedure;
	wc.hInstance = hInstance;
	wc.lpszClassName = CLASS_NAME;

	RegisterClass(&wc);

	//Note: AdjustWindowRectEx doesn't seem to actually work.
		
	/*RECT adjClientRect = desiredClientRect;*/
	DWORD extendedWindowStyles = 0;
	DWORD windowStyles = WS_OVERLAPPEDWINDOW;
	HMENU hMenu = NULL;
	/*if (!AdjustWindowRectEx(&adjClientRect, windowStyles, hMenu == NULL, extendedWindowStyles))
	{
		throw std::exception("AdjustWindowRectEx failed");
	}*/

	//It doesn't matter what the actual dimensions are, we'll change them anyway
	int windowWidth = 640;
	int windowHeight = 480;

	// Create the window.
	hWindow = CreateWindowEx(
		extendedWindowStyles,           // Optional window styles.
		CLASS_NAME,                     // Window class
		windowText,				        // Window text
		windowStyles,                   // Window style

		// Size and position
		windowTopLeft.x, windowTopLeft.y, windowWidth, windowHeight, 

		NULL,       // Parent window    
		hMenu,      // Menu
		hInstance,  // Instance handle
		NULL        // Additional application data
		);

	if (hWindow == NULL)
	{
		throw std::exception("Window creation failed");
	}

	int desiredClientWidth = clientDimensions.cx;
	int desiredClientHeight = clientDimensions.cy;

	RECT actualClientRect;
	GetClientRect(hWindow, &actualClientRect);

	int actualClientWidth = actualClientRect.right - actualClientRect.left;
	int actualClientHeight = actualClientRect.bottom - actualClientRect.top;
	
	//Note that hWndInsertAfter, x, and y are set to zero. They will be ignored because of the flags.
	SetWindowPos(hWindow, 0, 0, 0, windowWidth + (desiredClientWidth - actualClientWidth), windowHeight + (desiredClientHeight - actualClientHeight), SWP_NOMOVE | SWP_NOZORDER);

	RECT finalClientRect;
	GetClientRect(hWindow, &finalClientRect);

	width = finalClientRect.right - finalClientRect.left;
	height = finalClientRect.bottom - finalClientRect.top;

	if (width != desiredClientWidth)
	{
		throw new std::exception();
	}

	if (height != desiredClientHeight)
	{
		throw new std::exception();
	}

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
	hasBeenClosed = true;
}

DECLDIR PixelWindow::PixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int clientWidth, int clientHeight, int windowX, int windowY)
{
	POINT windowTopLeft = { windowX, windowY };
	SIZE clientDimensions = { clientWidth, clientHeight };

	hWindowCreatedEvent = CreateEvent(
		NULL,                       // default security attributes
		TRUE,                       // manual-reset event
		FALSE,                      // initial state is nonsignaled
		TEXT("WindowCreatedEvent")  // object name
		);

	messageLoopThread = std::thread(&PixelWindow::CreateWindowAndRunMessageLoop, this, hInstance, windowTopLeft, clientDimensions, windowText);

	WaitForSingleObject(hWindowCreatedEvent, INFINITE);

	hasBeenClosed = false;
}

DECLDIR PixelWindow::~PixelWindow()
{
	//Destroy the window
	SendMessage(hWindow, WM_CLOSE, 0, 0);
	messageLoopThread.join();

	//TODO: clean up stuff
}

DECLDIR bool PixelWindow::HasBeenClosed()
{
	return hasBeenClosed;
}

DECLDIR bool PixelWindow::SetPixel(int x, int y, uint32 color)
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

DECLDIR bool PixelWindow::SetPixel(int index, uint32 color)
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

DECLDIR void PixelWindow::SetPixelNoCheck(int x, int y, uint32 color)
{
	int numPixels = x + (y * width);

	uint32* writeTo = pBackBufferPixels + numPixels;
	*writeTo = color;
}

DECLDIR void PixelWindow::SetPixelNoCheck(int index, uint32 color)
{
	uint32* writeTo = pBackBufferPixels + index;
	*writeTo = color;
}

DECLDIR bool PixelWindow::GetPixel(int x, int y, uint32* outColor)
{
	if (IsWithinClient(x, y))
	{
		*outColor = GetPixelNoCheck(x, y);
		return true;
	}
	else
	{
		return false;
	}
}

DECLDIR bool PixelWindow::GetPixel(int index, uint32* outColor)
{
	if (IsWithinClient(index))
	{
		*outColor = GetPixelNoCheck(index);
		return true;
	}
	else
	{
		return false;
	}
}

DECLDIR uint32 PixelWindow::GetPixelNoCheck(int x, int y)
{
	int numPixels = x + (y * width);

	uint32* pColor = pBackBufferPixels + numPixels;
	return *pColor;
}

DECLDIR uint32 PixelWindow::GetPixelNoCheck(int index)
{
	uint32* pColor = pBackBufferPixels + index;
	return *pColor;
}

DECLDIR void PixelWindow::Fill(uint32 color)
{
	for (int i = 0; i < (width * height); i++)
	{
		uint32* pPixel = pBackBufferPixels + i;
		*pPixel = color;
	}
}

DECLDIR bool PixelWindow::IsWithinClient(int x, int y)
{
	return (x >= 0) && (y >= 0) && (x < width) && (y < height);
}

DECLDIR bool PixelWindow::IsWithinClient(int index)
{
	return index >= 0 && index < (width * height);
}

//TODO: is there a better name?
DECLDIR bool PixelWindow::UpdateClient()
{
	if (hasBeenClosed)
	{
		return false;
	}

	return BitBlt(GetDC(hWindow), 0, 0, width, height, hBackBufferDC, 0, 0, SRCCOPY);
}

//Getters
DECLDIR int PixelWindow::GetClientWidth()
{
	return width;
}

DECLDIR int PixelWindow::GetClientHeight()
{
	return height;
}

DECLDIR int PixelWindow::GetTotalPixels()
{
	return width * height;
}

DECLDIR HBITMAP PixelWindow::GetHBackBufferBitmap()
{
	return (HBITMAP)GetCurrentObject(hBackBufferDC, OBJ_BITMAP);
	//GetObject(hbmp, sizeof(BITMAP), &mainBitmap);
}