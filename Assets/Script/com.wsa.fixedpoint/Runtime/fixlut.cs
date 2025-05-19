namespace Deterministics.Math {
    public static partial class fixlut {
        public const int  FRACTIONS_COUNT = 5;
        public const int  PRECISION       = 16;
        public const int  FRACTIONAL_PLACES_SQRT = PRECISION / 2;
        public const int  SHIFT           = 16 - 9;
        public const long PI              = 205887L;
        public const long ONE             = 1 << PRECISION;
        public const long HALF            = 1 << (PRECISION-1);
        public const long ZERO            = 0;
        

        public static long sin(long value) {
            var sign = 1;
            if (value < 0) {
                value = -value;
                sign = -1;
            }

            var index    = (int) (value >> SHIFT);
            var fraction = (value - (index << SHIFT)) << 9;
            var a        = SinLut[index];
            var b        = SinLut[index + 1];
            var v2       = a + (((b - a) * fraction) >> PRECISION);
            return v2 * sign;
        }

        public static long cos(long value) {
            if (value < 0) {
                value = -value;
            }

            value += fp._0_25.RawValue;
            
            if (value >= 65536) {
                value -= 65536;
            }

            var index    = (int) (value >> SHIFT);
            var fraction = (value - (index << SHIFT)) << 9;
            var a        = SinLut[index];
            var b        = SinLut[index + 1];
            var v2       = a + (((b - a) * fraction) >> PRECISION);
            return v2;
        }


        public static long tan(long value) {
            var sign = 1;

            if (value < 0) {
                value = -value;
                sign  = -1;
            }

            var index    = (int) (value >> SHIFT);
            var fraction = (value - (index << SHIFT)) << 9;
            var a        = TanLut[index];
            var b        = TanLut[index + 1];
            var v2       = a + (((b - a) * fraction) >> PRECISION);
            return v2 * sign;
        }

        public static void sin_cos(long value, out long sin, out long cos) {
            var sign = 1;
            if (value < 0) {
                value = -value;
                sign = -1;
            }

            var index       = (int) (value >> SHIFT);
            var doubleIndex = index * 2;
            var fractions   = (value - (index << SHIFT)) << 9;

            var sinA = SinCosLut[doubleIndex];
            var cosA = SinCosLut[doubleIndex + 1];
            var sinB = SinCosLut[doubleIndex + 2];
            var cosB = SinCosLut[doubleIndex + 3];

            sin = (sinA + (((sinB - sinA) * fractions) >> PRECISION)) * sign;
            cos = cosA + (((cosB - cosA) * fractions) >> PRECISION);
        }

        public static long asin(long value) {
            bool flag = false;
            if (value < 0) {
                flag  = true;
                value = -value;
            }

            var index = (int)System.Math.Min(value, AsinLut.Length - 1);

            var result = AsinLut[index];
            if (flag)
                result = -result;
            return result;
        }
        
        public static long acos(long value) {
            bool flag = false;
            if (value < 0) {
                flag  = true;
                value = -value;
            }

            var index = (int)System.Math.Min(value, AsinLut.Length - 1);

            long result = -AsinLut[index];
            if (flag)
                result = -result;

            result += fp.PiDiv2Long;
            return result;
        }

        
        const long WaterShedSqrt = 4 << 16;
        internal static long sqrt(long x)
        {
            if (x < 0L) 
            {
                UnityEngine.Debug.LogError($"input < 0 x:${x}");
                return 0L;
            }
            if (x == 0L) return 0L;
            bool lessThanOne = x < ONE;
            long sValue = x;
            int count = 0;
            if (lessThanOne) sValue <<= PRECISION;
            while (sValue >= WaterShedSqrt)
            {
                count++;
                sValue >>= 2;
            }

            int index = (int)(sValue - ONE) >> 2;
            sValue = (SqrtLut[index] + ONE) << count;
            if (lessThanOne) sValue >>= FRACTIONAL_PLACES_SQRT;
            return sValue;
        }
    }
}