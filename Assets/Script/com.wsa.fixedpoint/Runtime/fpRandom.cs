using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Deterministics.Math.fpMath;

namespace Deterministics.Math {
    [StructLayout(LayoutKind.Explicit, Size = SIZE)]
    public struct fpRandom {
        public const int SIZE = 4;

        [FieldOffset(0)]
        public uint state;
        
        /// <summary>
        /// Seed must be non-zero
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fpRandom(uint seed)
        {
            state = seed;
            if (state == 0)
            {
                throw new System.ArgumentException("Seed must be non-zero");
            }

            NextState();
        }

        // 如果是一堆index，需要使用这个方法，不然生成的随机数很多重复。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpRandom CreateFromIndex(uint index)
        {
             if (index == uint.MaxValue)
                throw new System.ArgumentException("Index must not be uint.MaxValue");

            // Wang hash will hash 61 to zero but we want uint.MaxValue to hash to zero.  To make this happen
            // we must offset by 62.
            return new fpRandom(WangHash(index + 62u));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint WangHash(uint n)
        {
            // https://gist.github.com/badboy/6267743#hash-function-construction-principles
            // Wang hash: this has the property that none of the outputs will
            // collide with each other, which is important for the purposes of
            // seeding a random number generator.  This was verified empirically
            // by checking all 2^32 uints.
            n = (n ^ 61u) ^ (n >> 16);
            n *= 9u;
            n = n ^ (n >> 4);
            n *= 0x27d4eb2du;
            n = n ^ (n >> 15);

            return n;
        }

        /// <summary>
        /// Seed must be non-zero
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitState(uint seed = 1851936439u)
        {
            state = seed;
            NextState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint NextState() {
            var t  = state;
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return t;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBool()
        {
            return (NextState() & 1) == 1;
        }

        /// <summary>Returns value in range [-2147483647, 2147483647]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInt()
        {
            return (int)NextState() ^ -2147483648;
        }
        
        /// <summary>Returns value in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInt(int max)
        {
            return (int)((NextState() * (ulong)max) >> 32);
        }
        
        /// <summary>Returns value in range [min, max].</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInt(int min, int max)
        {
            var range = (uint)(max - min);
            return (int)(NextState() * (ulong)range >> 32) + min;
        }
        
        /// <summary>Returns value in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp NextFp() {
            return new fp(){RawValue = NextInt(0, 65535)};
        }
        
        /// <summary>Returns vector with all components in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp2 NextFp2() {
            return new fp2(NextFp(), NextFp());
        }
        
        /// <summary>Returns vector with all components in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp3 NextFp3() {
            return new fp3(NextFp(), NextFp(), NextFp());
        }
        
        /// <summary>Returns vector with all components in range [0, 1]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp4 NextFp4() {
            return new fp4(NextFp(), NextFp(), NextFp(), NextFp());
        }


        /// <summary>Returns value in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp NextFp(fp max) {
            return NextFp() * max;
        }
        
        /// <summary>Returns vector with all components in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp2 NextFp2(fp2 max) {
            return NextFp2() * max;
        }
        
        /// <summary>Returns vector with all components in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp3 NextFp3(fp3 max) {
            return NextFp3() * max;
        }
        
        /// <summary>Returns vector with all components in range [0, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp4 NextFp4(fp4 max) {
            return NextFp4() * max;
        }

        /// <summary>Returns value in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp NextFp(fp min, fp max) {
            return NextFp() * (max - min) + min;
        }

        /// <summary>Returns vector with all components in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp2 NextFp2(fp2 min, fp2 max) {
            return NextFp2() * (max - min) + min;
        }
        
        /// <summary>Returns vector with all components in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp3 NextFp3(fp3 min, fp3 max) {
            return NextFp3() * (max - min) + min;
        }
        
        /// <summary>Returns vector with all components in range [min, max]</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public fp4 NextFp4(fp4 min, fp4 max) {
            return NextFp4() * (max - min) + min;
        }
        
        /// <summary>Returns a normalized 2D direction</summary>
        public fp2 NextDirection2D() {
            var angle = NextFp() * fp.PI * fp.two;
            sincos(angle, out var sin, out var cos);
            return new fp2(sin,cos);
        }
        
        /// <summary>Returns a normalized 3D direction</summary>
        public fp3 NextDirection3D() {
            var z = NextFp(fp.two) - fp.one;
            var r = sqrt(max(fp.one - z * z, fp.zero));
            var angle = NextFp(fp.PITimes2);
            sincos(angle, out var sin, out var cos);
            return new fp3(cos * r, sin * r, z);
        }
    }
}