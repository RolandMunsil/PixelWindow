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

class DECLDIR PixelWindow
{
private:
	HWND hWindow;
	uint32* pBackBufferPixels;
	HDC hBackBufferDC;

	int width;
	int height;

	HANDLE hWindowCreatedEvent;

	std::thread messageLoopThread;

	static LRESULT CALLBACK WindowProcedure(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	void CreateWindowAndRunMessageLoop(HINSTANCE hInstance, RECT windowRect, LPCWSTR windowText);

public:
	PixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int windowWidth, int windowHeight, int windowX = 0, int windowY = 0);
	~PixelWindow();

	bool SetPixel(int x, int y, uint32 color);
	bool SetPixel(int index, uint32 color);

	//TODO: compare performance of check vs. nocheck and decide whether to keep NoCheck methods
	void SetPixelNoCheck(int x, int y, uint32 color);
	void SetPixelNoCheck(int index, uint32 color);

	uint32 GetPixel(int x, int y);
	uint32 GetPixel(int index);

	uint32 GetPixelNoCheck(int x, int y);
	uint32 GetPixelNoCheck(int index);

	void Fill(uint32 color);

	bool IsWithinClient(int x, int y);
	bool IsWithinClient(int index);

	void UpdateClient();

	int GetClientWidth();

	int GetClientHeight();

	int GetTotalPixels();
};

//For C#
extern "C" __declspec(dllexport) PixelWindow* CreatePixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int windowWidth, int windowHeight, int windowX, int windowY)
{
	return new PixelWindow(hInstance, windowText, windowWidth, windowHeight, windowX, windowY);
}

extern "C" __declspec(dllexport) void DisposePixelWindow(PixelWindow* pPixelWindow)
{
	if (pPixelWindow != NULL)
	{
		delete pPixelWindow;
		pPixelWindow = NULL;
	}
}