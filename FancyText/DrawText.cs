using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;


//enum TextAlign
//{
//    CENTER = 0x33, /** Horizontal center and vertical center. */
//    TOP = 0x13, /** Horizontal center and vertical top. */
//    TOP_RIGHT = 0x12, /** Horizontal right and vertical top. */
//    RIGHT = 0x32, /** Horizontal right and vertical center. */
//    BOTTOM_RIGHT = 0x22, /** Horizontal right and vertical bottom. */
//    BOTTOM = 0x23, /** Horizontal center and vertical bottom. */
//    BOTTOM_LEFT = 0x21, /** Horizontal left and vertical bottom. */
//    LEFT = 0x31, /** Horizontal left and vertical center. */
//    TOP_LEFT = 0x11, /** Horizontal left and vertical top. */
//};

namespace FancyText
{
    public class BitmapInfo
    {
        public BitmapInfo()
        {
            data = new byte[0];
            width = height = 0;
        }
        public byte[] data;
        public int width;
        public int height;
    }

    public class DrawText
    {
        public static BitmapInfo TestFunc(string strText)
        {
            Console.WriteLine("TestFunc strText: {0}", strText);
            return new BitmapInfo();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strText"> �ı� </param>
        /// <param name="fontName"> �������� </param>
        /// <param name="fontSize"> �����С </param>
        /// <param name="fontStyle"> ������ �Ӵ�/��б�� </param>
        /// <param name="fontColor"> ������ɫ </param>
        /// <param name="textAlign"> ���ֶ��뷽ʽ ���� TextAlign </param>
        /// <param name="strokeSize"> ��ߴ�С </param>
        /// <param name="strokeColor"> �����ɫ </param>
        /// <param name="dimensionsWidth"> ������� </param>
        /// <param name="dimensionsHeight"> �߶����� </param>
        /// <param name="formatFlags">  </param>
        /// <param name="overflow"> ������� </param>
        /// <param name="enableWrap"> ������� </param>
        /// <param name="outputRawData"> �Ƿ񷵻�ԭʼ����,Ϊfalseʱ����png��ʽ���� </param>
        public static BitmapInfo RenderText(string strText, 
            string fontName, 
            int fontSize, 
            int fontStyle, 
            int fontColor, 
            int textAlign, 
            int strokeSize, 
            int strokeColor, 
            int dimensionsWidth, 
            int dimensionsHeight,
            int formatFlags, 
            int overflow, 
            bool enableWrap, 
            bool outputRawData)
        {
            try
            {
                var font = new Font(fontName, fontSize, (System.Drawing.FontStyle)fontStyle, GraphicsUnit.Pixel);

                //// ��������ı�������͸�ʽ
                StringFormat stringFormat = StringFormat.GenericTypographic;

                switch (textAlign)
                {
                    /** Horizontal center and vertical center. */
                    case 0x33:
                        {
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;
                        }
                        break;
                    /** Horizontal center and vertical top. */
                    case 0x13:
                        {
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Near;
                        }
                        break;
                    /** Horizontal right and vertical top. */
                    case 0x12:
                        {
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Near;
                        }
                        break;
                    /** Horizontal right and vertical center. */
                    case 0x32:
                        {
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Center;
                        }
                        break;
                    /** Horizontal right and vertical bottom. */
                    case 0x22:
                        {
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Far;
                        }
                        break;
                    /** Horizontal center and vertical bottom. */
                    case 0x23:
                        {
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Far;
                        }
                        break;
                    /** Horizontal left and vertical bottom. */
                    case 0x21:
                        {
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Far;
                        }
                        break;
                    /** Horizontal left and vertical center. */
                    case 0x31:
                        {
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Center;
                        }
                        break;
                    /** Horizontal left and vertical top. */
                    case 0x11:
                        {
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Near;
                        }
                        break;
                }
                stringFormat.FormatFlags = (StringFormatFlags)formatFlags;

                var bitmap = ImageFromText(strText, font, Color.FromArgb(fontColor), Color.FromArgb(strokeColor), dimensionsWidth, dimensionsHeight, strokeSize, stringFormat, overflow, enableWrap);

                var bitmapInfo = new BitmapInfo();
                bitmapInfo.width = bitmap.Width;
                bitmapInfo.height = bitmap.Height;
                if (outputRawData)
                {
                    bitmapInfo.data = BitmapToRGBA(bitmap);
                }
                else 
                {
                    using (var stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Png); // �ӵ㣺��ʽѡBmpʱ������͸����
                        bitmapInfo.data = stream.ToArray();
                    }
                }
                return bitmapInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("RenderText Error: {0}", ex);
                return new BitmapInfo();
            }
        }

        static byte[] BitmapToRGBA(Bitmap bitmap)
        {
            // ȷ��Ŀ��ͼ��Ĵ�С�͸�ʽ
            int width = bitmap.Width;
            int height = bitmap.Height;
            PixelFormat format = PixelFormat.Format32bppArgb;

            // ����һ���µ� Bitmap ����
            Bitmap targetBitmap = new Bitmap(width, height, format);

            // ʹ�� Graphics �������ԭʼ Bitmap ���� Bitmap ��
            using (Graphics g = Graphics.FromImage(targetBitmap))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height));
            }

            // ������ Bitmap ����������
            BitmapData bmpData = targetBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * height;
            byte[] result = new byte[bytes];

            // ���������ݸ��Ƶ� byte[]
            Marshal.Copy(ptr, result, 0, bytes);

            // ���� Bitmap
            targetBitmap.UnlockBits(bmpData);
            targetBitmap.Dispose();

            return result;
        }

        private static Bitmap ImageFromText(string strText,
            Font fnt, 
            Color clrFore, 
            Color clrBack, 
            int dimensionsWidth, 
            int dimensionsHeight, 
            int blurAmount, 
            StringFormat stringFormat,
            int overflow,
            bool enableWrap)
        {
            Bitmap? bmpOut = null;
            var sunNum = 255; //���ε�ֵ
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                var sz = new SizeF(Math.Max(dimensionsWidth - blurAmount, blurAmount + 1), Math.Max(dimensionsHeight - blurAmount, blurAmount + 1));
                if (dimensionsHeight == 0 || dimensionsWidth == 0)
                {
                    var oldMaxWidth = sz.Width;
                    sz = g.MeasureString(strText, fnt, dimensionsWidth, stringFormat);
                    if (dimensionsHeight == 0)
                    {
                        sz.Width = Math.Max(sz.Width, oldMaxWidth);
                    }
                }
                else
                {
                    // ��̬���������С
                    if (overflow == 2)
                    {
                        fnt = GetAdjustedFont(g, strText, fnt, sz, stringFormat, enableWrap);
                    }
                    else if(!enableWrap)
                    {
                        stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                    }
                }

                using (var bmp = new Bitmap((int)sz.Width, (int)sz.Height))
                using (var gBmp = Graphics.FromImage(bmp))
                using (var brBack = new SolidBrush(Color.FromArgb(sunNum, clrBack.R, clrBack.G, clrBack.B)))
                using (var brFore = new SolidBrush(clrFore))
                {
                    RectangleF layoutRect = new RectangleF(0, 0, sz.Width, sz.Height); // ��������ı�������

                    gBmp.SmoothingMode = SmoothingMode.HighQuality;
                    gBmp.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    gBmp.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    gBmp.DrawString(strText, fnt, brBack, layoutRect, stringFormat);

                    bmpOut = new Bitmap(bmp.Width + blurAmount, bmp.Height + blurAmount);
                    using (var gBmpOut = Graphics.FromImage(bmpOut))
                    {
                        gBmpOut.SmoothingMode = SmoothingMode.HighQuality;
                        gBmpOut.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        gBmpOut.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        //��Ӱ����
                        for (var x = 0; x <= blurAmount; x++)
                        {
                            for (var y = 0; y <= blurAmount; y++)
                            {
                                gBmpOut.DrawImageUnscaled(bmp, x, y);
                            }
                        }

                        layoutRect.X = blurAmount / 2;
                        layoutRect.Y = blurAmount / 2;
                        gBmpOut.DrawString(strText, fnt, brFore, layoutRect, stringFormat);
                    }
                }
            }

            return bmpOut;
        }

        static Font GetAdjustedFont(Graphics graphics, string text, Font originalFont, SizeF layoutSize, StringFormat stringFormat, bool enableWrap)
        {
            var actualWidth = (int)layoutSize.Width + 1;
            var actualHeight = (int)layoutSize.Height + 1;
            var newFontSize = originalFont.Size + 1;

            while (actualWidth > (int)layoutSize.Width || actualHeight > (int)layoutSize.Height)
            {
                if (newFontSize <= 0)
                {
                    break;
                }

                var font = new Font(originalFont.FontFamily, newFontSize, originalFont.Style, originalFont.Unit);

                SizeF stringSize;
                if (enableWrap)
                {
                    stringSize = graphics.MeasureString(text, font, (int)layoutSize.Width, stringFormat);
                }
                else
                {
                    stringSize = graphics.MeasureString(text, font, 0, stringFormat);
                }

                actualWidth  = (int)stringSize.Width;
                actualHeight = (int)stringSize.Height;
                newFontSize--;
            }

            return new Font(originalFont.FontFamily, newFontSize, originalFont.Style, originalFont.Unit);
        }
    }
}
