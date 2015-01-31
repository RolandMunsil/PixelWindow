#pragma once

#ifndef UNICODE
#define UNICODE
#endif 

#include "PixelWindow.h"

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE, PWSTR /*pCmdLine*/, int nCmdShow)
{
	PixelWindow window(hInstance, L"Test", 1280, 720);
	for (int x = 0; x < window.GetClientWidth(); x++)
	{
		for (int y = 0; y < window.GetClientHeight(); y++)
		{
			//Distance from (0, 0)
			int dist = (int)sqrt(x * x + y * y);

			RGBQUAD color;
			color.rgbRed = dist & 0xFF;
			color.rgbGreen = dist & 0xFF;
			color.rgbBlue = dist & 0xFF;
			color.rgbReserved = 0xFF;

			window.SetPixel(x, y, color);
		}
	}

	window.UpdateClient();

	Sleep(10000);

	window.Destroy();
}