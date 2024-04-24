#include "pch.h"
#include "FancyText.h"
#include <memory>

#using <System.Drawing.dll>

/// <summary>
/// cocos ����
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
    // ָ���ı��ķ���Ϊ������������֧�ִ���������ı����֣��簢�������ϣ������ȡ�
    DirectionRightToLeft = 0x1,
    // ָ���ı�Ӧ������ֱ�������У�������ˮƽ��������ĳЩ�ı������к����ã�������ĳЩ���������еĴ�ֱ��д��ʽ��
    DirectionVertical = 0x2,
    // ָ���ı�Ӧ�ñ������ں�ɫ�����У��Ա��ı�����ʾʱ���õض��롣
    FitBlackBox = 0x4,
    // ָ��Ӧ����ʾ��ʽ�����ַ�����ʽ�����ַ�ͨ�����ڿ����ı�����ʾ��ʽ�����绻�з�������ָ����ȡ�
    DisplayFormatControl = 0x20,
    // ָ����Ӧ��ʹ��������˻��ơ����ѡ������岻֧��ĳЩ�ַ����ᵼ����Щ�ַ��޷���ȷ��ʾ��
    NoFontFallback = 0x400,
    // ָ��Ӧ�ò���β���ո�����ĳЩ����º����ã�������ȷ���ı����ʱ�Ƿ���β���ո��Ӱ�졣
    MeasureTrailingSpaces = 0x800,
    // ָ���ı���Ӧ���Զ����С����ı��ﵽָ�����ʱ�������Զ����е���һ�У����ǳ����߽硣
    NoWrap = 0x1000,
    // ָ��Ӧ�������ı�������������ָ���������ı��ᱻ�ضϻ���ʡ�ԡ�
    LineLimit = 0x2000,
    // ָ����Ӧ�òü��ı�����ʹ��������ָ������������ζ�ų���ָ��������ı�����ȫ��ʾ�������ᱻ�ü���
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

        // overflow �������
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