using UnityEngine;

namespace MolecularLib.Helpers
{
    public static class Maths
    {
        /// <summary>Interpolates linearly (and unclamped) between a and b by the given t value. (to get clamped lerp use UnityEngine.Mathf.Lerp())</summary>
        /// <param name="a">"A" value, when t = 0 it will return this.</param>
        /// <param name="b">"B" value, when t = 1 it will return this.</param>
        /// <param name="t">t, kind of the percentage between a and b. The value of the interpolation.</param>
        /// <returns>the linear interpolation between A and B by the given t: t = 0 -&gt; A; t = 1 -&gt; B; 0 &lt;  t &lt;  1 -&gt; a value between A and B.</returns>
        public static float Lerp(float a, float b, float t) => (1f - t) * a + b * t;

        /// <summary>Finds the value for t where Lerp(a, b, t) equals v (value). (Unclamped, for clamped values use UnityEngine.Mathf.InverseLerp()</summary>
        /// <param name="a">"A" value, when "V" equals "A" returns 0.</param>
        /// <param name="b">"B" value, when "V" equals "B" returns 1.</param>
        /// <param name="v">The lerped value between "A" and "B" (v = Lerp(a, b, t -&gt; return value of InvLerp()).</param>
        /// <returns>
        /// Returns the value of t where Lerp(a, b, t) equals v (value). If A equals V, than it will return 0. If B equals V, than it will return 1.
        /// If A &lt; V &lt; B, then it will return a value between 0 and 1.
        /// </returns>
        public static float InvLerp(float a, float b, float v) => (v - a)/(b - a);

        /// <summary>
        /// Remaps the specified value (v) from the iMin, iMax range to the oMin, oMax range. (Unclamped, for clamped use RemapClamped)
        /// </summary>
        /// <param name="iMin">The minimum range value for the input.</param>
        /// <param name="iMax">The maximum range value for the input.</param>
        /// <param name="oMin">The minimum range value for the output. (Since it is unclamped, it might return values smaller than this depending on the value v)</param>
        /// <param name="oMax">The maximum range value for the output. (Since it is unclamped, it might return values bigger than this depending on the value v)</param>
        /// <param name="v">The value to be remapped</param>
        /// <returns>The value (v) remapped from the input range to the output range</returns>
        public static float Remap(float iMin, float iMax, float oMin, float oMax, float v) => Lerp(oMin, oMax, InvLerp(iMin, iMax, v));

        /// <summary>
        /// Remaps the specified value (v) from the iMin, iMax range to the oMin, oMax range. (Clamped, for unclamped use Remap)
        /// </summary>
        /// <param name="iMin">The minimum range value for the input.</param>
        /// <param name="iMax">The maximum range value for the input.</param>
        /// <param name="oMin">The minimum range value for the output.</param>
        /// <param name="oMax">The maximum range value for the output.</param>
        /// <param name="v">The value to be remapped</param>
        /// <returns>The value (v) remapped from the input range to the output range</returns>
        public static float RemapClamped(float iMin, float iMax, float oMin, float oMax, float v) => Mathf.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, v));

        /// <summary>Finds the value for t where Lerp(a, b, t) equals v (value). (Unclamped, for clamped values use InvLerpClamped()</summary>
        /// <param name="a">"A" value, when "V" equals "A" returns 0.</param>
        /// <param name="b">"B" value, when "V" equals "B" returns 1.</param>
        /// <param name="v">The lerped value between "A" and "B" (v = Lerp(a, b, t -&gt; return value of InvLerp()).</param>
        /// <returns>
        /// Returns the value of t where Lerp(a, b, t) equals v (value). If A equals V, than it will return 0. If B equals V, than it will return 1.
        /// If A &lt; V &lt; B, then it will return a value between 0 and 1. In case the V Vector is not on the line defined by A and B, it will return the Inverse Lerp of the perpendicular projection of the value. It's like ignoring the coordinates that don't fit the AB system. Might return unexpected values in that case, but mostly works extremely fine.
        /// </returns>
        public static float InvLerp(Vector3 a, Vector3 b, Vector3 v)
        {
            var ab = b - a;
            var av = v - a;
            return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
        }

        /// <summary>Finds the value for t where Lerp(a, b, t) equals v (value). (Unclamped, for clamped values use InvLerpClamped()</summary>
        /// <param name="a">"A" value, when "V" equals "A" returns 0.</param>
        /// <param name="b">"B" value, when "V" equals "B" returns 1.</param>
        /// <param name="v">The lerped value between "A" and "B" (v = Lerp(a, b, t -&gt; return value of InvLerp()).</param>
        /// <returns>
        /// Returns the value of t where Lerp(a, b, t) equals v (value). If A equals V, than it will return 0. If B equals V, than it will return 1.
        /// If A &lt; V &lt; B, then it will return a value between 0 and 1. In case the V Vector is not on the line defined by A and B, it will return the Inverse Lerp of the perpendicular projection of the value. It's like ignoring the coordinates that don't fit the AB system. Might rarely return unexpected values in that case, but mostly works extremely fine.
        /// </returns>
        public static float InvLerp(Vector2 a, Vector2 b, Vector2 v)
        {
            var ab = b - a;
            var av = v - a;
            return Vector2.Dot(av, ab) / Vector2.Dot(ab, ab);
        }

        /// <summary>Finds the value for t where Lerp(a, b, t) equals v (value). (Clamped, for unclamped values use InvLerp()</summary>
        /// <param name="a">"A" value, when "V" equals "A" returns 0.</param>
        /// <param name="b">"B" value, when "V" equals "B" returns 1.</param>
        /// <param name="v">The lerped value between "A" and "B" (v = Lerp(a, b, t -&gt; return value of InvLerp()).</param>
        /// <returns>
        /// Returns the value of t where Lerp(a, b, t) equals v (value). If A equals V, than it will return 0. If B equals V, than it will return 1.
        /// If A &lt; V &lt; B, then it will return a value between 0 and 1. In case the V Vector is not on the line defined by A and B, it will return the Inverse Lerp of the perpendicular projection of the value. It's like ignoring the coordinates that don't fit the AB system. Might rarely eturn unexpected values in that case, but mostly works extremely fine.
        /// </returns>
        public static float InvLerpClamped(Vector3 a, Vector3 b, Vector3 v) => Mathf.Clamp01(InvLerp(a, b, v));

        /// <summary>Finds the value for t where Lerp(a, b, t) equals v (value). (Clamped, for unclamped values use InvLerp()</summary>
        /// <param name="a">"A" value, when "V" equals "A" returns 0.</param>
        /// <param name="b">"B" value, when "V" equals "B" returns 1.</param>
        /// <param name="v">The lerped value between "A" and "B" (v = Lerp(a, b, t -&gt; return value of InvLerp()).</param>
        /// <returns>
        /// Returns the value of t where Lerp(a, b, t) equals v (value). If A equals V, than it will return 0. If B equals V, than it will return 1.
        /// If A &lt; V &lt; B, then it will return a value between 0 and 1. In case the V Vector is not on the line defined by A and B, it will return the Inverse Lerp of the perpendicular projection of the value. It's like ignoring the coordinates that don't fit the AB system. Might return unexpected values in that case, but mostly works extremely fine.
        /// </returns>
        public static float InvLerpClamped(Vector2 a, Vector2 b, Vector2 v) => Mathf.Clamp01(InvLerp(a, b, v));

        /// <summary>
        /// Checks if the toBeGreater x,y and z are all greater than the reference x,y and z.
        /// </summary>
        /// <param name="reference">The one that is should be smaller</param>
        /// <param name="toBeGreater">The one that should be bigger</param>
        /// <returns>toBeGreater.x &gt; reference.x and toBeGreater.y &gt; reference.y and toBeGreater.z &gt; reference.z</returns>
        public static bool IsAllGreaterThan(Vector3 reference, Vector3 toBeGreater)
            => toBeGreater.x > reference.x && toBeGreater.y > reference.y && toBeGreater.z > reference.z;

        /// <summary>
        /// Checks if the toBeGreater x,y and z are all smaller than the reference x,y and z.
        /// </summary>
        /// <param name="reference">The one that is should be bigger</param>
        /// <param name="toBeSmaller">The one that should be smaller</param>
        /// <returns>toBeSmaller.x &lt; reference.x and toBeSmaller.y  &lt; reference.y and toBeSmaller.z &lt; reference.z</returns>
        public static bool IsAllSmallerThan(Vector3 reference, Vector3 toBeSmaller)
            => toBeSmaller.x < reference.x && toBeSmaller.y < reference.y && toBeSmaller.z < reference.z;

        /// <summary>
        /// Checks if the toBeGreater x and y are all greater than the reference x and y.
        /// </summary>
        /// <param name="reference">The one that is should be smaller</param>
        /// <param name="toBeGreater">The one that should be bigger</param>
        /// <returns>toBeGreater.x &gt; reference.x and toBeGreater.y &gt; reference.y</returns>
        public static bool IsAllGreaterThan(Vector2 reference, Vector2 toBeGreater)
            => toBeGreater.x > reference.x && toBeGreater.y > reference.y;

        /// <summary>
        /// Checks if the toBeGreater x and y are all smaller than the reference x and y.
        /// </summary>
        /// <param name="reference">The one that is should be bigger</param>
        /// <param name="toBeSmaller">The one that should be smaller</param>
        /// <returns>toBeSmaller.x &lt; reference.x and toBeSmaller.y &lt; reference.y</returns>
        public static bool IsAllSmallerThan(Vector2 reference, Vector2 toBeSmaller)
            => toBeSmaller.x < reference.x && toBeSmaller.y < reference.y;
        
        /// <summary>
        /// Checks if a value is greater or equal than a min and smaller or equal than a max
        /// </summary>
        /// <param name="v">The value to be within the min and max</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>True if the value is greater or equal than the min and smaller or equal than the max</returns>
        public static bool IsWithin(this float v, float min, float max) => v >= min && v <= max;
        
        /// <summary>
        /// Checks if a value is greater than a min and smaller than a max
        /// </summary>
        /// <param name="v">The value to be between the min and max</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>True if the value is greater than the min and smaller than the max</returns>
        public static bool IsBetween(this float v, float min, float max) => v > min && v < max;
    }
}
