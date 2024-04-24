using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
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
        /// <param name="formatFlags"> ������� </param>
        public static BitmapInfo RenderText(string strText, string fontName, int fontSize, int fontStyle, int fontColor, int textAlign, int strokeSize, int strokeColor, int dimensionsWidth, int dimensionsHeight, int formatFlags)
        {

            Console.WriteLine("fontNamexxx");
            Console.WriteLine("fontName1 {0}", fontName);
            try
            {
                Console.WriteLine("fontName2 {0}", fontName);
                var font = new Font(fontName, fontSize, (System.Drawing.FontStyle)fontStyle);

                //// ��������ı�������͸�ʽ
                StringFormat stringFormat = new StringFormat(); // ���� StringFormat ����

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

                var bitmap = ImageFromText(strText, font, Color.FromArgb(fontColor), Color.FromArgb(strokeColor), dimensionsWidth, dimensionsHeight, strokeSize, stringFormat);

                return new BitmapInfo();
                //var bitmapInfo = new BitmapInfo();
                //bitmapInfo.data = BitmapToRGBA(bitmap);
                //bitmapInfo.width = bitmap.Width;
                //bitmapInfo.height = bitmap.Height;

                //return bitmapInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("RenderText Error: {0}", ex);
                return new BitmapInfo();
            }
        }

        static byte[] BitmapToRGBA(Bitmap bitmap)
        {
            // ȷ��λͼ�����ظ�ʽ��32λARGB
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new Exception("Bitmap must be in 32bppArgb format.");
            }

            // ����λͼ����������
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                // �����ֽ�����
                int byteCount = bitmapData.Stride * bitmap.Height;
                byte[] rgbaBytes = new byte[byteCount];

                // ���������ݸ��Ƶ��ֽ�������
                Marshal.Copy(bitmapData.Scan0, rgbaBytes, 0, byteCount);

                return rgbaBytes;
            }
            finally
            {
                // ����λͼ����������
                bitmap.UnlockBits(bitmapData);
            }
        }

        private static Bitmap ImageFromText(string strText, Font fnt, Color clrFore, Color clrBack, int dimensionsWidth, int dimensionsHeight, int blurAmount, StringFormat stringFormat)
        {
            Bitmap? bmpOut = null;
            var sunNum = 255; //���ε�ֵ
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                var sz = new SizeF(dimensionsWidth, dimensionsHeight);
                if (dimensionsHeight == 0 || dimensionsWidth == 0)
                {
                    sz = g.MeasureString(strText, fnt, dimensionsWidth, stringFormat);
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

                    if (blurAmount <= 0)
                    {
                        return bmp;
                    }

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

        ///// <summary>
        /////     �ı�תͼƬ
        ///// </summary>
        ///// <param name="strText"></param>
        ///// <param name="fnt"></param>
        ///// <param name="clrFore"></param>
        ///// <param name="clrBack"></param>
        ///// <param name="blurAmount"></param>
        ///// <returns></returns>
        //public static BitmapImage BitmapImageFromText(string strText, Font fnt, Color clrFore, Color clrBack,
        //    int blurAmount = 5) => BitmapToBitmapImage(ImageFromText(strText, fnt, clrFore, clrBack, blurAmount));

        //private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        bitmap.Save(stream, ImageFormat.Png); // �ӵ㣺��ʽѡBmpʱ������͸����

        //        stream.Position = 0;
        //        var result = new BitmapImage();
        //        result.BeginInit();
        //        result.CacheOption = BitmapCacheOption.OnLoad;
        //        result.StreamSource = stream;
        //        result.EndInit();
        //        result.Freeze();
        //        return result;
        //    }
        //}

    }
}
