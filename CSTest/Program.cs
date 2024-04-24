using FancyText;
using System;
using System.Collections;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bitmapInfo = FancyText.DrawText.RenderText("AAAAA", "", 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, true, false);

            File.WriteAllBytes("out.png", bitmapInfo.data);

            Console.WriteLine("Hello, World!");
        }
    }
}