using System;

namespace GameExercise
{
    public static class MathUtils
    {
        public static float Radians(float degrees)
        {
            float radians = ((float)Math.PI / 180) * degrees;
            return (radians);
        }
    }
}
