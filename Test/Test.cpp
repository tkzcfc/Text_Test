#include <iostream>
#include "../FancyTextToCppBridge/DrawText.h"

#pragma comment(lib,"../FancyText/x64/Debug/FancyTextToCppBridge.lib")

int main()
{
	int dimensionsWidth = 0;
	int dimensionsHeight = 0;
	int overflow = 0;
	auto data = DrawText::Render("ABC", "Arial", 30, 0, 0, 0x33, 0, 0, dimensionsWidth, dimensionsHeight, overflow);


	printf("xxx");
}

