

using UnityEngine;

namespace W {

    /// <summary>
    /// M for Math
    /// </summary>
    public static class M
    {
        public static float Progress(long now, long start, long del) => (float)((double)(now - start) / del);

        public static long Abs(long x) => x < 0 ? -x : x;
        public static long Clamp(long a, long b, long x) => x < a ? a : x > b ? b : x;
        public static long Clamp01(long x) => Clamp(0, 1, x);


        public static float Abs(float x) => x < 0 ? -x : x;
        public static float Clamp(float a, float b, float x) => x < a ? a : x > b ? b : x;
        public static float Clamp01(float x) => Clamp(0, 1, x);
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static float ReverseLerp(float a, float b, float v) => (b - a) == 0 ? 0 : (v - a) / (b - a);


        public static double Abs(double x) => x < 0 ? -x : x;
        public static double Clamp(double a, double b, double x) => x < a ? a : x > b ? b : x;
        public static double Clamp01(double x) => Clamp(0, 1, x);
        public static double Lerp(double a, double b, double t) => a + (b - a) * t;
        public static double ReverseLerp(double a, double b, double v) => (b - a) == 0 ? 0 : (v - a) / (b - a);
    }
}
