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

class DECLDIR PixelWindow
{
private:
	HWND hWindow;
	RGBQUAD* pBackBufferPixels;
	HDC hBackBufferDC;

	int width;
	int height;

	HANDLE hWindowCreatedEvent;

	std::thread messageLoopThread;

	static LRESULT CALLBACK WindowProcedure(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	void CreateWindowAndRunMessageLoop(HINSTANCE hInstance, RECT windowRect, LPCWSTR windowText);

public:
	PixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int windowWidth, int windowHeight, int windowX = 0, int windowY = 0);

	void Destroy();

	bool SetPixel(int x, int y, RGBQUAD color);
	bool SetPixel(int index, RGBQUAD color);

	//TODO: compare performance of check vs. nocheck and decide whether to keep NoCheck methods
	void SetPixelNoCheck(int x, int y, RGBQUAD color);
	void SetPixelNoCheck(int index, RGBQUAD color);

	RGBQUAD GetPixel(int x, int y);
	RGBQUAD GetPixel(int index);

	RGBQUAD GetPixelNoCheck(int x, int y);
	RGBQUAD GetPixelNoCheck(int index);

	void Fill(RGBQUAD color);

	bool IsWithinClient(int x, int y);
	bool IsWithinClient(int index);

	void UpdateClient();

	int GetClientWidth();

	int GetClientHeight();

	int GetTotalPixels();

};

//For C#
extern "C" DECLDIR PixelWindow* CreatePixelWindow(HINSTANCE hInstance, LPCWSTR windowText, int windowWidth, int windowHeight, int windowX, int windowY)
{
	return new PixelWindow(hInstance, windowText, windowWidth, windowHeight, windowX, windowY);
}

extern "C" DECLDIR void DisposePixelWindow(PixelWindow* pPixelWindow)
{
	if (pPixelWindow != NULL)
	{
		delete pPixelWindow;
		pPixelWindow = NULL;
	}
}