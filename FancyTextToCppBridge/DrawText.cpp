#include "pch.h"
#include "DrawText.h"
#include <memory>

#using <System.Drawing.dll>

unsigned char* DrawText::Render(const char* text,
    const char* fontName,
    int fontSize,
    int fontStyle,
    int fontColor,
    int textAlign,
    int strokeSize,
    int strokeColor,
    int& dimensionsWidth,
    int& dimensionsHeight,
    int overflow)
{
    printf("text:%s\n", text);
    printf("fontName:%s\n", fontName);
    System::String^ strText = gcnew System::String(text);
    System::String^ strFontName = gcnew System::String(fontName);
    int formatFlags = 0;

    int width = dimensionsWidth;
    int height = dimensionsHeight;
    FancyText::BitmapInfo^ info = FancyText::DrawText::RenderText(strText,
        strFontName,
        fontSize,
        fontStyle,
        fontColor,
        textAlign,
        strokeSize,
        strokeColor,
        width,
        height,
        formatFlags);

    array<System::Byte>^ bytes = info->data;

    if (bytes->Length == 0)
    {
        return NULL;
    }
    pin_ptr<System::Byte> pinnedArray = &bytes[0];

    auto data = (unsigned char*)malloc(bytes->Length);
    memcpy(data, pinnedArray, bytes->Length);

    return data;
}