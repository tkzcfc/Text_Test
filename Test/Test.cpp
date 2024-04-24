#include <iostream>
#include <fstream>
#include "../FancyTextToCppBridge/FancyText.h"

#if _DEBUG
#pragma comment(lib,"../FancyText/x64/Debug/FancyTextToCppBridge.lib")
#else
#pragma comment(lib,"../FancyText/x64/Release/FancyTextToCppBridge.lib")
#endif

#define ALPHA_OFFSET 24
#define RED_OFFSET 16
#define GREEN_OFFSET 8
#define BLUE_OFFSET 0

inline int argbToInt(unsigned char alpha, unsigned char red, unsigned char green, unsigned char blue) {
	return (alpha << ALPHA_OFFSET) | (red << RED_OFFSET) | (green << GREEN_OFFSET) | (blue << BLUE_OFFSET);
}

/// <summary>
/// cocos ����
/// </summary>
enum Overflow
{
	// In NONE mode, the dimensions is (0,0) and the content size will change dynamically to fit the label.
	NONE,
	/**
	*In CLAMP mode, when label content goes out of the bounding box, it will be clipped.
	*/
	CLAMP,
	/**
	* In SHRINK mode, the font size will change dynamically to adapt the content size.
	*/
	SHRINK,
	/**
	*In RESIZE_HEIGHT mode, you can only change the width of label and the height is changed automatically.
	*/
	RESIZE_HEIGHT
};


enum TextAlign
{
	CENTER = 0x33, /** Horizontal center and vertical center. */
	TOP = 0x13, /** Horizontal center and vertical top. */
	TOP_RIGHT = 0x12, /** Horizontal right and vertical top. */
	RIGHT = 0x32, /** Horizontal right and vertical center. */
	BOTTOM_RIGHT = 0x22, /** Horizontal right and vertical bottom. */
	BOTTOM = 0x23, /** Horizontal center and vertical bottom. */
	BOTTOM_LEFT = 0x21, /** Horizontal left and vertical bottom. */
	LEFT = 0x31, /** Horizontal left and vertical center. */
	TOP_LEFT = 0x11, /** Horizontal left and vertical top. */
};

int main()
{
	// �������
	int dimensionsWidth = 200;
	// �߶�����
	int dimensionsHeight = 0;
	// �������
	int overflow = Overflow::SHRINK;
	// �Ƿ������Զ�����
	bool enableWrap = true;
	// �����С
	int fontSize = 20;

	// ��߿��
	int strokeSize = 5;

	int dataLen = 0;

	auto data = FancyText::Render("�� NONE ģʽ�£��ߴ�Ϊ (0,0)�����ݴ�С����̬�������ʺϱ�ǩ��",
		"Arial", 
		fontSize,
		FancyText::FontStyle::Bold | FancyText::FontStyle::Italic,
		argbToInt(100, 255, 0, 0), // ������ɫ
		TextAlign::LEFT,
		strokeSize,
		argbToInt(255, 0, 255, 0), // �����ɫ
		dimensionsWidth, 
		dimensionsHeight,
		overflow, 
		enableWrap,
		false,
		&dataLen);

	if (dataLen == 0)
	{
		printf("render failed\n");
		return 0;
	}

	std::string filePath = "out.png";
	std::ofstream outputFile(filePath, std::ios::binary);
	// ����ļ��Ƿ�ɹ���
	if (!outputFile.is_open()) {
		std::cerr << "Error: Unable to open file: " << filePath << std::endl;
		return 1;
	}
	outputFile.write((const char*)data, dataLen);

	free(data);

	printf("render success\n");
	return 0;
}

