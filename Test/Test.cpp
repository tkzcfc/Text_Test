#include <iostream>
#include <fstream>
#include "../FancyTextToCppBridge/DrawText.h"

#pragma comment(lib,"../FancyText/x64/Debug/FancyTextToCppBridge.lib")

#define ALPHA_OFFSET 24
#define RED_OFFSET 16
#define GREEN_OFFSET 8
#define BLUE_OFFSET 0

inline int argbToInt(unsigned char alpha, unsigned char red, unsigned char green, unsigned char blue) {
	return (alpha << ALPHA_OFFSET) | (red << RED_OFFSET) | (green << GREEN_OFFSET) | (blue << BLUE_OFFSET);
}

int main()
{
	int dimensionsWidth = 200;
	int dimensionsHeight = 100;
	int overflow = 2;
	int dataLen = 0;
	bool enableWrap = true;
	auto data = DrawText::Render("在 NONE 模式下，尺寸为 (0,0)，内容大小将动态更改以适合标签。", 
		"Arial", 
		30, 
		DrawText::FontStyle::Bold | DrawText::FontStyle::Italic,
		argbToInt(100, 255, 0, 0), 
		0x33, 
		5, 
		argbToInt(255, 0, 255, 0), 
		dimensionsWidth, 
		dimensionsHeight,
		overflow, 
		enableWrap,
		&dataLen);

	if (dataLen == 0)
	{
		printf("render failed\n");
		return 0;
	}

	std::string filePath = "out.png";
	std::ofstream outputFile(filePath, std::ios::binary);
	// 检查文件是否成功打开
	if (!outputFile.is_open()) {
		std::cerr << "Error: Unable to open file: " << filePath << std::endl;
		return 1;
	}
	outputFile.write((const char*)data, dataLen);

	free(data);

	printf("render success\n");
	return 0;
}

