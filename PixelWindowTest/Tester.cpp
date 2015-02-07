#pragma once

#ifndef UNICODE
#define UNICODE
#endif 

#include "../PixelWindow/PixelWindow.h"

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE, PWSTR /*pCmdLine*/, int nCmdShow)
{
	PixelWindow window(hInstance, L"Test", 1280, 720);
	for (int x = 0; x < window.GetClientWidth(); x++)
	{
		for (int y = 0; y < window.GetClientHeight(); y++)
		{
			//Distance from (0, 0)
			int dist = (int)sqrt(x * x + y * y);

			uint32 color = 0xFF000000;

			color |= (dist & 0xFF) << 16;
			color |= (dist & 0xFF) << 8;
			color |= (dist & 0xFF) << 0;

			window.SetPixel(x, y, color);
		}
	}

	window.UpdateClient();

	Sleep(10000);

	delete &window;
}