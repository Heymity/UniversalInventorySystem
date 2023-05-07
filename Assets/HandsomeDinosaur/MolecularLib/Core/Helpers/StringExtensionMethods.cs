using System;
using System.Text;
using UnityEngine;

namespace MolecularLib.Helpers
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Gets a color based on the provided string
        /// </summary>
        /// <param name="str">The string to get the color from</param>
        /// <param name="minRGB">The minimum RGB values</param>
        /// <param name="maxRGB">The maximum RGB values</param>
        /// <returns>A color based from the provided string</returns>
        public static Color ToColor(this string str, byte minRGB = 0, byte maxRGB = 255) => ColorHelper.FromString(str, minRGB, maxRGB);

        /// <summary>
        /// Based on a GUIStyle and a max width, will generate a string that will just fit the width. When the string is shortened it will have "..." added to its end
        /// </summary>
        /// <param name="text">The text to ellipsis</param>
        /// <param name="maxWidth">The maximum Width that the text can be</param>
        /// <param name="style">A GUI style to calculate the text size</param>
        /// <returns>A text that will always fit the maxWidth</returns>
        public static string Ellipsis(this string text, float maxWidth, GUIStyle style)
        {
            return text.Ellipsis(maxWidth, c => style.CalcSize(c).x);
        }

        /// <summary>
        /// Based on a function to calculate the text width and a max width, will generate a string that will just fit the width. When the string is shortened it will have "..." added to its end
        /// </summary>
        /// <param name="text">The text to ellipsis</param>
        /// <param name="maxWidth">The maximum Width that the text can be</param>
        /// <param name="calcWidthFunc">A function to calculate the text size</param>
        /// <returns>A text that will always fit the maxWidth</returns>
        public static string Ellipsis(this string text, float maxWidth, Func<GUIContent, float> calcWidthFunc)
        {
            var textDimensions = calcWidthFunc(new GUIContent(text));
            if (textDimensions <= maxWidth) return text;

            var newStr = text;
            var threeDotsSize = calcWidthFunc(new GUIContent("..."));
            while (calcWidthFunc(new GUIContent(newStr)) > maxWidth - threeDotsSize)
            {
                if (newStr.Length - 1 < 0) return "...";
                newStr = newStr.Remove(newStr.Length - 1);
            }

            return newStr + "...";
        }

        /// <summary>
        /// Will add the rich text tags for a colored text
        /// </summary>
        /// <param name="str">The string to be colored</param>
        /// <param name="color">To color to apply to the string</param>
        /// <returns>A rich text colored string</returns>
        public static string Color(this string str, Color color) =>
            $"<color={color.ToHexString()}>{str}</color>";

        /// <summary>
        /// Will add the rich text tags for a bold text
        /// </summary>
        /// <param name="str">The string to be in bold</param>
        /// <returns>A rich text bold string</returns>
        public static string Bold(this string str) => $"<b>{str}</b>";
        
        /// <summary>
        /// Will add the rich text tags for a italic text
        /// </summary>
        /// <param name="str">The string to be in italics</param>
        /// <returns>A rich text italic string</returns>
        public static string Italic(this string str) => $"<i>{str}</i>";

        /// <summary>
        /// Will add the rich text tags for controlling ths text size
        /// </summary>
        /// <param name="str">The string to be sized</param>
        /// <param name="pixelSize">The size of the text</param>
        /// <returns>A rich text sized string</returns>
        public static string Size(this string str, float pixelSize) => $"<size={pixelSize}>{str}</size>";

        /// <summary>
        /// Add "\n" to the end of the string
        /// </summary>
        /// <param name="str">The string to be new lined</param>
        /// <returns>The provided string plus a new line at the end</returns>
        public static string NewLine(this string str) => str + "\n";
        
        /// <summary>
        /// Get a string builder with the provided string already appended. Very useful for text manipulation, since StringBuilders are way faster than just adding or modifying strings.
        /// </summary>
        /// <param name="str">The string to append</param>
        /// <returns>A string builder with the provided string already appended</returns>
        public static StringBuilder ToBuilder(this string str)
        {
            var builder = new StringBuilder();

            builder.Append(str);

            return builder;
        }
        
        /// <summary>
        /// Will add the rich text tags for a colored text
        /// </summary>
        /// <param name="builder">The builder to add the color to</param>
        /// <param name="color">To color to apply to the string</param>
        /// <returns>A rich text colored string in a string builder</returns>
        public static StringBuilder Color(this StringBuilder builder, Color color) 
            => builder.Insert(0, $"<color={color.ToHexString()}>").Append("</color>");

        /// <summary>
        /// Will add the rich text tags for a bold text
        /// </summary>
        /// <param name="builder">The builder to add the bold to</param>
        /// <returns>A rich text bold string in a string builder</returns>
        public static StringBuilder Bold(this StringBuilder builder)
            => builder.Insert(0, "<b>").Append("</b>");
        
        /// <summary>
        /// Will add the rich text tags for a italic text
        /// </summary>
        /// <param name="builder">The builder to add the italics to</param>
        /// <returns>A rich text italic string in a string builder</returns>
        public static StringBuilder Italic(this StringBuilder builder) => builder.Insert(0, "<i>").Append("</i>");

        /// <summary>
        /// Will add the rich text tags for controlling the size of the string
        /// </summary>
        /// <param name="builder">The builder to add the color to</param>
        /// <param name="pixelSize">The size of the text</param>
        /// <returns>A rich text sized string in a string builder</returns>
        public static StringBuilder Size(this StringBuilder builder, float pixelSize) =>  builder.Insert(0, $"<size={pixelSize}>").Append("</size>");

        /// <summary>
        /// builder.AppendLine();
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static StringBuilder NewLine(this StringBuilder builder) => builder.AppendLine();
    }
}
