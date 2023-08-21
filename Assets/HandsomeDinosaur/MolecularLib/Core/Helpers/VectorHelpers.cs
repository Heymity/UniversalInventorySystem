using System;
using UnityEngine;

namespace MolecularLib.Helpers
{
    public static class VectorHelperExtensionMethods
    {
        #region Vec2 With

        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="x">The new X value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided X value</returns>
        public static Vector2 WithX(this Vector2 vec, float x) => new Vector2(x, vec.y);
        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="y">The new Y value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided Y value</returns>
        public static Vector2 WithY(this Vector2 vec, float y) => new Vector2(vec.x, y);

        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="xSetter">A function to get the new X value based on the original value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided X value</returns>
        public static Vector2 WithX(this Vector2 vec, Func<float, float> xSetter) =>
            new Vector2(xSetter.Invoke(vec.x), vec.y);
        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="ySetter">A function to get the new Y value based on the original value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided Y value</returns>
        public static Vector2 WithY(this Vector2 vec, Func<float, float> ySetter) =>
            new Vector2(vec.x, ySetter.Invoke(vec.y));
        
        #endregion
        #region Vec3 With
        
        
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="x">The new X value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided X value</returns>
        public static Vector3 WithX(this Vector3 vec, float x) => new Vector3(x, vec.y, vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="y">The new Y value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Y value</returns>
        public static Vector3 WithY(this Vector3 vec, float y) => new Vector3(vec.x, y, vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Z value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="z">The new Z value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Z value</returns>
        public static Vector3 WithZ(this Vector3 vec, float z) => new Vector3(vec.x, vec.y, z);

        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="xSetter">A function to get the new X value based on the original value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided X value</returns>
        public static Vector3 WithX(this Vector3 vec, Func<float, float> xSetter) =>
            new Vector3(xSetter.Invoke(vec.x), vec.y, vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="ySetter">A function to get the new Y value based on the original value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Y value</returns>
        public static Vector3 WithY(this Vector3 vec, Func<float, float> ySetter) =>
            new Vector3(vec.x, ySetter.Invoke(vec.y), vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Z value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="zSetter">A function to get the new Z value based on the original value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Z value</returns>
        public static Vector3 WithZ(this Vector3 vec, Func<float, float> zSetter) =>
            new Vector3(vec.x, vec.y, zSetter.Invoke(vec.z));
        
        #endregion
        #region Vec4 With
        
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="x">The new X value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided X value</returns>
        public static Vector4 WithX(this Vector4 vec, float x) => new Vector4(x, vec.y, vec.z, vec.w);
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="y">The new Y value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided Y value</returns>
        public static Vector4 WithY(this Vector4 vec, float y) => new Vector4(vec.x, y, vec.z, vec.w);
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the Z value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="z">The new Z value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided Z value</returns>
        public static Vector4 WithZ(this Vector4 vec, float z) => new Vector4(vec.x, vec.y, z, vec.w);
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the W value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="w">The new W value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided W value</returns>
        public static Vector4 WithW(this Vector4 vec, float w) => new Vector4(vec.x, vec.y, vec.z, w);

        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="xSetter">A function to get the new X value based on the original value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided X value</returns>
        public static Vector4 WithX(this Vector4 vec, Func<float, float> xSetter) =>
            new Vector4(xSetter.Invoke(vec.x), vec.y, vec.z, vec.w);
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="ySetter">A function to get the new Y value based on the original value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided Y value</returns>
        public static Vector4 WithY(this Vector4 vec, Func<float, float> ySetter) =>
            new Vector4(vec.x, ySetter.Invoke(vec.y), vec.z, vec.w);
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the Z value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="zSetter">A function to get the new Z value based on the original value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided Z value</returns>
        public static Vector4 WithZ(this Vector4 vec, Func<float, float> zSetter) =>
            new Vector4(vec.x, vec.y, zSetter.Invoke(vec.z), vec.w);
        /// <summary>
        /// Creates a new Vector4 with the same values as the provided one, except the W value.
        /// </summary>
        /// <param name="vec">The original Vector4</param>
        /// <param name="wSetter">A function to get the new W value based on the original value</param>
        /// <returns>A new Vector4 with the same values as the original and the provided W value</returns>
        public static Vector4 WithW(this Vector4 vec, Func<float, float> wSetter) =>
            new Vector4(vec.x, vec.y, vec.z, wSetter.Invoke(vec.w));
        
        #endregion
        #region Vec2Int With

        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="x">The new X value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided X value</returns>
        public static Vector2Int WithX(this Vector2Int vec, int x) => new Vector2Int(x, vec.y);
        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="y">The new Y value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided Y value</returns>
        public static Vector2Int WithY(this Vector2Int vec, int y) => new Vector2Int(vec.x, y);

        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="xSetter">A function to get the new X value based on the original value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided X value</returns>
        public static Vector2Int WithX(this Vector2Int vec, Func<int, int> xSetter) =>
            new Vector2Int(xSetter.Invoke(vec.x), vec.y);
        /// <summary>
        /// Creates a new Vector2 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector2</param>
        /// <param name="ySetter">A function to get the new Y value based on the original value</param>
        /// <returns>A new Vector2 with the same values as the original and the provided Y value</returns>
        public static Vector2Int WithY(this Vector2Int vec, Func<int, int> ySetter) =>
            new Vector2Int(vec.x, ySetter.Invoke(vec.y));
        
        #endregion
        #region Vec3Int With
        
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="x">The new X value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided X value</returns>
        public static Vector3Int WithX(this Vector3Int vec, int x) => new Vector3Int(x, vec.y, vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="y">The new Y value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Y value</returns>
        public static Vector3Int WithY(this Vector3Int vec, int y) => new Vector3Int(vec.x, y, vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Z value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="z">The new Z value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Z value</returns>
        public static Vector3Int WithZ(this Vector3Int vec, int z) => new Vector3Int(vec.x, vec.y, z);

        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the X value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="xSetter">A function to get the new X value based on the original value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided X value</returns>
        public static Vector3Int WithX(this Vector3Int vec, Func<int, int> xSetter) =>
            new Vector3Int(xSetter.Invoke(vec.x), vec.y, vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Y value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="ySetter">A function to get the new Y value based on the original value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Y value</returns>
        public static Vector3Int WithY(this Vector3Int vec, Func<int, int> ySetter) =>
            new Vector3Int(vec.x, ySetter.Invoke(vec.y), vec.z);
        /// <summary>
        /// Creates a new Vector3 with the same values as the provided one, except the Z value.
        /// </summary>
        /// <param name="vec">The original Vector3</param>
        /// <param name="zSetter">A function to get the new Z value based on the original value</param>
        /// <returns>A new Vector3 with the same values as the original and the provided Z value</returns>
        public static Vector3Int WithZ(this Vector3Int vec, Func<int, int> zSetter) =>
            new Vector3Int(vec.x, vec.y, zSetter.Invoke(vec.z));
        
        #endregion
        #region ToVec2Int

        /// <summary>
        /// Creates a new Vector2Int using the values from the provided vector2
        /// </summary>
        /// <param name="vec2">the non integer vector2</param>
        /// <returns>The rounded to int vector2</returns>
        public static Vector2Int ToVec2Int(this Vector2 vec2) => new Vector2Int(Mathf.RoundToInt(vec2.x), Mathf.RoundToInt(vec2.y));
        /// <summary>
        /// Creates a new Vector2Int using the values from the provided vector3
        /// </summary>
        /// <param name="vec3">the non integer vector3</param>
        /// <returns>The rounded to int vector2</returns>
        public static Vector2Int ToVec2Int(this Vector3 vec3) => new Vector2Int(Mathf.RoundToInt(vec3.x), Mathf.RoundToInt(vec3.y));
        /// <summary>
        /// Creates a new Vector2Int using the values from the provided vector4
        /// </summary>
        /// <param name="vec4">the non integer vector4</param>
        /// <returns>The rounded to int vector2</returns>
        public static Vector2Int ToVec2Int(this Vector4 vec4) => new Vector2Int(Mathf.RoundToInt(vec4.x), Mathf.RoundToInt(vec4.y));

        #endregion
        #region ToVec3Int

        /// <summary>
        /// Creates a new Vector2Int using the values from the provided vector2
        /// </summary>
        /// <param name="vec2">the non integer vector2</param>
        /// <returns>The rounded to int vector3</returns>
        public static Vector3Int ToVec3Int(this Vector2 vec2) => new Vector3Int(Mathf.RoundToInt(vec2.x), Mathf.RoundToInt(vec2.y), 0);
        /// <summary>
        /// Creates a new Vector2Int using the values from the provided vector3
        /// </summary>
        /// <param name="vec3">the non integer vector3</param>
        /// <returns>The rounded to int vector3</returns>
        public static Vector3Int ToVec3Int(this Vector3 vec3) => new Vector3Int(Mathf.RoundToInt(vec3.x), Mathf.RoundToInt(vec3.y), Mathf.RoundToInt(vec3.z));
        /// <summary>
        /// Creates a new Vector2Int using the values from the provided vector4
        /// </summary>
        /// <param name="vec4">the non integer vector4</param>
        /// <returns>The rounded to int vector3</returns>
        public static Vector3Int ToVec3Int(this Vector4 vec4) => new Vector3Int(Mathf.RoundToInt(vec4.x), Mathf.RoundToInt(vec4.y), Mathf.RoundToInt(vec4.z));

        #endregion
        #region ToVec2

        /// <summary>
        /// Creates a new Vector2 using the values from the provided Vector2Int
        /// </summary>
        /// <param name="vec">The Vector2Int to provide the x and y values</param>
        /// <returns>A new Vector2 with the x and y values from the provided vector</returns>
        public static Vector2 ToVec2(this Vector2Int vec) => new Vector2(vec.x, vec.y);
        /// <summary>
        /// Creates a new Vector2 using the values from the provided Vector3Int
        /// </summary>
        /// <param name="vec">The Vector3Int to provide the x and y values</param>
        /// <returns>A new Vector2 with the x and y values from the provided vector</returns>
        public static Vector2 ToVec2(this Vector3Int vec) => new Vector2(vec.x, vec.y);

        #endregion
        #region ToVec3

        /// <summary>
        /// Creates a new Vector3 using the values from the provided Vector2Int
        /// </summary>
        /// <param name="vec">The Vector2Int to provide the x and y values</param>
        /// <returns>A new Vector3 using the x and y values from the provided vector with z equals 0</returns>
        public static Vector3 ToVec3(this Vector2Int vec) => new Vector3(vec.x, vec.y, 0);
        /// <summary>
        /// Creates a new Vector3 using the values from the provided Vector3Int
        /// </summary>
        /// <param name="vec">The Vector3Int to provide the x, y and z values</param>
        /// <returns>A new Vector3 using the x, y and z values from the provided vector</returns>
        public static Vector3 ToVec3(this Vector3Int vec) => new Vector3(vec.x, vec.y, vec.z);

        #endregion
        #region ToVec4

        /// <summary>
        /// Creates a new Vector4 using the values from the provided Vector2Int
        /// </summary>
        /// <param name="vec">The Vector2Int to provide the x and y values</param>
        /// <returns>A new Vector4 using the x and y values from the provided vector with z and w equals to 0</returns>
        public static Vector4 ToVec4(this Vector2Int vec) => new Vector4(vec.x, vec.y, 0, 0);
        /// <summary>
        /// Creates a new Vector4 using the values from the provided Vector2Int
        /// </summary>
        /// <param name="vec">The Vector3Int to provide the x, y and z values</param>
        /// <returns>A new Vector4 using the x, y and z values from the provided vector with w equals to 0</returns>
        public static Vector4 ToVec4(this Vector3Int vec) => new Vector4(vec.x, vec.y, vec.z, 0);

        #endregion
        #region WithoutX

        /// <summary>
        /// Sets the x value of the vector to 0
        /// </summary>
        /// <param name="vec2">The vector2 to have its x set to 0</param>
        /// <returns>The provided vector with x = 0</returns>
        public static Vector2 WithoutX(this Vector2 vec2) => vec2.WithX(0);

        /// <summary>
        /// Sets the x value of the vector to 0
        /// </summary>
        /// <param name="vec3">The vector3 to have its x set to 0</param>
        /// <returns>The provided vector with x = 0</returns>
        public static Vector3 WithoutX(this Vector3 vec3) => vec3.WithX(0);

        /// <summary>
        /// Sets the x value of the vector to 0
        /// </summary>
        /// <param name="vec4">The vector4 to have its x set to 0</param>
        /// <returns>The provided vector with x = 0</returns>
        public static Vector4 WithoutX(this Vector4 vec4) => vec4.WithX(0);
        
        /// <summary>
        /// Sets the x value of the vector to 0
        /// </summary>
        /// <param name="vec2">The vector2 to have its x set to 0</param>
        /// <returns>The provided vector with x = 0</returns>
        public static Vector2Int WithoutX(this Vector2Int vec2) => vec2.WithX(0);

        /// <summary>
        /// Sets the x value of the vector to 0
        /// </summary>
        /// <param name="vec3">The vector3 to have its x set to 0</param>
        /// <returns>The provided vector with x = 0</returns>
        public static Vector3Int WithoutX(this Vector3Int vec3) => vec3.WithX(0);

        #endregion
        #region WithoutY

        /// <summary>
        /// Sets the y value of the vector to 0
        /// </summary>
        /// <param name="vec2">The vector2 to have its y set to 0</param>
        /// <returns>The provided vector with y = 0</returns>
        public static Vector2 WithoutY(this Vector2 vec2) => vec2.WithY(0);

        /// <summary>
        /// Sets the y value of the vector to 0
        /// </summary>
        /// <param name="vec3">The vector3 to have its y set to 0</param>
        /// <returns>The provided vector with y = 0</returns>
        public static Vector3 WithoutY(this Vector3 vec3) => vec3.WithY(0);

        /// <summary>
        /// Sets the y value of the vector to 0
        /// </summary>
        /// <param name="vec4">The vector4 to have its y set to 0</param>
        /// <returns>The provided vector with y = 0</returns>
        public static Vector4 WithoutY(this Vector4 vec4) => vec4.WithY(0);
        
        /// <summary>
        /// Sets the y value of the vector to 0
        /// </summary>
        /// <param name="vec2">The vector2 to have its y set to 0</param>
        /// <returns>The provided vector with y = 0</returns>
        public static Vector2Int WithoutY(this Vector2Int vec2) => vec2.WithY(0);

        /// <summary>
        /// Sets the y value of the vector to 0
        /// </summary>
        /// <param name="vec3">The vector3 to have its y set to 0</param>
        /// <returns>The provided vector with y = 0</returns>
        public static Vector3Int WithoutY(this Vector3Int vec3) => vec3.WithY(0);

        #endregion
        #region WithoutZ

        /// <summary>
        /// Sets the z value of the vector to 0
        /// </summary>
        /// <param name="vec3">The vector3 to have its z set to 0</param>
        /// <returns>The provided vector with z = 0</returns>
        public static Vector3 WithoutZ(this Vector3 vec3) => vec3.WithZ(0);

        /// <summary>
        /// Sets the z value of the vector to 0
        /// </summary>
        /// <param name="vec4">The vector4 to have its z set to 0</param>
        /// <returns>The provided vector with z = 0</returns>
        public static Vector4 WithoutZ(this Vector4 vec4) => vec4.WithZ(0);
        
        /// <summary>
        /// Sets the z value of the vector to 0
        /// </summary>
        /// <param name="vec3">The vector3 to have its z set to 0</param>
        /// <returns>The provided vector with z = 0</returns>
        public static Vector3Int WithoutZ(this Vector3Int vec3) => vec3.WithZ(0);

        #endregion
        #region WithoutW

        /// <summary>
        /// Sets the w value of the vector to 0
        /// </summary>
        /// <param name="vec4">The vector4 to have its w set to 0</param>
        /// <returns>The provided vector with w = 0</returns>
        public static Vector4 WithoutW(this Vector4 vec4) => vec4.WithW(0);

        #endregion
        #region IsBetween&IsWithin

        /// <summary>
        /// Checks if a vector has all of his values between(greater/less) the ones of the min and max vector. In the cartesian space, where the min and max defines a square, the vector is between if it is contained in said square.
        /// </summary>
        /// <param name="vec2">The value to be between the min and max</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>True if the vector has all of his values between(greater/less) the ones of the min and max vector</returns>
        public static bool IsBetween(this Vector2 vec2, Vector2 min, Vector2 max) => vec2.x.IsBetween(min.x, max.x) && vec2.y.IsBetween(min.y, max.y);
        /// <summary>
        /// Checks if a vector has all of his values between(greater/less) the ones of the min and max vector. In the cartesian space, where the min and max defines a square, the vector is between if it is contained in said square.
        /// </summary>
        /// <param name="vec3">The value to be between the min and max</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>True if the vector has all of his values between(greater/less) the ones of the min and max vector</returns>
        public static bool IsBetween(this Vector3 vec3, Vector3 min, Vector3 max) => vec3.x.IsBetween(min.x, max.x) && vec3.y.IsBetween(min.y, max.y) && vec3.z.IsBetween(min.z, max.z);
        /// <summary>
        /// Checks if a vector has all of his values between(greater/less) the ones of the min and max vector. In the cartesian space, where the min and max defines a square, the vector is between if it is contained in said square.
        /// </summary>
        /// <param name="vec4">The value to be between the min and max</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>True if the vector has all of his values between(greater/less) the ones of the min and max vector</returns>
        public static bool IsBetween(this Vector4 vec4, Vector4 min, Vector4 max) => vec4.x.IsBetween(min.x, max.x) && vec4.y.IsBetween(min.y, max.y) && vec4.z.IsBetween(min.z, max.z) && vec4.w.IsBetween(min.w, max.w);
        
        /// <summary>
        /// Checks if a vector has all of his values within(greater/less than or equal) the ones of the min and max vector. In the cartesian space, where the min and max defines a square, the vector is between if it is inside in said square.
        /// </summary>
        /// <param name="vec2">The value to be within the min and max</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>True if the vector has all of his values within(greater/less than or equal) the ones of the min and max vector</returns>
        public static bool IsWithin(this Vector2 vec2, Vector2 min, Vector2 max) => vec2.x.IsWithin(min.x, max.x) && vec2.y.IsWithin(min.y, max.y);
        /// <summary>
        /// Checks if a vector has all of his values within(greater/less than or equal) the ones of the min and max vector. In the cartesian space, where the min and max defines a square, the vector is between if it is inside in said square.
        /// </summary>
        /// <param name="vec3">The value to be within the min and max</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>True if the vector has all of his values within(greater/less than or equal) the ones of the min and max vector</returns>
        public static bool IsWithin(this Vector3 vec3, Vector3 min, Vector3 max) => vec3.x.IsWithin(min.x, max.x) && vec3.y.IsWithin(min.y, max.y) && vec3.z.IsWithin(min.z, max.z);
        /// <summary>
        /// Checks if a vector has all of his values within(greater/less than or equal) the ones of the min and max vector. In the cartesian space, where the min and max defines a square, the vector is between if it is inside in said square.
        /// </summary>
        /// <param name="vec4">The value to be within the min and max</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>True if the vector has all of his values within(greater/less than or equal) the ones of the min and max vector</returns>
        public static bool IsWithin(this Vector4 vec4, Vector4 min, Vector4 max) => vec4.x.IsWithin(min.x, max.x) && vec4.y.IsWithin(min.y, max.y) && vec4.z.IsWithin(min.z, max.z) && vec4.w.IsWithin(min.w, max.w);

        #endregion
        #region Deconstruct

        public static void Deconstruct(this Vector2 vec, out float x, out float y)
        {
            x = vec.x;
            y = vec.y;
        }
        
        public static void Deconstruct(this Vector3 vec, out float x, out float y)
        {
            x = vec.x;
            y = vec.y;
        }
        
        public static void Deconstruct(this Vector3 vec, out float x, out float y, out float z)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }
        
        public static void Deconstruct(this Vector4 vec, out float x, out float y)
        {
            x = vec.x;
            y = vec.y;
        }
        
        public static void Deconstruct(this Vector4 vec, out float x, out float y, out float z)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }
        
        public static void Deconstruct(this Vector4 vec, out float x, out float y, out float z, out float w)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
            w = vec.w;
        }
        
        public static void Deconstruct(this Vector2Int vec, out int x, out int y)
        {
            x = vec.x;
            y = vec.y;
        }
        
        public static void Deconstruct(this Vector3Int vec, out int x, out int y)
        {
            x = vec.x;
            y = vec.y;
        }
        
        public static void Deconstruct(this Vector3Int vec, out int x, out int y, out int z)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }
        
        #endregion
        #region XYZ

        public static Vector2 XY(this Vector3 vec) => new Vector2(vec.x, vec.y);

        public static Vector2 XZ(this Vector3 vec) => new Vector2(vec.x, vec.z);

        public static Vector2 YZ(this Vector3 vec) => new Vector2(vec.y, vec.z);

        public static Vector2Int XY(this Vector3Int vec) => new Vector2Int(vec.x, vec.y);

        public static Vector2Int XZ(this Vector3Int vec) => new Vector2Int(vec.x, vec.z);

        public static Vector2Int YZ(this Vector3Int vec) => new Vector2Int(vec.y, vec.z);

        #endregion
    }
}
