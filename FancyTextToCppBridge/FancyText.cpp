#include "pch.h"
#include "FancyText.h"
#include <memory>

#using <System.Drawing.dll>

/// <summary>
/// cocos 定义
/// </summary>
enum class Overflow
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


enum StringFormatFlags
{
    // 指定文本的方向为从右向左，用于支持从右向左的文本布局，如阿拉伯语和希伯来语等。
    DirectionRightToLeft = 0x1,
    // 指定文本应该以竖直方向排列，而不是水平方向。这在某些文本布局中很有用，比如在某些亚洲语言中的垂直书写方式。
    DirectionVertical = 0x2,
    // 指定文本应该被包含在黑色方块中，以便文本在显示时更好地对齐。
    FitBlackBox = 0x4,
    // 指定应该显示格式控制字符。格式控制字符通常用于控制文本的显示方式，比如换行符、段落分隔符等。
    DisplayFormatControl = 0x20,
    // 指定不应该使用字体回退机制。如果选择的字体不支持某些字符，会导致这些字符无法正确显示。
    NoFontFallback = 0x400,
    // 指定应该测量尾部空格。这在某些情况下很有用，比如在确定文本宽度时是否考虑尾部空格的影响。
    MeasureTrailingSpaces = 0x800,
    // 指定文本不应该自动换行。当文本达到指定宽度时，不会自动换行到下一行，而是超出边界。
    NoWrap = 0x1000,
    // 指定应该限制文本的行数。超过指定行数的文本会被截断或者省略。
    LineLimit = 0x2000,
    // 指定不应该裁剪文本，即使它超出了指定的区域。这意味着超出指定区域的文本会完全显示，而不会被裁剪。
    NoClip = 0x4000
};

typedef void* (*MallocPtr)(size_t);


static MallocPtr gMallocPtr = 0;

namespace FancyText
{

    DLLEXPORT void SetMallocCallback(void* (*callback)(size_t))
    {
        gMallocPtr = callback;
    }

    DLLEXPORT unsigned char* Render(const wchar_t* text,
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
        bool outputRawData,
        size_t* dataLen)
    {
        System::String^ strText = gcnew System::String(text);
        System::String^ strFontName = gcnew System::String(fontName);
        int formatFlags = StringFormatFlags::MeasureTrailingSpaces;

        int width = dimensionsWidth;
        int height = dimensionsHeight;

        // overflow 溢出处理
        if (overflow == (int)Overflow::RESIZE_HEIGHT)
        {
            height = 0;
            enableWrap = true;
        }

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
            formatFlags,
            overflow,
            enableWrap,
            outputRawData);

        array<System::Byte>^ bytes = info->data;

        if (dataLen)
            *dataLen = bytes->LongLength;

        dimensionsWidth = info->width;
        dimensionsHeight = info->height;

        if (bytes->Length == 0)
        {
            return NULL;
        }
        pin_ptr<System::Byte> pinnedArray = &bytes[0];

        if (gMallocPtr == 0)
            gMallocPtr = malloc;

        auto data = (unsigned char*)gMallocPtr(bytes->Length);
        memcpy(data, pinnedArray, bytes->Length);

        return data;
    }
}