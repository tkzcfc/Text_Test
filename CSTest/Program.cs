using FancyText;
using System;
using System.Collections;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bitmapInfo = FancyText.DrawText.RenderText("简体中文 繁體中文 Bahasa Indonesia Tiếng Việt ဗမာစာ हिन्दी 한국어 Español Português dansk български Nederlands Italiano Deutsch Français 日本語 Filipino ENGLISH ภาษาไทย বেঙ্গল 孟加拉", "", 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, true, false);

            File.WriteAllBytes("out.png", bitmapInfo.data);

            Console.WriteLine("Hello, World!");
        }
    }
}