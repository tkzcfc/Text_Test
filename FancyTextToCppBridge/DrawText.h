#ifdef CSHARPTOCPPBRIDGE_EXPORTS
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT __declspec(dllimport)
#endif

#ifndef DRAW_TEXT_H
#define DRAW_TEXT_H

class DLLEXPORT  DrawText
{
public:

    enum FontStyle
    {
        Regular = 0x0,
        Bold = 0x1,
        Italic = 0x2,
        Underline = 0x4,
        Strikeout = 0x8
    };

	static unsigned char* Render(const char* text,
		const char* fontName,
		int fontSize,
		int fontStyle,
		int fontColor,
		int textAlign,
		int strokeSize,
		int strokeColor,
		int& dimensionsWidth,
		int& dimensionsHeight,
		int overflow,
		bool enableWrap,
		int* dataLen);
};
#endif
