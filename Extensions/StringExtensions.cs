using System;

namespace laui.Extensions
{
    public static class StringExtensions
    {
        public static string MaskLeft(this String input)
        {
            return input.Substring(input.Length - 4).PadLeft(input.Length, '*');
        }

        public static string MaskRight(this string input)
        {
            return input.Substring(0, 4).PadRight(input.Length, '*');
        }
    }
}