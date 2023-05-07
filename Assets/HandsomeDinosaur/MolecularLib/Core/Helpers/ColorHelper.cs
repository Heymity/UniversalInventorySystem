using System;
using System.Globalization;
using UnityEngine;

namespace MolecularLib.Helpers
{
    public static class ColorHelper
    {
        public static readonly Color DarkTextColor = NormalizeToColor(16, 16, 16);
        public static readonly Color LightTextColor = NormalizeToColor(201, 201, 201);

        
        /// <summary>
        /// Gets a totally random color
        /// </summary>
        /// <returns>A random color</returns>
        public static Color Random() => NormalizeToColor((byte) UnityEngine.Random.Range(0, 255),
            (byte) UnityEngine.Random.Range(0, 255), (byte) UnityEngine.Random.Range(0, 255));
        
        /// <summary>
        /// Gets a random color with a minimum and maximum RGB values
        /// </summary>
        /// <param name="minRGB">the minimum RBG value</param>
        /// <param name="maxRGB">the maximum RBG value</param>
        /// <returns>Random color between the min and max RGB values</returns>
        public static Color Random(byte minRGB, byte maxRGB)
        {
            var span = (maxRGB + 1) - minRGB;
            
            return NormalizeToColor(
                (byte) (UnityEngine.Random.Range(0, 255) % span + minRGB),
                (byte) (UnityEngine.Random.Range(0, 255) % span + minRGB), 
                (byte) (UnityEngine.Random.Range(0, 255) % span + minRGB));
        }

        /// <summary>
        /// Gets a color from a string based on its hash
        /// </summary>
        /// <param name="value">The string to generate the color</param>
        /// <param name="minRGB">the minimum RBG value</param>
        /// <param name="maxRGB">the maximum RBG value</param>
        /// <returns>A color based on the provided string</returns>
        public static Color FromString(string value, byte minRGB = 0, byte maxRGB = 255)
        {
            if (value.Length == 0) return Color.white;
            
            var divider = value.Length >= 3 ? Mathf.CeilToInt(value.Length / 3f) : 0;

            var span = (maxRGB + 1) - minRGB;

            var r = Mathf.Abs(value.Substring(0, divider).GetHashCode() % span) + minRGB;
            var g = Mathf.Abs(value.Substring(divider, divider).GetHashCode() % span) + minRGB;
            var b = Mathf.Abs(value.Substring(2 * divider).GetHashCode() % span) + minRGB;

            return new Color(r / 255f, g / 255f, b / 255f);

        }

        /// <summary>
        /// Gets a text color (DarkTextColor or LightTextColor) based on the background color luminance, calculated by (0.299 * background.r + 0.587 * background.g + 0.114 * background.b)
        /// </summary>
        /// <param name="background">The background color</param>
        /// <returns>Either DarkTextColor or LightTextColor, depending on the luminance</returns>
        public static Color GetTextColorFromBackground(Color background) => TextColorShouldBeDark(background) ? DarkTextColor : LightTextColor;

        /// <summary>
        /// Gets whether the text color should be dark or light based on the background color luminance, calculated by (0.299 * background.r + 0.587 * background.g + 0.114 * background.b)
        /// </summary>
        /// <param name="backgroundColor">The background color</param>
        /// <returns>True if the text color should be dark, false if should be light</returns>
        public static bool TextColorShouldBeDark(Color backgroundColor)
        {
            var luminance = (0.299 * backgroundColor.r + 0.587 * backgroundColor.g + 0.114 * backgroundColor.b);

            return luminance > 0.5;
        }

        /// <summary>
        /// Gets a color from bytes, not floats
        /// </summary>
        /// <param name="r">R value, 0 to 255</param>
        /// <param name="g">G value, 0 to 255</param>
        /// <param name="b">B value, 0 to 255</param>
        /// <param name="a">A value, 0 to 255, default is 255</param>
        /// <returns></returns>
        public static Color NormalizeToColor(byte r, byte g, byte b, byte a = 255) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);

        /// <summary>
        /// Gets a color based on its hex string
        /// </summary>
        /// <param name="hex">The hex string</param>
        /// <returns>The color represented by the hex string</returns>
        public static Color FromHex(string hex)
        {
            if (hex[0] == '#') hex = hex.Substring(1);

            byte r = 0;
            byte g = 0;
            byte b = 0;
            byte a = 255;
            if (hex.Length >= 6)
            {
                r = (byte)int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                g = (byte)int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                b = (byte)int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                if (hex.Length == 8) a = (byte)int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
            }
            else if (hex.Length >= 3)
            {
                r = (byte)int.Parse(hex.Substring(0, 1) + hex.Substring(0, 1), NumberStyles.HexNumber);
                g = (byte)int.Parse(hex.Substring(1, 1) + hex.Substring(1, 1), NumberStyles.HexNumber);
                b = (byte)int.Parse(hex.Substring(2, 1) + hex.Substring(2, 1), NumberStyles.HexNumber);
                if (hex.Length == 4)
                    a = (byte)int.Parse(hex.Substring(3, 1) + hex.Substring(3, 1), NumberStyles.HexNumber);
            }

            return NormalizeToColor(r, g, b, a);
        }
        
        /// <summary>
        /// Converts a color to a Color32
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>The provided color as a Color32</returns>
        public static Color32 ToColor32(this Color color)
        {
            return new Color32((byte) (color.r * 255), (byte) (color.g * 255), (byte) (color.b * 255), (byte) (color.a * 255));
        }

        /// <summary>
        /// Gets the hex string that represents the provided color
        /// </summary>
        /// <param name="color">The color to get the hex string</param>
        /// <param name="addSharpAtStart">Whether the returned string should have a # at the beginning</param>
        /// <returns>The hex string of the provided color</returns>
        public static string ToHexString(this Color color, bool addSharpAtStart = true)
        {
            var r = (byte) (color.r * 255);
            var g = (byte) (color.g * 255);
            var b = (byte) (color.b * 255);
            var a = (byte) (color.a * 255);

            var colorBytes = new[] { r, g, b, a };

            var hex = (addSharpAtStart ? "#" : string.Empty) + BitConverter.ToString(colorBytes).Replace("-", string.Empty);
            return hex;
        }
        
        /// <summary>
        /// Gets the hex string that represents the provided color ignoring the alpha channel
        /// </summary>
        /// <param name="color">The color to get the hex string</param>
        /// <param name="addSharpAtStart">Whether the returned string should have a # at the beginning</param>
        /// <param name="add00ToEnd">Whether the returned string should have a 00 at the place of the alpha</param>
        /// <returns>The hex string of the provided color ignoring the alpha channel</returns>
        public static string ToHexStringNoAlpha(this Color color, bool addSharpAtStart = true, bool add00ToEnd = true)
        {
            var r = (byte) (color.r * 255);
            var g = (byte) (color.g * 255);
            var b = (byte) (color.b * 255);

            var colorBytes = new[] { r, g, b };

            var hex = (addSharpAtStart ? "#" : string.Empty) + BitConverter.ToString(colorBytes).Replace("-", string.Empty) + (add00ToEnd ? "00" : string.Empty);
            return hex;
        }

        /// <summary>
        /// Gets a text color (DarkTextColor or LightTextColor) based on the background color luminance, calculated by (0.299 * background.r + 0.587 * background.g + 0.114 * background.b)
        /// </summary>
        /// <param name="backgroundColor">The background color</param>
        /// <returns>Either DarkTextColor or LightTextColor, depending on the luminance</returns>
        public static Color TextForegroundColor(this Color backgroundColor) =>
            GetTextColorFromBackground(backgroundColor);
        
        /// <summary>
        /// Gets whether the text color should be dark or light based on the background color luminance, calculated by (0.299 * background.r + 0.587 * background.g + 0.114 * background.b)
        /// </summary>
        /// <param name="backgroundColor">The background color</param>
        /// <returns>True if the text color should be dark, false if should be light</returns>
        public static bool TextForegroundColorShouldBeDark(this Color backgroundColor) =>
            TextColorShouldBeDark(backgroundColor);

        /// <summary>
        /// Creates a new color with all the same values, except in the R channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="r">The new R value</param>
        /// <returns>A new color with all the same values except the red, which will have the provided value</returns>
        public static Color WithR(this Color color, byte r) => new Color(r / 255f, color.g, color.b, color.a);
        /// <summary>
        /// Creates a new color with all the same values, except in the R channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="r">The new R value</param>
        /// <returns>A new color with all the same values except the red, which will have the provided value</returns>
        public static Color WithR(this Color color, float r) => new Color(r, color.g, color.b, color.a);
        
        /// <summary>
        /// Creates a new color with all the same values, except in the G channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="g">The new G value</param>
        /// <returns>A new color with all the same values except the green, which will have the provided value</returns>
        public static Color WithG(this Color color, byte g) => new Color(color.r, g  / 255f, color.b, color.a);
        /// <summary>
        /// Creates a new color with all the same values, except in the G channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="g">The new G value</param>
        /// <returns>A new color with all the same values except the green, which will have the provided value</returns>
        public static Color WithG(this Color color, float g) => new Color(color.r, g, color.b, color.a);
        
        /// <summary>
        /// Creates a new color with all the same values, except in the B channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="b">The new B value</param>
        /// <returns>A new color with all the same values except the blue, which will have the provided value</returns>
        public static Color WithB(this Color color, byte b) => new Color(color.r, color.g, b / 255f, color.a);
        /// <summary>
        /// Creates a new color with all the same values, except in the B channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="b">The new B value</param>
        /// <returns>A new color with all the same values except the blue, which will have the provided value</returns>
        public static Color WithB(this Color color, float b) => new Color(color.r, color.g, b, color.a);
        
        /// <summary>
        /// Creates a new color with all the same values, except in the A channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="a">The new A value</param>
        /// <returns>A new color with all the same values except the alpha, which will have the provided value</returns>
        public static Color WithA(this Color color, byte a) => new Color(color.r, color.g, color.b, a / 255f);
        /// <summary>
        /// Creates a new color with all the same values, except in the A channel, which will have the provided value
        /// </summary>
        /// <param name="color">The base color</param>
        /// <param name="a">The new A value</param>
        /// <returns>A new color with all the same values except the alpha, which will have the provided value</returns>
        public static Color WithA(this Color color, float a) => new Color(color.r, color.g, color.b, a);
    }
}
