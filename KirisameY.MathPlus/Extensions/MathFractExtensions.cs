using JetBrains.Annotations;

namespace KirisameY.MathPlus.Extensions;

public static class MathExtensions
{
    extension(System.Math)
    {
        [PublicAPI]
        public static decimal Fract(decimal value) => value - Math.Truncate(value);
        [PublicAPI]
        public static double Fract(double value) => value - Math.Truncate(value);
    }

    extension(System.MathF)
    {
        [PublicAPI]
        public static float Fract(float value) => value - MathF.Truncate(value);
    }
}