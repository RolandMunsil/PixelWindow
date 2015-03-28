#pragma once

#ifndef UNICODE
#define UNICODE
#endif 

#include <windows.h>
#include <stdexcept>
#include <thread>

#if defined DLL_EXPORT
#define DECLDIR __declspec(dllexport)
#else
#define DECLDIR __declspec(dllimport)
#endif

typedef uint32_t uint32;

class PixelWindow
{
private:
	HWND hWindow;
	uint32* pBackBufferPixels;
	HDC hBackBufferDC;
	bool hasBeenClosed;

	int width;
	int height;

	HANDLE hWindowCreatedEvent;

	std::thread messageLoopThread;

	static LRESULT CALLBACK WindowProcedure(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	void CreateWindowAndRunMessageLoop(HINSTANCE hInstance, POINT windowTopLeft, SIZE clientDimensions, LPCWSTR windowText);

public:
	DECLDIR PixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int clientWidth, int clientHeight, int windowX = 0, int windowY = 0);
	DECLDIR ~PixelWindow();

	DECLDIR bool HasBeenClosed();

	DECLDIR bool SetPixel(int x, int y, uint32 color);
	DECLDIR bool SetPixel(int index, uint32 color);

	//TODO: compare performance of check vs. nocheck and decide whether to keep NoCheck methods
	DECLDIR void SetPixelNoCheck(int x, int y, uint32 color);
	DECLDIR void SetPixelNoCheck(int index, uint32 color);

	DECLDIR bool GetPixel(int x, int y, uint32* outColor);
	DECLDIR bool GetPixel(int index, uint32* outColor);

	DECLDIR uint32 GetPixelNoCheck(int x, int y);
	DECLDIR uint32 GetPixelNoCheck(int index);

	DECLDIR void Fill(uint32 color);

	DECLDIR bool IsWithinClient(int x, int y);
	DECLDIR bool IsWithinClient(int index);

	DECLDIR bool UpdateClient();

	DECLDIR int GetClientWidth();

	DECLDIR int GetClientHeight();

	DECLDIR int GetTotalPixels();

	DECLDIR HBITMAP GetHBackBufferBitmap();
};

//For C#
extern "C" __declspec(dllexport) PixelWindow* CreatePixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int clientWidth, int clientHeight, int windowX, int windowY)
{
	return new PixelWindow(hInstance, windowText, clientWidth, clientHeight, windowX, windowY);
}

extern "C" __declspec(dllexport) void DisposePixelWindow(PixelWindow* pPixelWindow)
{
	if (pPixelWindow != NULL)
	{
		delete pPixelWindow;
		pPixelWindow = NULL;
	}
}