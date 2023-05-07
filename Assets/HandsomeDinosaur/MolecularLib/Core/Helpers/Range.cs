using UnityEngine;

namespace MolecularLib.Helpers
{
    public interface IRange
    {
        void ValidateMinMaxValues();
    }

    public interface IRange<T> : IRange
    {
        T Min { get; set; }
        T Max { get; set; }
    }

    [System.Serializable]
    public class Range<T> : IRange<T> where T : System.IComparable
    {
        [SerializeField] protected T min;
        [SerializeField] protected T max;

        public virtual T Min
        {
            get => min;
            set => min = value;
        }

        public virtual T Max
        {
            get => max;
            set => max = value;
        }

        public virtual bool IsInRange(T value) => value.CompareTo(Min) > 0 && value.CompareTo(Max) < 0;

        public virtual T Clamp(T value)
        {
            if (value.CompareTo(Min) < 0) return Min;
            if (value.CompareTo(Max) > 0) return Max;
            return value;
        }

        public virtual T ClampCeil(T value)
        {
            return value.CompareTo(Max) > 0 ? Max : value;
        }

        public virtual T ClampFloor(T value)
        {
            return value.CompareTo(Min) < 0 ? Min : value;
        }

        public virtual void ValidateMinMaxValues()
        {
            if (Min.CompareTo(Max) > 0) Min = Max;
        }

        public void Deconstruct(out T tMin, out T tMax)
        {
            tMin = min;
            tMax = max;
        }

        public static implicit operator (T min, T max)(Range<T> range) => (range.min, range.max);
        public static implicit operator Range<T>((T min, T max) rangeTup) => new Range<T>(rangeTup.min, rangeTup.max);

        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public Range() : this(default, default)
        {
        }
    }

    [System.Serializable]
    public class Range : Range<float>
    {
        public float MidPoint => (Min + Max) / 2f;

        public float Lerp(float t) => Mathf.Lerp(Min, Max, t);
        public float LerpUnclamped(float t) => Mathf.LerpUnclamped(Min, Max, t);
        public float InverseLerp(float value) => Mathf.InverseLerp(Min, Max, value);
        public float InverseLerpUnclamped(float value) => Maths.InvLerp(Min, Max, value);

        /// <summary>
        /// Min inclusive, Max inclusive
        /// </summary>
        /// <returns>A Random float between the Min and Max variables in this range</returns>
        public float Random() => UnityEngine.Random.Range(Min, Max);
    }

    [System.Serializable]
    public class RangeInteger : Range<int>
    {
        public int MidPoint => Mathf.RoundToInt((Min + Max) / 2f);

        public int Lerp(float t) => Mathf.RoundToInt(Mathf.Lerp(Min, Max, t));
        public int LerpUnclamped(float t) => Mathf.RoundToInt(Mathf.LerpUnclamped(Min, Max, t));
        public float InverseLerp(int value) => Mathf.InverseLerp(Min, Max, value);
        public float InverseLerpUnclamped(int value) => Maths.InvLerp(Min, Max, value);

        /// <summary>
        /// Min inclusive, Max exclusive
        /// </summary>
        /// <returns>A Random int between the Min and Max variables in this range. Doesn't include the Max.</returns>
        public int Random() => UnityEngine.Random.Range(Min, Max);
    }

    [System.Serializable]
    public class RangeVector3 : Range<(float x, float y, float z)>, ISerializationCallbackReceiver
    {
        [SerializeField] private Vector3 minVector3SaveData;
        [SerializeField] private Vector3 maxVector3SaveData;

        public new Vector3 Min
        {
            get => ToVec3(base.Min);
            set => base.Min = FromVec3(value);
        }

        public new Vector3 Max
        {
            get => ToVec3(base.Max);
            set => base.Max = FromVec3(value);
        }

        public Vector3 Lerp(float t) => Vector3.Lerp(Min, Max, t);
        public Vector3 LerpUnclamped(float t) => Vector3.LerpUnclamped(Min, Max, t);
        public float InverseLerp(Vector3 value) => Mathf.Clamp01(Maths.InvLerp(Min, Max, value));
        public float InverseLerpUnclamped(Vector3 value) => Maths.InvLerp(Min, Max, value);

        public Vector3 Random() => new Vector3(UnityEngine.Random.Range(Min.x, Max.x),
            UnityEngine.Random.Range(Min.y, Max.y), UnityEngine.Random.Range(Min.z, Max.z));

        public bool IsInRange(Vector3 vec3) => Maths.IsAllGreaterThan(Min, vec3) && Maths.IsAllSmallerThan(Max, vec3);

        public Bounds GetBoundingBox()
        {
            var center = new Vector3((Max.x - Min.x) / 2 + Min.x, (Max.y - Min.y) / 2 + Min.y,
                (Max.z - Min.z) / 2 + Min.z);
            var size = new Vector3(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);

            return new Bounds(center, size);
        }

        public Vector3 Clamp(Vector3 vec3)
        {
            if (vec3.x < Min.x) vec3.x = Min.x;
            if (vec3.y < Min.y) vec3.y = Min.y;
            if (vec3.z < Min.z) vec3.z = Min.z;

            if (vec3.x > Max.x) vec3.x = Max.x;
            if (vec3.y > Max.y) vec3.y = Max.y;
            if (vec3.z > Max.z) vec3.z = Max.z;

            return vec3;
        }

        public Vector3 ClampCeil(Vector3 vec3)
        {
            if (vec3.x > Max.x) vec3.x = Max.x;
            if (vec3.y > Max.y) vec3.y = Max.y;
            if (vec3.z > Max.z) vec3.z = Max.z;

            return vec3;
        }

        public Vector3 ClampFloor(Vector3 vec3)
        {
            if (vec3.x < Min.x) vec3.x = Min.x;
            if (vec3.y < Min.y) vec3.y = Min.y;
            if (vec3.z < Min.z) vec3.z = Min.z;

            return vec3;
        }

        public static Vector3 ToVec3((float x, float y, float z) vec3Tup) =>
            new Vector3(vec3Tup.x, vec3Tup.y, vec3Tup.z);

        public static (float x, float y, float z) FromVec3(Vector3 vec3) => (vec3.x, vec3.y, vec3.z);

        public void OnBeforeSerialize()
        {
            minVector3SaveData = Min;
            maxVector3SaveData = Max;
        }

        public void OnAfterDeserialize()
        {
            Min = minVector3SaveData;
            Max = maxVector3SaveData;
        }
    }

    [System.Serializable]
    public class RangeVector2 : Range<(float x, float y)>, ISerializationCallbackReceiver
    {
        [SerializeField] private Vector2 minVector2SaveData;
        [SerializeField] private Vector2 maxVector2SaveData;

        public new Vector2 Min
        {
            get => ToVec2(base.Min);
            set => base.Min = FromVec2(value);
        }

        public new Vector2 Max
        {
            get => ToVec2(base.Max);
            set => base.Max = FromVec2(value);
        }

        public Vector2 Lerp(float t) => Vector2.Lerp(Min, Max, t);
        public Vector2 LerpUnclamped(float t) => Vector2.LerpUnclamped(Min, Max, t);
        public float InverseLerp(Vector2 value) => Mathf.Clamp01(Maths.InvLerp(Min, Max, value));
        public float InverseLerpUnclamped(Vector2 value) => Maths.InvLerp(Min, Max, value);

        public Vector2 Random() =>
            new Vector2(UnityEngine.Random.Range(Min.x, Max.x), UnityEngine.Random.Range(Min.y, Max.y));

        public bool IsInRange(Vector2 vec2) => Maths.IsAllGreaterThan(Min, vec2) && Maths.IsAllSmallerThan(Max, vec2);

        public Bounds GetBoundingBox()
        {
            var center = new Vector3((Max.x - Min.x) / 2 + Min.x, (Max.y - Min.y) / 2 + Min.y);
            var size = new Vector3(Max.x - Min.x, Max.y - Min.y);

            return new Bounds(center, size);
        }

        public Vector2 Clamp(Vector2 vec2)
        {
            if (vec2.x < Min.x) vec2.x = Min.x;
            if (vec2.y < Min.y) vec2.y = Min.y;

            if (vec2.x > Max.x) vec2.x = Max.x;
            if (vec2.y > Max.y) vec2.y = Max.y;

            return vec2;
        }

        public Vector2 ClampCeil(Vector2 vec2)
        {
            if (vec2.x > Max.x) vec2.x = Max.x;
            if (vec2.y > Max.y) vec2.y = Max.y;

            return vec2;
        }

        public Vector2 ClampFloor(Vector2 vec2)
        {
            if (vec2.x < Min.x) vec2.x = Min.x;
            if (vec2.y < Min.y) vec2.y = Min.y;

            return vec2;
        }

        public static Vector2 ToVec2((float x, float y) vec2Tup) => new Vector2(vec2Tup.x, vec2Tup.y);
        public static (float x, float y) FromVec2(Vector2 vec2) => (vec2.x, vec2.y);

        public void OnBeforeSerialize()
        {
            minVector2SaveData = Min;
            maxVector2SaveData = Max;
        }

        public void OnAfterDeserialize()
        {
            Min = minVector2SaveData;
            Max = maxVector2SaveData;
        }
    }
    
    [System.Serializable]
    public class RangeVector3Int : Range<(int x, int y, int z)>, ISerializationCallbackReceiver
    {
        [SerializeField] private Vector3Int minVector2SaveData;
        [SerializeField] private Vector3Int maxVector2SaveData;

        public new Vector3Int Min
        {
            get => ToVec3(base.Min);
            set => base.Min = FromVec3(value);
        }

        public new Vector3Int Max
        {
            get => ToVec3(base.Max);
            set => base.Max = FromVec3(value);
        }

        public float InverseLerp(Vector3Int value) => Mathf.Clamp01(Maths.InvLerp(Min, Max, value));
        public float InverseLerpUnclamped(Vector3Int value) => Maths.InvLerp(Min, Max, value);

        public Vector3Int Random() =>
            new Vector3Int(UnityEngine.Random.Range(Min.x, Max.x), UnityEngine.Random.Range(Min.y, Max.y), UnityEngine.Random.Range(Min.z, Max.z));

        public bool IsInRange(Vector3Int vec3) => Maths.IsAllGreaterThan(Min, vec3) && Maths.IsAllSmallerThan(Max, vec3);

        public BoundsInt GetBoundingIntBox()
        {
            var center = new Vector3Int((Max.x - Min.x) / 2 + Min.x, (Max.y - Min.y) / 2 + Min.y, (Max.z - Min.z) / 2 + Min.z);
            var size = new Vector3Int(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);

            return new BoundsInt(center, size);
        }
        
        public Bounds GetBoundingBox()
        {
            var center = new Vector3((Max.x - Min.x) / 2 + Min.x, (Max.y - Min.y) / 2 + Min.y, (Max.z - Min.z) / 2 + Min.z);
            var size = new Vector3(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);

            return new Bounds(center, size);
        }

        public Vector3Int Clamp(Vector3Int vec3)
        {
            if (vec3.x < Min.x) vec3.x = Min.x;
            if (vec3.y < Min.y) vec3.y = Min.y;
            if (vec3.z < Min.z) vec3.z = Min.z;

            if (vec3.x > Max.x) vec3.x = Max.x;
            if (vec3.y > Max.y) vec3.y = Max.y;
            if (vec3.z > Max.z) vec3.z = Max.z;

            return vec3;
        }

        public Vector3Int ClampCeil(Vector3Int vec3)
        {
            if (vec3.x > Max.x) vec3.x = Max.x;
            if (vec3.y > Max.y) vec3.y = Max.y;
            if (vec3.z > Max.z) vec3.z = Max.z;

            return vec3;
        }

        public Vector3Int ClampFloor(Vector3Int vec3)
        {
            if (vec3.x < Min.x) vec3.x = Min.x;
            if (vec3.y < Min.y) vec3.y = Min.y;
            if (vec3.z < Min.z) vec3.z = Min.z;

            return vec3;
        }

        public static Vector3Int ToVec3((int x, int y, int z) vec3Tup) => new Vector3Int(vec3Tup.x, vec3Tup.y, vec3Tup.z);
        public static (int x, int y, int z) FromVec3(Vector3Int vec3) => (vec3.x, vec3.y, vec3.z);

        public void OnBeforeSerialize()
        {
            minVector2SaveData = Min;
            maxVector2SaveData = Max;
        }

        public void OnAfterDeserialize()
        {
            Min = minVector2SaveData;
            Max = maxVector2SaveData;
        }
    }
    
    [System.Serializable]
    public class RangeVector2Int : Range<(int x, int y)>, ISerializationCallbackReceiver
    {
        [SerializeField] private Vector2Int minVector2SaveData;
        [SerializeField] private Vector2Int maxVector2SaveData;

        public new Vector2Int Min
        {
            get => ToVec2(base.Min);
            set => base.Min = FromVec2(value);
        }

        public new Vector2Int Max
        {
            get => ToVec2(base.Max);
            set => base.Max = FromVec2(value);
        }

        public float InverseLerp(Vector2Int value) => Mathf.Clamp01(Maths.InvLerp(Min, Max, value));
        public float InverseLerpUnclamped(Vector2Int value) => Maths.InvLerp(Min, Max, value);

        public Vector2Int Random() =>
            new Vector2Int(UnityEngine.Random.Range(Min.x, Max.x), UnityEngine.Random.Range(Min.y, Max.y));

        public bool IsInRange(Vector2Int vec2) => Maths.IsAllGreaterThan(Min, vec2) && Maths.IsAllSmallerThan(Max, vec2);

        public BoundsInt GetBoundingIntBox()
        {
            var center = new Vector3Int((Max.x - Min.x) / 2 + Min.x, (Max.y - Min.y) / 2 + Min.y, 0);
            var size = new Vector3Int(Max.x - Min.x, Max.y - Min.y, 0);

            return new BoundsInt(center, size);
        }
        
        public Bounds GetBoundingBox()
        {
            var center = new Vector3((Max.x - Min.x) / 2 + Min.x, (Max.y - Min.y) / 2 + Min.y);
            var size = new Vector3(Max.x - Min.x, Max.y - Min.y);

            return new Bounds(center, size);
        }

        public Vector2Int Clamp(Vector2Int vec2)
        {
            if (vec2.x < Min.x) vec2.x = Min.x;
            if (vec2.y < Min.y) vec2.y = Min.y;

            if (vec2.x > Max.x) vec2.x = Max.x;
            if (vec2.y > Max.y) vec2.y = Max.y;

            return vec2;
        }

        public Vector2Int ClampCeil(Vector2Int vec2)
        {
            if (vec2.x > Max.x) vec2.x = Max.x;
            if (vec2.y > Max.y) vec2.y = Max.y;

            return vec2;
        }

        public Vector2Int ClampFloor(Vector2Int vec2)
        {
            if (vec2.x < Min.x) vec2.x = Min.x;
            if (vec2.y < Min.y) vec2.y = Min.y;

            return vec2;
        }

        public static Vector2Int ToVec2((int x, int y) vec2Tup) => new Vector2Int(vec2Tup.x, vec2Tup.y);
        public static (int x, int y) FromVec2(Vector2Int vec2) => (vec2.x, vec2.y);

        public void OnBeforeSerialize()
        {
            minVector2SaveData = Min;
            maxVector2SaveData = Max;
        }

        public void OnAfterDeserialize()
        {
            Min = minVector2SaveData;
            Max = maxVector2SaveData;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Max { get; }

        public float Min { get; }
    }

    public static class RangeExtensionMethods
    {
        public static float Random(this Range<float> range) =>
            (range as Range)?.Random() ?? throw new System.NullReferenceException();

        public static float Random(this Range<int> range) =>
            (range as RangeInteger)?.Random() ?? throw new System.NullReferenceException();
    }
}
