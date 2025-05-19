using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Deterministics.Math
{
    /// <summary>
    /// A static class to contain various math functions and constants.
    /// </summary>
    public static partial class fpMath
    {
        public readonly static fp PI = fp.PI;

        /// <summary>Returns true if the input number is a finite floating point value, false otherwise.</summary>
        /// <param name="x">The number value to test.</param>
        /// <returns>True if the number is finite, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isfinite(fp x) { return abs(x) < fp.PositiveInfinity; }

        /// <summary>Returns a bool2 indicating for each component of a float2 whether it is a finite floating point value.</summary>
        /// <param name="x">The float2 value to test.</param>
        /// <returns>A bool2 where it is true in a component if that component is finite, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isfinite(fp2 x) { return abs(x) < fp.PositiveInfinity; }

        /// <summary>Returns a bool3 indicating for each component of a float3 whether it is a finite floating point value.</summary>
        /// <param name="x">The float3 value to test.</param>
        /// <returns>A bool3 where it is true in a component if that component is finite, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isfinite(fp3 x) { return abs(x) < fp.PositiveInfinity; }

        /// <summary>Returns a bool4 indicating for each component of a float4 whether it is a finite floating point value.</summary>
        /// <param name="x">The float4 value to test.</param>
        /// <returns>A bool4 where it is true in a component if that component is finite, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isfinite(fp4 x) { return abs(x) < fp.PositiveInfinity; }

        /// <summary>Returns the minimum of two number values.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The minimum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp min(fp x, fp y) { return x <= y ? x : y; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int min(int x, int y) { return x <= y ? x : y; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long min(long x, long y) { return x <= y ? x : y; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint min(uint x, uint y) { return x <= y ? x : y; }

        /// <summary>Returns the componentwise minimum of two float2 vectors.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The componentwise minimum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 min(fp2 x, fp2 y) { return new fp2(min(x.x, y.x), min(x.y, y.y)); }

        /// <summary>Returns the componentwise minimum of two float3 vectors.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The componentwise minimum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 min(fp3 x, fp3 y) { return new fp3(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z)); }

        /// <summary>Returns the componentwise minimum of two float4 vectors.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The componentwise minimum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 min(fp4 x, fp4 y) { return new fp4(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z), min(x.w, y.w)); }

        /// <summary>Returns the maximum of two number values.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The maximum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp max(fp x, fp y) { return x >= y ? x : y; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int max(int x, int y) { return x >= y ? x : y; }

        /// <summary>Returns the componentwise maximum of two float2 vectors.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The componentwise maximum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 max(fp2 x, fp2 y) { return new fp2(max(x.x, y.x), max(x.y, y.y)); }

        /// <summary>Returns the componentwise maximum of two float3 vectors.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The componentwise maximum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 max(fp3 x, fp3 y) { return new fp3(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z)); }

        /// <summary>Returns the componentwise maximum of two float4 vectors.</summary>
        /// <param name="x">The first input value.</param>
        /// <param name="y">The second input value.</param>
        /// <returns>The componentwise maximum of the two input values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 max(fp4 x, fp4 y) { return new fp4(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z), max(x.w, y.w)); }


        /// <summary>Returns the result of linearly interpolating from x to y using the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp lerp(fp x, fp y, fp s) { return x + s * (y - x); }

        /// <summary>Returns the result of a componentwise linear interpolating from x to y using the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The componentwise interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 lerp(fp2 x, fp2 y, fp s) { return x + s * (y - x); }

        /// <summary>Returns the result of a componentwise linear interpolating from x to y using the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The componentwise interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 lerp(fp3 x, fp3 y, fp s) { return x + s * (y - x); }

        /// <summary>Returns the result of a componentwise linear interpolating from x to y using the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The componentwise interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 lerp(fp4 x, fp4 y, fp s) { return x + s * (y - x); }


        /// <summary>Returns the result of a componentwise linear interpolating from x to y using the corresponding components of the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The componentwise interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 lerp(fp2 x, fp2 y, fp2 s) { return x + s * (y - x); }

        /// <summary>Returns the result of a componentwise linear interpolating from x to y using the corresponding components of the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The componentwise interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 lerp(fp3 x, fp3 y, fp3 s) { return x + s * (y - x); }

        /// <summary>Returns the result of a componentwise linear interpolating from x to y using the corresponding components of the interpolation parameter s.</summary>
        /// <remarks>
        /// If the interpolation parameter is not in the range [0, 1], then this function extrapolates.
        /// </remarks>
        /// <param name="x">The first endpoint, corresponding to the interpolation parameter value of 0.</param>
        /// <param name="y">The second endpoint, corresponding to the interpolation parameter value of 1.</param>
        /// <param name="s">The interpolation parameter. May be a value outside the interval [0, 1].</param>
        /// <returns>The componentwise interpolation from x to y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 lerp(fp4 x, fp4 y, fp4 s) { return x + s * (y - x); }


        /// <summary>Returns the result of normalizing a fix point value x to a range [a, b]. The opposite of lerp. Equivalent to (x - a) / (b - a).</summary>
        /// <param name="a">The first endpoint of the range.</param>
        /// <param name="b">The second endpoint of the range.</param>
        /// <param name="x">The value to normalize to the range.</param>
        /// <returns>The interpolation parameter of x with respect to the input range [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp unlerp(fp a, fp b, fp x) { return (x - a) / (b - a); }

        /// <summary>Returns the componentwise result of normalizing a fix point value x to a range [a, b]. The opposite of lerp. Equivalent to (x - a) / (b - a).</summary>
        /// <param name="a">The first endpoint of the range.</param>
        /// <param name="b">The second endpoint of the range.</param>
        /// <param name="x">The value to normalize to the range.</param>
        /// <returns>The componentwise interpolation parameter of x with respect to the input range [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 unlerp(fp2 a, fp2 b, fp2 x) { return (x - a) / (b - a); }

        /// <summary>Returns the componentwise result of normalizing a fix point value x to a range [a, b]. The opposite of lerp. Equivalent to (x - a) / (b - a).</summary>
        /// <param name="a">The first endpoint of the range.</param>
        /// <param name="b">The second endpoint of the range.</param>
        /// <param name="x">The value to normalize to the range.</param>
        /// <returns>The componentwise interpolation parameter of x with respect to the input range [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 unlerp(fp3 a, fp3 b, fp3 x) { return (x - a) / (b - a); }

        /// <summary>Returns the componentwise result of normalizing a fix point value x to a range [a, b]. The opposite of lerp. Equivalent to (x - a) / (b - a).</summary>
        /// <param name="a">The first endpoint of the range.</param>
        /// <param name="b">The second endpoint of the range.</param>
        /// <param name="x">The value to normalize to the range.</param>
        /// <returns>The componentwise interpolation parameter of x with respect to the input range [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 unlerp(fp4 a, fp4 b, fp4 x) { return (x - a) / (b - a); }


        /// <summary>Returns the result of a non-clamping linear remapping of a value x from source range [a, b] to the destination range [c, d].</summary>
        /// <param name="a">The first endpoint of the source range [a,b].</param>
        /// <param name="b">The second endpoint of the source range [a, b].</param>
        /// <param name="c">The first endpoint of the destination range [c, d].</param>
        /// <param name="d">The second endpoint of the destination range [c, d].</param>
        /// <param name="x">The value to remap from the source to destination range.</param>
        /// <returns>The remap of input x from the source range to the destination range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp remap(fp a, fp b, fp c, fp d, fp x) { return lerp(c, d, unlerp(a, b, x)); }

        /// <summary>Returns the componentwise result of a non-clamping linear remapping of a value x from source range [a, b] to the destination range [c, d].</summary>
        /// <param name="a">The first endpoint of the source range [a,b].</param>
        /// <param name="b">The second endpoint of the source range [a, b].</param>
        /// <param name="c">The first endpoint of the destination range [c, d].</param>
        /// <param name="d">The second endpoint of the destination range [c, d].</param>
        /// <param name="x">The value to remap from the source to destination range.</param>
        /// <returns>The componentwise remap of input x from the source range to the destination range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 remap(fp2 a, fp2 b, fp2 c, fp2 d, fp2 x) { return lerp(c, d, unlerp(a, b, x)); }

        /// <summary>Returns the componentwise result of a non-clamping linear remapping of a value x from source range [a, b] to the destination range [c, d].</summary>
        /// <param name="a">The first endpoint of the source range [a,b].</param>
        /// <param name="b">The second endpoint of the source range [a, b].</param>
        /// <param name="c">The first endpoint of the destination range [c, d].</param>
        /// <param name="d">The second endpoint of the destination range [c, d].</param>
        /// <param name="x">The value to remap from the source to destination range.</param>
        /// <returns>The componentwise remap of input x from the source range to the destination range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 remap(fp3 a, fp3 b, fp3 c, fp3 d, fp3 x) { return lerp(c, d, unlerp(a, b, x)); }

        /// <summary>Returns the componentwise result of a non-clamping linear remapping of a value x from source range [a, b] to the destination range [c, d].</summary>
        /// <param name="a">The first endpoint of the source range [a,b].</param>
        /// <param name="b">The second endpoint of the source range [a, b].</param>
        /// <param name="c">The first endpoint of the destination range [c, d].</param>
        /// <param name="d">The second endpoint of the destination range [c, d].</param>
        /// <param name="x">The value to remap from the source to destination range.</param>
        /// <returns>The componentwise remap of input x from the source range to the destination range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 remap(fp4 a, fp4 b, fp4 c, fp4 d, fp4 x) { return lerp(c, d, unlerp(a, b, x)); }


        /// <summary>Returns the result of a multiply-add operation (a * b + c) on 3 number values.</summary>
        /// <remarks>
        /// When Burst compiled with fast math enabled on some architectures, this could be converted to a fused multiply add (FMA).
        /// FMA is more accurate due to rounding once at the end of the computation rather than twice that is required when
        /// this computation is not fused.
        /// </remarks>
        /// <param name="a">First value to multiply.</param>
        /// <param name="b">Second value to multiply.</param>
        /// <param name="c">Third value to add to the product of a and b.</param>
        /// <returns>The multiply-add of the inputs.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp mad(fp a, fp b, fp c) { return a * b + c; }

        /// <summary>Returns the result of a componentwise multiply-add operation (a * b + c) on 3 float2 vectors.</summary>
        /// <remarks>
        /// When Burst compiled with fast math enabled on some architectures, this could be converted to a fused multiply add (FMA).
        /// FMA is more accurate due to rounding once at the end of the computation rather than twice that is required when
        /// this computation is not fused.
        /// </remarks>
        /// <param name="a">First value to multiply.</param>
        /// <param name="b">Second value to multiply.</param>
        /// <param name="c">Third value to add to the product of a and b.</param>
        /// <returns>The componentwise multiply-add of the inputs.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 mad(fp2 a, fp2 b, fp2 c) { return a * b + c; }

        /// <summary>Returns the result of a componentwise multiply-add operation (a * b + c) on 3 float3 vectors.</summary>
        /// <remarks>
        /// When Burst compiled with fast math enabled on some architectures, this could be converted to a fused multiply add (FMA).
        /// FMA is more accurate due to rounding once at the end of the computation rather than twice that is required when
        /// this computation is not fused.
        /// </remarks>
        /// <param name="a">First value to multiply.</param>
        /// <param name="b">Second value to multiply.</param>
        /// <param name="c">Third value to add to the product of a and b.</param>
        /// <returns>The componentwise multiply-add of the inputs.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 mad(fp3 a, fp3 b, fp3 c) { return a * b + c; }

        /// <summary>Returns the result of a componentwise multiply-add operation (a * b + c) on 3 float4 vectors.</summary>
        /// <remarks>
        /// When Burst compiled with fast math enabled on some architectures, this could be converted to a fused multiply add (FMA).
        /// FMA is more accurate due to rounding once at the end of the computation rather than twice that is required when
        /// this computation is not fused.
        /// </remarks>
        /// <param name="a">First value to multiply.</param>
        /// <param name="b">Second value to multiply.</param>
        /// <param name="c">Third value to add to the product of a and b.</param>
        /// <returns>The componentwise multiply-add of the inputs.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 mad(fp4 a, fp4 b, fp4 c) { return a * b + c; }


        /// <summary>Returns the result of clamping the value x into the interval [a, b], where x, a and b are number values.</summary>
        /// <param name="x">Input value to be clamped.</param>
        /// <param name="a">Lower bound of the interval.</param>
        /// <param name="b">Upper bound of the interval.</param>
        /// <returns>The clamping of the input x into the interval [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp clamp(fp x, fp a, fp b) { return max(a, min(b, x)); }

        /// <summary>Returns the result of a componentwise clamping of the value x into the interval [a, b], where x, a and b are float2 vectors.</summary>
        /// <param name="x">Input value to be clamped.</param>
        /// <param name="a">Lower bound of the interval.</param>
        /// <param name="b">Upper bound of the interval.</param>
        /// <returns>The componentwise clamping of the input x into the interval [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 clamp(fp2 x, fp2 a, fp2 b) { return max(a, min(b, x)); }

        /// <summary>Returns the result of a componentwise clamping of the value x into the interval [a, b], where x, a and b are float3 vectors.</summary>
        /// <param name="x">Input value to be clamped.</param>
        /// <param name="a">Lower bound of the interval.</param>
        /// <param name="b">Upper bound of the interval.</param>
        /// <returns>The componentwise clamping of the input x into the interval [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 clamp(fp3 x, fp3 a, fp3 b) { return max(a, min(b, x)); }

        /// <summary>Returns the result of a componentwise clamping of the value x into the interval [a, b], where x, a and b are float4 vectors.</summary>
        /// <param name="x">Input value to be clamped.</param>
        /// <param name="a">Lower bound of the interval.</param>
        /// <param name="b">Upper bound of the interval.</param>
        /// <returns>The componentwise clamping of the input x into the interval [a, b].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 clamp(fp4 x, fp4 a, fp4 b) { return max(a, min(b, x)); }


        /// <summary>Returns the result of clamping the number value x into the interval [0, 1].</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The clamping of the input into the interval [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp saturate(fp x) { return clamp(x, fp.zero, fp.one); }

        /// <summary>Returns the result of a componentwise clamping of the float2 vector x into the interval [0, 1].</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise clamping of the input into the interval [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 saturate(fp2 x) { return clamp(x, new fp2(0), new fp2(1)); }

        /// <summary>Returns the result of a componentwise clamping of the float3 vector x into the interval [0, 1].</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise clamping of the input into the interval [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 saturate(fp3 x) { return clamp(x, new fp3(0), new fp3(1)); }

        /// <summary>Returns the result of a componentwise clamping of the float4 vector x into the interval [0, 1].</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise clamping of the input into the interval [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 saturate(fp4 x) { return clamp(x, new fp4(0), new fp4(1)); }


        /// <summary>Returns the absolute value of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The absolute value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int abs(int x) { return x > 0 ? x : -x;
         }
        /// <summary>Returns the absolute value of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The absolute value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp abs(fp x) { return fp.Abs(x); }

        /// <summary>Returns the componentwise absolute value of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise absolute value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 abs(fp2 x) { return fp2(abs(x.x), abs(x.y)); }

        /// <summary>Returns the componentwise absolute value of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise absolute value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 abs(fp3 x) { return fp3(abs(x.x), abs(x.y), abs(x.z)); }

        /// <summary>Returns the componentwise absolute value of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise absolute value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 abs(fp4 x) { return fp4(abs(x.x), abs(x.y), abs(x.z), abs(x.w)); }


        /// <summary>Returns the dot product of two number values. Equivalent to multiplication.</summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The dot product of two values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp dot(fp x, fp y) { return x * y; }

        /// <summary>Returns the dot product of two float2 vectors.</summary>
        /// <param name="x">The first vector.</param>
        /// <param name="y">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp dot(fp2 x, fp2 y) { return x.x * y.x + x.y * y.y; }

        /// <summary>Returns the dot product of two float3 vectors.</summary>
        /// <param name="x">The first vector.</param>
        /// <param name="y">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp dot(fp3 x, fp3 y) { return x.x * y.x + x.y * y.y + x.z * y.z; }

        /// <summary>Returns the dot product of two float4 vectors.</summary>
        /// <param name="x">The first vector.</param>
        /// <param name="y">The second vector.</param>
        /// <returns>The dot product of two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp dot(fp4 x, fp4 y) { return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w; }


        /// <summary>Returns the tangent of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp tan(fp x) { return fp.Tan(x); }

        /// <summary>Returns the componentwise tangent of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 tan(fp2 x) { return fp2(tan(x.x), tan(x.y)); }

        /// <summary>Returns the componentwise tangent of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 tan(fp3 x) { return fp3(tan(x.x), tan(x.y), tan(x.z)); }

        /// <summary>Returns the componentwise tangent of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 tan(fp4 x) { return fp4(tan(x.x), tan(x.y), tan(x.z), tan(x.w)); }


        /// <summary>Returns the hyperbolic tangent of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The hyperbolic tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp tanh(fp x) { return fp.Tanh(x); }

        /// <summary>Returns the componentwise hyperbolic tangent of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 tanh(fp2 x) { return fp2(tanh(x.x), tanh(x.y)); }

        /// <summary>Returns the componentwise hyperbolic tangent of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 tanh(fp3 x) { return fp3(tanh(x.x), tanh(x.y), tanh(x.z)); }

        /// <summary>Returns the componentwise hyperbolic tangent of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic tangent of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 tanh(fp4 x) { return fp4(tanh(x.x), tanh(x.y), tanh(x.z), tanh(x.w)); }


        /// <summary>Returns the arctangent of a number value.</summary>
        /// <param name="x">A tangent value, usually the ratio y/x on the unit circle.</param>
        /// <returns>The arctangent of the input, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp atan(fp x) { return fp.Atan(x); }

        /// <summary>Returns the componentwise arctangent of a float2 vector.</summary>
        /// <param name="x">A tangent value, usually the ratio y/x on the unit circle.</param>
        /// <returns>The componentwise arctangent of the input, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 atan(fp2 x) { return fp2(atan(x.x), atan(x.y)); }

        /// <summary>Returns the componentwise arctangent of a float3 vector.</summary>
        /// <param name="x">A tangent value, usually the ratio y/x on the unit circle.</param>
        /// <returns>The componentwise arctangent of the input, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 atan(fp3 x) { return fp3(atan(x.x), atan(x.y), atan(x.z)); }

        /// <summary>Returns the componentwise arctangent of a float4 vector.</summary>
        /// <param name="x">A tangent value, usually the ratio y/x on the unit circle.</param>
        /// <returns>The componentwise arctangent of the input, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 atan(fp4 x) { return fp4(atan(x.x), atan(x.y), atan(x.z), atan(x.w)); }


        /// <summary>Returns the 2-argument arctangent of a pair of number values.</summary>
        /// <param name="y">Numerator of the ratio y/x, usually the y component on the unit circle.</param>
        /// <param name="x">Denominator of the ratio y/x, usually the x component on the unit circle.</param>
        /// <returns>The arctangent of the ratio y/x, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp atan2(fp y, fp x) { return fp.Atan2(y, x); }

        /// <summary>Returns the componentwise 2-argument arctangent of a pair of float2 vectors.</summary>
        /// <param name="y">Numerator of the ratio y/x, usually the y component on the unit circle.</param>
        /// <param name="x">Denominator of the ratio y/x, usually the x component on the unit circle.</param>
        /// <returns>The componentwise arctangent of the ratio y/x, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 atan2(fp2 y, fp2 x) { return fp2(atan2(y.x, x.x), atan2(y.y, x.y)); }

        /// <summary>Returns the componentwise 2-argument arctangent of a pair of float3 vectors.</summary>
        /// <param name="y">Numerator of the ratio y/x, usually the y component on the unit circle.</param>
        /// <param name="x">Denominator of the ratio y/x, usually the x component on the unit circle.</param>
        /// <returns>The componentwise arctangent of the ratio y/x, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 atan2(fp3 y, fp3 x) { return fp3(atan2(y.x, x.x), atan2(y.y, x.y), atan2(y.z, x.z)); }

        /// <summary>Returns the componentwise 2-argument arctangent of a pair of float4 vectors.</summary>
        /// <param name="y">Numerator of the ratio y/x, usually the y component on the unit circle.</param>
        /// <param name="x">Denominator of the ratio y/x, usually the x component on the unit circle.</param>
        /// <returns>The componentwise arctangent of the ratio y/x, in radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 atan2(fp4 y, fp4 x) { return fp4(atan2(y.x, x.x), atan2(y.y, x.y), atan2(y.z, x.z), atan2(y.w, x.w)); }


        /// <summary>Returns the cosine of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The cosine cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cos(fp x) { return fp.Cos(x); }

        /// <summary>Returns the componentwise cosine of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise cosine cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 cos(fp2 x) { return fp2(cos(x.x), cos(x.y)); }

        /// <summary>Returns the componentwise cosine of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise cosine cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 cos(fp3 x) { return fp3(cos(x.x), cos(x.y), cos(x.z)); }

        /// <summary>Returns the componentwise cosine of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise cosine cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 cos(fp4 x) { return fp4(cos(x.x), cos(x.y), cos(x.z), cos(x.w)); }


        /// <summary>Returns the hyperbolic cosine of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The hyperbolic cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cosh(fp x) { return fp.Cosh(x); }

        /// <summary>Returns the componentwise hyperbolic cosine of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 cosh(fp2 x) { return fp2(cosh(x.x), cosh(x.y)); }

        /// <summary>Returns the componentwise hyperbolic cosine of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 cosh(fp3 x) { return fp3(cosh(x.x), cosh(x.y), cosh(x.z)); }

        /// <summary>Returns the componentwise hyperbolic cosine of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic cosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 cosh(fp4 x) { return fp4(cosh(x.x), cosh(x.y), cosh(x.z), cosh(x.w)); }


        /// <summary>Returns the arccosine of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The arccosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp acos(fp x) { return fp.Acos(x); }

        /// <summary>Returns the componentwise arccosine of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise arccosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 acos(fp2 x) { return fp2(acos(x.x), acos(x.y)); }

        /// <summary>Returns the componentwise arccosine of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise arccosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 acos(fp3 x) { return fp3(acos(x.x), acos(x.y), acos(x.z)); }

        /// <summary>Returns the componentwise arccosine of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise arccosine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 acos(fp4 x) { return fp4(acos(x.x), acos(x.y), acos(x.z), acos(x.w)); }


        /// <summary>Returns the sine of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp sin(fp x) { return fp.Sin(x); }

        /// <summary>Returns the componentwise sine of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 sin(fp2 x) { return fp2(sin(x.x), sin(x.y)); }

        /// <summary>Returns the componentwise sine of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 sin(fp3 x) { return fp3(sin(x.x), sin(x.y), sin(x.z)); }

        /// <summary>Returns the componentwise sine of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 sin(fp4 x) { return fp4(sin(x.x), sin(x.y), sin(x.z), sin(x.w)); }


        /// <summary>Returns the hyperbolic sine of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The hyperbolic sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp sinh(fp x) { return fp.Sinh(x); }

        /// <summary>Returns the componentwise hyperbolic sine of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 sinh(fp2 x) { return new fp2(sinh(x.x), sinh(x.y)); }

        /// <summary>Returns the componentwise hyperbolic sine of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 sinh(fp3 x) { return new fp3(sinh(x.x), sinh(x.y), sinh(x.z)); }

        /// <summary>Returns the componentwise hyperbolic sine of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise hyperbolic sine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 sinh(fp4 x) { return new fp4(sinh(x.x), sinh(x.y), sinh(x.z), sinh(x.w)); }


        /// <summary>Returns the arcsine of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The arcsine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp asin(fp x) { return fp.Asin(x); }

        /// <summary>Returns the componentwise arcsine of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise arcsine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 asin(fp2 x) { return fp2(asin(x.x), asin(x.y)); }

        /// <summary>Returns the componentwise arcsine of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise arcsine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 asin(fp3 x) { return fp3(asin(x.x), asin(x.y), asin(x.z)); }

        /// <summary>Returns the componentwise arcsine of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise arcsine of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 asin(fp4 x) { return fp4(asin(x.x), asin(x.y), asin(x.z), asin(x.w)); }


        /// <summary>Returns the result of rounding a number value up to the nearest integral value less or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The round down to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp floor(fp x) { return fp.Floor(x); }

        /// <summary>Returns the result of rounding each component of a float2 vector value down to the nearest value less or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round down to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 floor(fp2 x) { return fp2(floor(x.x), floor(x.y)); }

        /// <summary>Returns the result of rounding each component of a float3 vector value down to the nearest value less or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round down to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 floor(fp3 x) { return fp3(floor(x.x), floor(x.y), floor(x.z)); }

        /// <summary>Returns the result of rounding each component of a float4 vector value down to the nearest value less or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round down to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 floor(fp4 x) { return fp4(floor(x.x), floor(x.y), floor(x.z), floor(x.w)); }


        /// <summary>Returns the result of rounding a number value up to the nearest integral value greater or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The round up to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp ceil(fp x) { return fp.Ceiling(x); }

        /// <summary>Returns the result of rounding each component of a float2 vector value up to the nearest value greater or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round up to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 ceil(fp2 x) { return fp2(ceil(x.x), ceil(x.y)); }

        /// <summary>Returns the result of rounding each component of a float3 vector value up to the nearest value greater or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round up to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 ceil(fp3 x) { return fp3(ceil(x.x), ceil(x.y), ceil(x.z)); }

        /// <summary>Returns the result of rounding each component of a float4 vector value up to the nearest value greater or equal to the original value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round up to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 ceil(fp4 x) { return fp4(ceil(x.x), ceil(x.y), ceil(x.z), ceil(x.w)); }


        /// <summary>Returns the result of rounding a number value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp round(fp x) { return fp.Round(x); }

        /// <summary>Returns the result of rounding a number value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int roundToInt(fp x) { return fp.RoundToInt(x); }


        /// <summary>Returns the result of rounding each component of a float2 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 round(fp2 x) { return fp2(round(x.x), round(x.y)); }

        /// <summary>Returns the result of rounding each component of a float3 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 round(fp3 x) { return fp3(round(x.x), round(x.y), round(x.z)); }

        /// <summary>Returns the result of rounding each component of a float4 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 round(fp4 x) { return fp4(round(x.x), round(x.y), round(x.z), round(x.w)); }


        /// <summary>Returns the result of truncating a number value to an integral number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The truncation of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp trunc(fp x) { return fp.Truncate(x); }

        /// <summary>Returns the result of a componentwise truncation of a float2 value to an integral float2 value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise truncation of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 trunc(fp2 x) { return fp2(trunc(x.x), trunc(x.y)); }

        /// <summary>Returns the result of a componentwise truncation of a float3 value to an integral float3 value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise truncation of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 trunc(fp3 x) { return fp3(trunc(x.x), trunc(x.y), trunc(x.z)); }

        /// <summary>Returns the result of a componentwise truncation of a float4 value to an integral float4 value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise truncation of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 trunc(fp4 x) { return fp4(trunc(x.x), trunc(x.y), trunc(x.z), trunc(x.w)); }


        /// <summary>Returns the fractional part of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The fractional part of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp frac(fp x) { return x - floor(x); }

        /// <summary>Returns the componentwise fractional parts of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise fractional part of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 frac(fp2 x) { return x - floor(x); }

        /// <summary>Returns the componentwise fractional parts of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise fractional part of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 frac(fp3 x) { return x - floor(x); }

        /// <summary>Returns the componentwise fractional parts of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise fractional part of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 frac(fp4 x) { return x - floor(x); }


        /// <summary>Returns the reciprocal a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The reciprocal of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp rcp(fp x) { return fp.one / x; }

        /// <summary>Returns the componentwise reciprocal a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise reciprocal of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 rcp(fp2 x) { return fp.one / x; }

        /// <summary>Returns the componentwise reciprocal a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise reciprocal of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 rcp(fp3 x) { return fp.one / x; }

        /// <summary>Returns the componentwise reciprocal a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise reciprocal of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 rcp(fp4 x) { return fp.one / x; }


        /// <summary>Returns the sign of a number value. -1.0f if it is less than zero, 0.0f if it is zero and 1.0f if it greater than zero.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp sign(fp x) { return (fp)fp.Sign(x); }

        /// <summary>Returns the componentwise sign of a float2 value. 1.0f for positive components, 0.0f for zero components and -1.0f for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 sign(fp2 x) { return new fp2(sign(x.x), sign(x.y)); }

        /// <summary>Returns the componentwise sign of a float3 value. 1.0f for positive components, 0.0f for zero components and -1.0f for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 sign(fp3 x) { return new fp3(sign(x.x), sign(x.y), sign(x.z)); }

        /// <summary>Returns the componentwise sign of a float4 value. 1.0f for positive components, 0.0f for zero components and -1.0f for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 sign(fp4 x) { return new fp4(sign(x.x), sign(x.y), sign(x.z), sign(x.w)); }


        /// <summary>Returns x raised to the power y.</summary>
        /// <param name="x">The exponent base.</param>
        /// <param name="y">The exponent power.</param>
        /// <returns>The result of raising x to the power y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp pow(fp x, fp y) { return fp.Pow(x, y); }

        /// <summary>Returns the componentwise result of raising x to the power y.</summary>
        /// <param name="x">The exponent base.</param>
        /// <param name="y">The exponent power.</param>
        /// <returns>The componentwise result of raising x to the power y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 pow(fp2 x, fp2 y) { return new fp2(pow(x.x, y.x), pow(x.y, y.y)); }

        /// <summary>Returns the componentwise result of raising x to the power y.</summary>
        /// <param name="x">The exponent base.</param>
        /// <param name="y">The exponent power.</param>
        /// <returns>The componentwise result of raising x to the power y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 pow(fp3 x, fp3 y) { return new fp3(pow(x.x, y.x), pow(x.y, y.y), pow(x.z, y.z)); }

        /// <summary>Returns the componentwise result of raising x to the power y.</summary>
        /// <param name="x">The exponent base.</param>
        /// <param name="y">The exponent power.</param>
        /// <returns>The componentwise result of raising x to the power y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 pow(fp4 x, fp4 y) { return new fp4(pow(x.x, y.x), pow(x.y, y.y), pow(x.z, y.z), pow(x.w, y.w)); }


        /// <summary>Returns the base-e exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The base-e exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp exp(fp x) { return fp.Exp(x); }

        /// <summary>Returns the componentwise base-e exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-e exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 exp(fp2 x) { return new fp2(exp(x.x), exp(x.y)); }

        /// <summary>Returns the componentwise base-e exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-e exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 exp(fp3 x) { return new fp3(exp(x.x), exp(x.y), exp(x.z)); }

        /// <summary>Returns the componentwise base-e exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-e exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 exp(fp4 x) { return new fp4(exp(x.x), exp(x.y), exp(x.z), exp(x.w)); }


        /// <summary>Returns the base-2 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The base-2 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp exp2(fp x) { return fp.Pow((fp)2, x); }

        /// <summary>Returns the componentwise base-2 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-2 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 exp2(fp2 x) { return new fp2(exp2(x.x), exp2(x.y)); }

        /// <summary>Returns the componentwise base-2 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-2 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 exp2(fp3 x) { return new fp3(exp2(x.x), exp2(x.y), exp2(x.z)); }

        /// <summary>Returns the componentwise base-2 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-2 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 exp2(fp4 x) { return new fp4(exp2(x.x), exp2(x.y), exp2(x.z), exp2(x.w)); }


        /// <summary>Returns the base-10 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The base-10 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp exp10(fp x) { return fp.Pow((fp)10, x); }

        /// <summary>Returns the componentwise base-10 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-10 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 exp10(fp2 x) { return new fp2(exp10(x.x), exp10(x.y)); }

        /// <summary>Returns the componentwise base-10 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-10 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 exp10(fp3 x) { return new fp3(exp10(x.x), exp10(x.y), exp10(x.z)); }

        /// <summary>Returns the componentwise base-10 exponential of x.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-10 exponential of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 exp10(fp4 x) { return new fp4(exp10(x.x), exp10(x.y), exp10(x.z), exp10(x.w)); }


        /// <summary>Returns the natural logarithm of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The natural logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp log(fp x) { return fp.Log(x); }

        /// <summary>Returns the componentwise natural logarithm of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise natural logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 log(fp2 x) { return new fp2(log(x.x), log(x.y)); }

        /// <summary>Returns the componentwise natural logarithm of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise natural logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 log(fp3 x) { return new fp3(log(x.x), log(x.y), log(x.z)); }

        /// <summary>Returns the componentwise natural logarithm of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise natural logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 log(fp4 x) { return new fp4(log(x.x), log(x.y), log(x.z), log(x.w)); }


        /// <summary>Returns the base-2 logarithm of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The base-2 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp log2(fp x) { return fp.Log2(x); }

        /// <summary>Returns the componentwise base-2 logarithm of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-2 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 log2(fp2 x) { return new fp2(log2(x.x), log2(x.y)); }

        /// <summary>Returns the componentwise base-2 logarithm of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-2 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 log2(fp3 x) { return new fp3(log2(x.x), log2(x.y), log2(x.z)); }

        /// <summary>Returns the componentwise base-2 logarithm of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-2 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 log2(fp4 x) { return new fp4(log2(x.x), log2(x.y), log2(x.z), log2(x.w)); }

        /// <summary>Returns the base-10 logarithm of a number value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The base-10 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp log10(fp x) { return fp.Log10(x); }

        /// <summary>Returns the componentwise base-10 logarithm of a float2 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-10 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 log10(fp2 x) { return new fp2(log10(x.x), log10(x.y)); }

        /// <summary>Returns the componentwise base-10 logarithm of a float3 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-10 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 log10(fp3 x) { return new fp3(log10(x.x), log10(x.y), log10(x.z)); }

        /// <summary>Returns the componentwise base-10 logarithm of a float4 vector.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise base-10 logarithm of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 log10(fp4 x) { return new fp4(log10(x.x), log10(x.y), log10(x.z), log10(x.w)); }


        /// <summary>Returns the fix point remainder of x/y.</summary>
        /// <param name="x">The dividend in x/y.</param>
        /// <param name="y">The divisor in x/y.</param>
        /// <returns>The remainder of x/y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp fmod(fp x, fp y) { return x % y; }

        /// <summary>Returns the componentwise fix point remainder of x/y.</summary>
        /// <param name="x">The dividend in x/y.</param>
        /// <param name="y">The divisor in x/y.</param>
        /// <returns>The componentwise remainder of x/y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 fmod(fp2 x, fp2 y) { return new fp2(x.x % y.x, x.y % y.y); }

        /// <summary>Returns the componentwise fix point remainder of x/y.</summary>
        /// <param name="x">The dividend in x/y.</param>
        /// <param name="y">The divisor in x/y.</param>
        /// <returns>The componentwise remainder of x/y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 fmod(fp3 x, fp3 y) { return new fp3(x.x % y.x, x.y % y.y, x.z % y.z); }

        /// <summary>Returns the componentwise fix point remainder of x/y.</summary>
        /// <param name="x">The dividend in x/y.</param>
        /// <param name="y">The divisor in x/y.</param>
        /// <returns>The componentwise remainder of x/y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 fmod(fp4 x, fp4 y) { return new fp4(x.x % y.x, x.y % y.y, x.z % y.z, x.w % y.w); }


        /// <summary>Splits a number value into an integral part i and a fractional part that gets returned. Both parts take the sign of the input.</summary>
        /// <param name="x">Value to split into integral and fractional part.</param>
        /// <param name="i">Output value containing integral part of x.</param>
        /// <returns>The fractional part of x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp modf(fp x, out fp i) { i = trunc(x); return x - i; }

        /// <summary>
        /// Performs a componentwise split of a float2 vector into an integral part i and a fractional part that gets returned.
        /// Both parts take the sign of the corresponding input component.
        /// </summary>
        /// <param name="x">Value to split into integral and fractional part.</param>
        /// <param name="i">Output value containing integral part of x.</param>
        /// <returns>The componentwise fractional part of x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 modf(fp2 x, out fp2 i) { i = trunc(x); return x - i; }

        /// <summary>
        /// Performs a componentwise split of a float3 vector into an integral part i and a fractional part that gets returned.
        /// Both parts take the sign of the corresponding input component.
        /// </summary>
        /// <param name="x">Value to split into integral and fractional part.</param>
        /// <param name="i">Output value containing integral part of x.</param>
        /// <returns>The componentwise fractional part of x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 modf(fp3 x, out fp3 i) { i = trunc(x); return x - i; }

        /// <summary>
        /// Performs a componentwise split of a float4 vector into an integral part i and a fractional part that gets returned.
        /// Both parts take the sign of the corresponding input component.
        /// </summary>
        /// <param name="x">Value to split into integral and fractional part.</param>
        /// <param name="i">Output value containing integral part of x.</param>
        /// <returns>The componentwise fractional part of x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 modf(fp4 x, out fp4 i) { i = trunc(x); return x - i; }


        /// <summary>Returns the square root of a number value.</summary>
        /// <param name="x">Value to use when computing square root.</param>
        /// <returns>The square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp sqrt(fp x) { return fp.Sqrt(x); }

        /// <summary>Returns the componentwise square root of a float2 vector.</summary>
        /// <param name="x">Value to use when computing square root.</param>
        /// <returns>The componentwise square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 sqrt(fp2 x) { return new fp2(sqrt(x.x), sqrt(x.y)); }

        /// <summary>Returns the componentwise square root of a float3 vector.</summary>
        /// <param name="x">Value to use when computing square root.</param>
        /// <returns>The componentwise square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 sqrt(fp3 x) { return new fp3(sqrt(x.x), sqrt(x.y), sqrt(x.z)); }

        /// <summary>Returns the componentwise square root of a float4 vector.</summary>
        /// <param name="x">Value to use when computing square root.</param>
        /// <returns>The componentwise square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 sqrt(fp4 x) { return new fp4(sqrt(x.x), sqrt(x.y), sqrt(x.z), sqrt(x.w)); }


        /// <summary>Returns the reciprocal square root of a number value.</summary>
        /// <param name="x">Value to use when computing reciprocal square root.</param>
        /// <returns>The reciprocal square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp rsqrt(fp x) { var y = sqrt(x); return y.RawValue == 0 ? 0 : fp.one / y; }

        /// <summary>Returns the componentwise reciprocal square root of a float2 vector.</summary>
        /// <param name="x">Value to use when computing reciprocal square root.</param>
        /// <returns>The componentwise reciprocal square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 rsqrt(fp2 x) { return fp.one / sqrt(x); }

        /// <summary>Returns the componentwise reciprocal square root of a float3 vector.</summary>
        /// <param name="x">Value to use when computing reciprocal square root.</param>
        /// <returns>The componentwise reciprocal square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 rsqrt(fp3 x) { return fp.one / sqrt(x); }

        /// <summary>Returns the componentwise reciprocal square root of a float4 vector</summary>
        /// <param name="x">Value to use when computing reciprocal square root.</param>
        /// <returns>The componentwise reciprocal square root.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 rsqrt(fp4 x) { return fp.one / sqrt(x); }


        /// <summary>Returns a normalized version of the float2 vector x by scaling it by 1 / length(x).</summary>
        /// <param name="x">Vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 normalize(fp2 x) { return rsqrt(dot(x, x)) * x; }

        /// <summary>Returns a normalized version of the float3 vector x by scaling it by 1 / length(x).</summary>
        /// <param name="x">Vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 normalize(fp3 x) { return rsqrt(dot(x, x)) * x; }

        /// <summary>Returns a normalized version of the float4 vector x by scaling it by 1 / length(x).</summary>
        /// <param name="x">Vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 normalize(fp4 x) { return rsqrt(dot(x, x)) * x; }

        /// <summary>
        /// Returns a safe normalized version of the float2 vector x by scaling it by 1 / length(x).
        /// Returns the given default value when 1 / length(x) does not produce a finite number.
        /// </summary>
        /// <param name="x">Vector to normalize.</param>
        /// <param name="defaultvalue">Vector to return if normalized vector is not finite.</param>
        /// <returns>The normalized vector or the default value if the normalized vector is not finite.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public fp normalizesafe(fp x, fp defaultvalue = new fp())
        {
            var len = dot(x, x);
            return len > fp.MinNormal ? x * rsqrt(len) : defaultvalue;
        }


        /// <summary>
        /// Returns a safe normalized version of the float3 vector x by scaling it by 1 / length(x).
        /// Returns the given default value when 1 / length(x) does not produce a finite number.
        /// </summary>
        /// <param name="x">Vector to normalize.</param>
        /// <param name="defaultvalue">Vector to return if normalized vector is not finite.</param>
        /// <returns>The normalized vector or the default value if the normalized vector is not finite.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public fp3 normalizesafe(fp3 x, fp3 defaultvalue = new fp3())
        {
            fp len = dot(x, x);
            return len > fp.MinNormal ? x * rsqrt(len) : defaultvalue;
        }

        /// <summary>
        /// Returns a safe normalized version of the float4 vector x by scaling it by 1 / length(x).
        /// Returns the given default value when 1 / length(x) does not produce a finite number.
        /// </summary>
        /// <param name="x">Vector to normalize.</param>
        /// <param name="defaultvalue">Vector to return if normalized vector is not finite.</param>
        /// <returns>The normalized vector or the default value if the normalized vector is not finite.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public fp4 normalizesafe(fp4 x, fp4 defaultvalue = new fp4())
        {
            fp len = dot(x, x);
            return len > fp.MinNormal ? x * rsqrt(len) : defaultvalue;
        }


        /// <summary>Returns the length of a number value. Equivalent to the absolute value.</summary>
        /// <param name="x">Value to use when computing length.</param>
        /// <returns>Length of x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp length(fp x) { return abs(x); }

        /// <summary>Returns the length of a float2 vector.</summary>
        /// <param name="x">Vector to use when computing length.</param>
        /// <returns>Length of vector x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp length(fp2 x) { return sqrt(dot(x, x)); }
        //{
        //    long _dot = (x.x.RawValue * x.x.RawValue >> number.FRACTIONAL_PLACES) + (x.y.RawValue * x.y.RawValue >> number.FRACTIONAL_PLACES);
        //    x.x.RawValue = number.SqrtRaw(_dot);
        //    return x.x;
        //}
        //50MOps/s
        //{
        //    bool isnegX = x.x.RawValue < 0L, isnegY = x.y.RawValue < 0L;
        //    long xRawValue = isnegX ? -x.x.RawValue : x.x.RawValue;
        //    long yRawValue = isnegY ? -x.y.RawValue : x.y.RawValue;
        //    bool bHighArea = xRawValue < yRawValue;//true:(Pi/4, Pi/2] or false:[0, Pi/4]
        //    long idx = bHighArea ? xRawValue : yRawValue;
        //    xRawValue = bHighArea ? yRawValue : xRawValue;
        //    yRawValue = idx;
        //    idx = (yRawValue << number.FRACTIONAL_PLACES) / xRawValue;
        //    yRawValue = NumberLut.normalize2_x_lut[idx];
        //    x.x.RawValue = (xRawValue << number.FRACTIONAL_PLACES) / yRawValue;
        //    return x.x;
        //}

        /// <summary>Returns the length of a float3 vector.</summary>
        /// <param name="x">Vector to use when computing length.</param>
        /// <returns>Length of vector x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp length(fp3 x) { return sqrt(dot(x, x)); }

        /// <summary>Returns the length of a float4 vector.</summary>
        /// <param name="x">Vector to use when computing length.</param>
        /// <returns>Length of vector x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp length(fp4 x) { return sqrt(dot(x, x)); }


        /// <summary>Returns the squared length of a number value. Equivalent to squaring the value.</summary>
        /// <param name="x">Value to use when computing squared length.</param>
        /// <returns>Squared length of x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp lengthsq(fp x) { return x * x; }

        /// <summary>Returns the squared length of a float2 vector.</summary>
        /// <param name="x">Vector to use when computing squared length.</param>
        /// <returns>Squared length of vector x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp lengthsq(fp2 x) { return dot(x, x); }

        /// <summary>Returns the squared length of a float3 vector.</summary>
        /// <param name="x">Vector to use when computing squared length.</param>
        /// <returns>Squared length of vector x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp lengthsq(fp3 x) { return dot(x, x); }

        /// <summary>Returns the squared length of a float4 vector.</summary>
        /// <param name="x">Vector to use when computing squared length.</param>
        /// <returns>Squared length of vector x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp lengthsq(fp4 x) { return dot(x, x); }


        /// <summary>Returns the distance between two number values.</summary>
        /// <param name="x">First value to use in distance computation.</param>
        /// <param name="y">Second value to use in distance computation.</param>
        /// <returns>The distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distance(fp x, fp y) { return abs(y - x); }

        /// <summary>Returns the distance between two float2 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distance(fp2 x, fp2 y) { return length(y - x); }

        /// <summary>Returns the distance between two float3 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distance(fp3 x, fp3 y) { return length(y - x); }

        /// <summary>Returns the distance between two float4 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distance(fp4 x, fp4 y) { return length(y - x); }


        /// <summary>Returns the distance between two float3 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lessDistance(fp3 x, fp3 y, fp checkValue) { return distancesq(x, y) < checkValue * checkValue; }


        /// <summary>Returns the squared distance between two number values.</summary>
        /// <param name="x">First value to use in distance computation.</param>
        /// <param name="y">Second value to use in distance computation.</param>
        /// <returns>The squared distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distancesq(fp x, fp y) { return (y - x) * (y - x); }

        /// <summary>Returns the squared distance between two float2 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The squared distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distancesq(fp2 x, fp2 y) { return lengthsq(y - x); }

        /// <summary>Returns the squared distance between two float3 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The squared distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distancesq(fp3 x, fp3 y) { return lengthsq(y - x); }

        /// <summary>Returns the squared distance between two float4 vectors.</summary>
        /// <param name="x">First vector to use in distance computation.</param>
        /// <param name="y">Second vector to use in distance computation.</param>
        /// <returns>The squared distance between x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp distancesq(fp4 x, fp4 y) { return lengthsq(y - x); }


        /// <summary>Returns the cross product of two float3 vectors.</summary>
        /// <param name="x">First vector to use in cross product.</param>
        /// <param name="y">Second vector to use in cross product.</param>
        /// <returns>The cross product of x and y.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 cross(fp3 x, fp3 y) { return (x * y.yzx - x.yzx * y).yzx; }


        /// <summary>Returns a smooth Hermite interpolation between 0.0f and 1.0f when x is in [a, b].</summary>
        /// <param name="a">The minimum range of the x parameter.</param>
        /// <param name="b">The maximum range of the x parameter.</param>
        /// <param name="x">The value to be interpolated.</param>
        /// <returns>Returns a value camped to the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp smoothstep(fp a, fp b, fp x)
        {
            var t = saturate((x - a) / (b - a));
            return t * t * (fp.three - (fp.two * t));
        }

        /// <summary>Returns a componentwise smooth Hermite interpolation between 0.0f and 1.0f when x is in [a, b].</summary>
        /// <param name="a">The minimum range of the x parameter.</param>
        /// <param name="b">The maximum range of the x parameter.</param>
        /// <param name="x">The value to be interpolated.</param>
        /// <returns>Returns component values camped to the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 smoothstep(fp2 a, fp2 b, fp2 x)
        {
            var t = saturate((x - a) / (b - a));
            return t * t * (fp.three - (fp.two * t));
        }

        /// <summary>Returns a componentwise smooth Hermite interpolation between 0.0f and 1.0f when x is in [a, b].</summary>
        /// <param name="a">The minimum range of the x parameter.</param>
        /// <param name="b">The maximum range of the x parameter.</param>
        /// <param name="x">The value to be interpolated.</param>
        /// <returns>Returns component values camped to the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 smoothstep(fp3 a, fp3 b, fp3 x)
        {
            var t = saturate((x - a) / (b - a));
            return t * t * (fp.three - (fp.two * t));
        }

        /// <summary>Returns a componentwise smooth Hermite interpolation between 0.0f and 1.0f when x is in [a, b].</summary>
        /// <param name="a">The minimum range of the x parameter.</param>
        /// <param name="b">The maximum range of the x parameter.</param>
        /// <param name="x">The value to be interpolated.</param>
        /// <returns>Returns component values camped to the range [0, 1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 smoothstep(fp4 a, fp4 b, fp4 x)
        {
            var t = saturate((x - a) / (b - a));
            return t * t * (fp.three - (fp.two * t));
        }


        /// <summary>Returns true if any component of the input float2 vector is non-zero, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if any the components of x are non-zero, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool any(fp2 x) { return x.x != 0 || x.y != 0; }

        /// <summary>Returns true if any component of the input float3 vector is non-zero, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if any the components of x are non-zero, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool any(fp3 x) { return x.x != 0 || x.y != 0 || x.z != 0; }

        /// <summary>Returns true if any component of the input float4 vector is non-zero, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if any the components of x are non-zero, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool any(fp4 x) { return x.x != 0 || x.y != 0 || x.z != 0 || x.w != 0; }


        /// <summary>Returns true if any component of the input float2 vector is NaN, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if any the components of x are NaN, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool anyNaN(fp2 x) { return fp.IsNaN(x.x) || fp.IsNaN(x.y); }

        /// <summary>Returns true if any component of the input float3 vector is NaN, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if any the components of x are NaN, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool anyNaN(fp3 x) { return fp.IsNaN(x.x) || fp.IsNaN(x.y) || fp.IsNaN(x.z); }

        /// <summary>Returns true if any component of the input float4 vector is NaN, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if any the components of x are NaN, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool anyNaN(fp4 x) { return fp.IsNaN(x.x) || fp.IsNaN(x.y) || fp.IsNaN(x.z) || fp.IsNaN(x.w); }


        /// <summary>Returns true if all components of the input float2 vector are non-zero, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if all the components of x are non-zero, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool all(fp2 x) { return x.x != 0 && x.y != 0; }

        /// <summary>Returns true if all components of the input float3 vector are non-zero, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if all the components of x are non-zero, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool all(fp3 x) { return x.x != 0 && x.y != 0 && x.z != 0; }

        /// <summary>Returns true if all components of the input float4 vector are non-zero, false otherwise.</summary>
        /// <param name="x">Vector of values to compare.</param>
        /// <returns>True if all the components of x are non-zero, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool all(fp4 x) { return x.x != 0 && x.y != 0 && x.z != 0 && x.w != 0; }


        /// <summary>Returns b if c is true, a otherwise.</summary>
        /// <param name="a">Value to use if c is false.</param>
        /// <param name="b">Value to use if c is true.</param>
        /// <param name="c">Bool value to choose between a and b.</param>
        /// <returns>The selection between a and b according to bool c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp select(fp a, fp b, bool c) { return c ? b : a; }

        /// <summary>Returns b if c is true, a otherwise.</summary>
        /// <param name="a">Value to use if c is false.</param>
        /// <param name="b">Value to use if c is true.</param>
        /// <param name="c">Bool value to choose between a and b.</param>
        /// <returns>The selection between a and b according to bool c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 select(fp2 a, fp2 b, bool c) { return c ? b : a; }

        /// <summary>Returns b if c is true, a otherwise.</summary>
        /// <param name="a">Value to use if c is false.</param>
        /// <param name="b">Value to use if c is true.</param>
        /// <param name="c">Bool value to choose between a and b.</param>
        /// <returns>The selection between a and b according to bool c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 select(fp3 a, fp3 b, bool c) { return c ? b : a; }

        /// <summary>Returns b if c is true, a otherwise.</summary>
        /// <param name="a">Value to use if c is false.</param>
        /// <param name="b">Value to use if c is true.</param>
        /// <param name="c">Bool value to choose between a and b.</param>
        /// <returns>The selection between a and b according to bool c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 select(fp4 a, fp4 b, bool c) { return c ? b : a; }


        /// <summary>
        /// Returns a componentwise selection between two fp4 vectors a and b based on a bool4 selection mask c.
        /// Per component, the component from b is selected when c is true, otherwise the component from a is selected.
        /// </summary>
        /// <param name="a">Values to use if c is false.</param>
        /// <param name="b">Values to use if c is true.</param>
        /// <param name="c">Selection mask to choose between a and b.</param>
        /// <returns>The componentwise selection between a and b according to selection mask c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 select(fp2 a, fp2 b, bool2 c) { return new fp2(c.x ? b.x : a.x, c.y ? b.y : a.y); }

        /// <summary>
        /// Returns a componentwise selection between two fp4 vectors a and b based on a bool4 selection mask c.
        /// Per component, the component from b is selected when c is true, otherwise the component from a is selected.
        /// </summary>
        /// <param name="a">Values to use if c is false.</param>
        /// <param name="b">Values to use if c is true.</param>
        /// <param name="c">Selection mask to choose between a and b.</param>
        /// <returns>The componentwise selection between a and b according to selection mask c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 select(fp3 a, fp3 b, bool3 c) { return new fp3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z); }

        /// <summary>
        /// Returns a componentwise selection between two fp4 vectors a and b based on a bool4 selection mask c.
        /// Per component, the component from b is selected when c is true, otherwise the component from a is selected.
        /// </summary>
        /// <param name="a">Values to use if c is false.</param>
        /// <param name="b">Values to use if c is true.</param>
        /// <param name="c">Selection mask to choose between a and b.</param>
        /// <returns>The componentwise selection between a and b according to selection mask c.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 select(fp4 a, fp4 b, bool4 c) { return new fp4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w); }


        /// <summary>Returns the result of a step function where the result is 1.0f when x &gt;= y and 0.0f otherwise.</summary>
        /// <param name="y">Value to be used as a threshold for returning 1.</param>
        /// <param name="x">Value to compare against threshold y.</param>
        /// <returns>1 if the comparison x &gt;= y is true, otherwise 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp step(fp y, fp x) { return select(fp.zero, fp.one, x >= y); }

        /// <summary>Returns the result of a componentwise step function where each component is 1.0f when x &gt;= y and 0.0f otherwise.</summary>
        /// <param name="y">Vector of values to be used as a threshold for returning 1.</param>
        /// <param name="x">Vector of values to compare against threshold y.</param>
        /// <returns>1 if the componentwise comparison x &gt;= y is true, otherwise 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 step(fp2 y, fp2 x) { return select(fp2(0), fp2(1), x >= y); }

        /// <summary>Returns the result of a componentwise step function where each component is 1.0f when x &gt;= y and 0.0f otherwise.</summary>
        /// <param name="y">Vector of values to be used as a threshold for returning 1.</param>
        /// <param name="x">Vector of values to compare against threshold y.</param>
        /// <returns>1 if the componentwise comparison x &gt;= y is true, otherwise 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 step(fp3 y, fp3 x) { return select(fp3(0), fp3(1), x >= y); }

        /// <summary>Returns the result of a componentwise step function where each component is 1 when x &gt;= y and 0 otherwise.</summary>
        /// <param name="y">Vector of values to be used as a threshold for returning 1.</param>
        /// <param name="x">Vector of values to compare against threshold y.</param>
        /// <returns>1 if the componentwise comparison x &gt;= y is true, otherwise 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 step(fp4 y, fp4 x) { return select(fp4(0), fp4(1), x >= y); }


        /// <summary>Given an incident vector i and a normal vector n, returns the reflection vector r = i - 2.0f * dot(i, n) * n.</summary>
        /// <param name="i">Incident vector.</param>
        /// <param name="n">Normal vector.</param>
        /// <returns>Reflection vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 reflect(fp2 i, fp2 n) { return i - fp.two * n * dot(i, n); }

        /// <summary>Given an incident vector i and a normal vector n, returns the reflection vector r = i - 2.0f * dot(i, n) * n.</summary>
        /// <param name="i">Incident vector.</param>
        /// <param name="n">Normal vector.</param>
        /// <returns>Reflection vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 reflect(fp3 i, fp3 n) { return i - fp.two * n * dot(i, n); }

        /// <summary>Given an incident vector i and a normal vector n, returns the reflection vector r = i - 2.0f * dot(i, n) * n.</summary>
        /// <param name="i">Incident vector.</param>
        /// <param name="n">Normal vector.</param>
        /// <returns>Reflection vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 reflect(fp4 i, fp4 n) { return i - fp.two * n * dot(i, n); }


        /// <summary>Returns the refraction vector given the incident vector i, the normal vector n and the refraction index eta.</summary>
        /// <param name="i">Incident vector.</param>
        /// <param name="n">Normal vector.</param>
        /// <param name="eta">Index of refraction.</param>
        /// <returns>Refraction vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 refract(fp2 i, fp2 n, fp eta)
        {
            fp ni = dot(n, i);
            fp k = fp.one - eta * eta * (fp.one - ni * ni);
            return select(fp.zero, eta * i - (eta * ni + sqrt(k)) * n, k >= 0);
        }

        /// <summary>Returns the refraction vector given the incident vector i, the normal vector n and the refraction index eta.</summary>
        /// <param name="i">Incident vector.</param>
        /// <param name="n">Normal vector.</param>
        /// <param name="eta">Index of refraction.</param>
        /// <returns>Refraction vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 refract(fp3 i, fp3 n, fp eta)
        {
            fp ni = dot(n, i);
            fp k = fp.one - eta * eta * (fp.one - ni * ni);
            return select(fp.zero, eta * i - (eta * ni + sqrt(k)) * n, k >= 0);
        }

        /// <summary>Returns the refraction vector given the incident vector i, the normal vector n and the refraction index eta.</summary>
        /// <param name="i">Incident vector.</param>
        /// <param name="n">Normal vector.</param>
        /// <param name="eta">Index of refraction.</param>
        /// <returns>Refraction vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 refract(fp4 i, fp4 n, fp eta)
        {
            fp ni = dot(n, i);
            fp k = fp.one - eta * eta * (fp.one - ni * ni);
            return select(fp.zero, eta * i - (eta * ni + sqrt(k)) * n, k >= 0);
        }

        /// <summary>
        /// Compute vector projection of a onto b.
        /// </summary>
        /// <remarks>
        /// Some finite vectors a and b could generate a non-finite result. This is most likely when a's components
        /// are very large (close to Single.MaxValue) or when b's components are very small (close to number.MinNormal).
        /// In these cases, you can call <see cref="projectsafe(Deterministics.Math.fp2,Deterministics.Math.fp2,Deterministics.Math.fp2)"/>
        /// which will use a given default value if the result is not finite.
        /// </remarks>
        /// <param name="a">Vector to project.</param>
        /// <param name="b">Non-zero vector to project onto.</param>
        /// <returns>Vector projection of a onto b.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 project(fp2 a, fp2 b)
        {
            return (dot(a, b) / dot(b, b)) * b;
        }

        /// <summary>
        /// Compute vector projection of a onto b.
        /// </summary>
        /// <remarks>
        /// Some finite vectors a and b could generate a non-finite result. This is most likely when a's components
        /// are very large (close to Single.MaxValue) or when b's components are very small (close to number.MinNormal).
        /// In these cases, you can call <see cref="projectsafe(Deterministics.Math.fp3,Deterministics.Math.fp3,Deterministics.Math.fp3)"/>
        /// which will use a given default value if the result is not finite.
        /// </remarks>
        /// <param name="a">Vector to project.</param>
        /// <param name="b">Non-zero vector to project onto.</param>
        /// <returns>Vector projection of a onto b.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 project(fp3 a, fp3 b)
        {
            return (dot(a, b) / dot(b, b)) * b;
        }

        /// <summary>
        /// Compute vector projection of a onto b.
        /// </summary>
        /// <remarks>
        /// Some finite vectors a and b could generate a non-finite result. This is most likely when a's components
        /// are very large (close to Single.MaxValue) or when b's components are very small (close to number.MinNormal).
        /// In these cases, you can call <see cref="projectsafe(Deterministics.Math.fp4,Deterministics.Math.fp4,Deterministics.Math.fp4)"/>
        /// which will use a given default value if the result is not finite.
        /// </remarks>
        /// <param name="a">Vector to project.</param>
        /// <param name="b">Non-zero vector to project onto.</param>
        /// <returns>Vector projection of a onto b.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 project(fp4 a, fp4 b)
        {
            return (dot(a, b) / dot(b, b)) * b;
        }

        /// <summary>
        /// Compute vector projection of a onto b. If result is not finite, then return the default value instead.
        /// </summary>
        /// <remarks>
        /// This function performs extra checks to see if the result of projecting a onto b is finite. If you know that
        /// your inputs will generate a finite result or you don't care if the result is finite, then you can call
        /// <see cref="project(Deterministics.Math.fp2,Deterministics.Math.fp2)"/> instead which is faster than this
        /// function.
        /// </remarks>
        /// <param name="a">Vector to project.</param>
        /// <param name="b">Non-zero vector to project onto.</param>
        /// <param name="defaultValue">Default value to return if projection is not finite.</param>
        /// <returns>Vector projection of a onto b or the default value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 projectsafe(fp2 a, fp2 b, fp2 defaultValue = new fp2())
        {
            var proj = project(a, b);

            return select(defaultValue, proj, math.all(isfinite(proj)));
        }

        /// <summary>
        /// Compute vector projection of a onto b. If result is not finite, then return the default value instead.
        /// </summary>
        /// <remarks>
        /// This function performs extra checks to see if the result of projecting a onto b is finite. If you know that
        /// your inputs will generate a finite result or you don't care if the result is finite, then you can call
        /// <see cref="project(Deterministics.Math.fp3,Deterministics.Math.fp3)"/> instead which is faster than this
        /// function.
        /// </remarks>
        /// <param name="a">Vector to project.</param>
        /// <param name="b">Non-zero vector to project onto.</param>
        /// <param name="defaultValue">Default value to return if projection is not finite.</param>
        /// <returns>Vector projection of a onto b or the default value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 projectsafe(fp3 a, fp3 b, fp3 defaultValue = new fp3())
        {
            var proj = project(a, b);

            return select(defaultValue, proj, math.all(isfinite(proj)));
        }

        /// <summary>
        /// Compute vector projection of a onto b. If result is not finite, then return the default value instead.
        /// </summary>
        /// <remarks>
        /// This function performs extra checks to see if the result of projecting a onto b is finite. If you know that
        /// your inputs will generate a finite result or you don't care if the result is finite, then you can call
        /// <see cref="project(Deterministics.Math.fp4,Deterministics.Math.fp4)"/> instead which is faster than this
        /// function.
        /// </remarks>
        /// <param name="a">Vector to project.</param>
        /// <param name="b">Non-zero vector to project onto.</param>
        /// <param name="defaultValue">Default value to return if projection is not finite.</param>
        /// <returns>Vector projection of a onto b or the default value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 projectsafe(fp4 a, fp4 b, fp4 defaultValue = new fp4())
        {
            var proj = project(a, b);

            return select(defaultValue, proj, math.all(isfinite(proj)));
        }

        /// <summary>Conditionally flips a vector n if two vectors i and ng are pointing in the same direction. Returns n if dot(i, ng) &lt; 0, -n otherwise.</summary>
        /// <param name="n">Vector to conditionally flip.</param>
        /// <param name="i">First vector in direction comparison.</param>
        /// <param name="ng">Second vector in direction comparison.</param>
        /// <returns>-n if i and ng point in the same direction; otherwise return n unchanged.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 faceforward(fp2 n, fp2 i, fp2 ng) { return select(n, -n, dot(ng, i) >= 0); }

        /// <summary>Conditionally flips a vector n if two vectors i and ng are pointing in the same direction. Returns n if dot(i, ng) &lt; 0, -n otherwise.</summary>
        /// <param name="n">Vector to conditionally flip.</param>
        /// <param name="i">First vector in direction comparison.</param>
        /// <param name="ng">Second vector in direction comparison.</param>
        /// <returns>-n if i and ng point in the same direction; otherwise return n unchanged.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 faceforward(fp3 n, fp3 i, fp3 ng) { return select(n, -n, dot(ng, i) >= 0); }

        /// <summary>Conditionally flips a vector n if two vectors i and ng are pointing in the same direction. Returns n if dot(i, ng) &lt; 0, -n otherwise.</summary>
        /// <param name="n">Vector to conditionally flip.</param>
        /// <param name="i">First vector in direction comparison.</param>
        /// <param name="ng">Second vector in direction comparison.</param>
        /// <returns>-n if i and ng point in the same direction; otherwise return n unchanged.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 faceforward(fp4 n, fp4 i, fp4 ng) { return select(n, -n, dot(ng, i) >= 0); }


        /// <summary>Returns the sine and cosine of the input number value x through the out parameters s and c.</summary>
        /// <remarks>When Burst compiled, his method is faster than calling sin() and cos() separately.</remarks>
        /// <param name="x">Input angle in radians.</param>
        /// <param name="s">Output sine of the input.</param>
        /// <param name="c">Output cosine of the input.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(fp x, out fp s, out fp c) { fp.SinCos(x, out s, out c); }

        /// <summary>Returns the componentwise sine and cosine of the input float2 vector x through the out parameters s and c.</summary>
        /// <remarks>When Burst compiled, his method is faster than calling sin() and cos() separately.</remarks>
        /// <param name="x">Input vector containing angles in radians.</param>
        /// <param name="s">Output vector containing the componentwise sine of the input.</param>
        /// <param name="c">Output vector containing the componentwise cosine of the input.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(fp2 x, out fp2 s, out fp2 c) { s = sin(x); c = cos(x); }

        /// <summary>Returns the componentwise sine and cosine of the input float3 vector x through the out parameters s and c.</summary>
        /// <remarks>When Burst compiled, his method is faster than calling sin() and cos() separately.</remarks>
        /// <param name="x">Input vector containing angles in radians.</param>
        /// <param name="s">Output vector containing the componentwise sine of the input.</param>
        /// <param name="c">Output vector containing the componentwise cosine of the input.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(fp3 x, out fp3 s, out fp3 c) { 
            fp.SinCos(x.x.RawValue, out var sx, out var cx);
            fp.SinCos(x.y.RawValue, out var sy, out var cy);
            fp.SinCos(x.z.RawValue, out var sz, out var cz);
            s = new fp3(sx, sy, sz);
            c = new fp3(cx, cy, cz);
        }

        /// <summary>Returns the componentwise sine and cosine of the input float4 vector x through the out parameters s and c.</summary>
        /// <remarks>When Burst compiled, his method is faster than calling sin() and cos() separately.</remarks>
        /// <param name="x">Input vector containing angles in radians.</param>
        /// <param name="s">Output vector containing the componentwise sine of the input.</param>
        /// <param name="c">Output vector containing the componentwise cosine of the input.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(fp4 x, out fp4 s, out fp4 c) { s = sin(x); c = cos(x); }

        /// <summary>Returns the result of converting a number value from degrees to radians.</summary>
        /// <param name="x">Angle in degrees.</param>
        /// <returns>Angle converted to radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp radians(fp x) { return x * fp.Deg2Rad; }

        /// <summary>Returns the result of a componentwise conversion of a float2 vector from degrees to radians.</summary>
        /// <param name="x">Vector containing angles in degrees.</param>
        /// <returns>Vector containing angles converted to radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 radians(fp2 x) { return x * fp.Deg2Rad; }

        /// <summary>Returns the result of a componentwise conversion of a float3 vector from degrees to radians.</summary>
        /// <param name="x">Vector containing angles in degrees.</param>
        /// <returns>Vector containing angles converted to radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 radians(fp3 x) { return x * fp.Deg2Rad; }

        /// <summary>Returns the result of a componentwise conversion of a float4 vector from degrees to radians.</summary>
        /// <param name="x">Vector containing angles in degrees.</param>
        /// <returns>Vector containing angles converted to radians.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 radians(fp4 x) { return x * fp.Deg2Rad; }


        /// <summary>Returns the result of converting a fp value from radians to degrees.</summary>
        /// <param name="x">Angle in radians.</param>
        /// <returns>Angle converted to degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp degrees(fp x) { return x * fp.Rad2Deg; }

        /// <summary>Returns the result of a componentwise conversion of a fp2 vector from radians to degrees.</summary>
        /// <param name="x">Vector containing angles in radians.</param>
        /// <returns>Vector containing angles converted to degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp2 degrees(fp2 x) { return x * fp.Rad2Deg; }

        /// <summary>Returns the result of a componentwise conversion of a fp3 vector from radians to degrees.</summary>
        /// <param name="x">Vector containing angles in radians.</param>
        /// <returns>Vector containing angles converted to degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 degrees(fp3 x) { return x * fp.Rad2Deg; }

        /// <summary>Returns the result of a componentwise conversion of a fp4 vector from radians to degrees.</summary>
        /// <param name="x">Vector containing angles in radians.</param>
        /// <returns>Vector containing angles converted to degrees.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp4 degrees(fp4 x) { return x * fp.Rad2Deg; }


        /// <summary>Returns the minimum component of a float2 vector.</summary>
        /// <param name="x">The vector to use when computing the minimum component.</param>
        /// <returns>The value of the minimum component of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cmin(fp2 x) { return min(x.x, x.y); }

        /// <summary>Returns the minimum component of a float3 vector.</summary>
        /// <param name="x">The vector to use when computing the minimum component.</param>
        /// <returns>The value of the minimum component of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cmin(fp3 x) { return min(min(x.x, x.y), x.z); }

        /// <summary>Returns the minimum component of a float4 vector.</summary>
        /// <param name="x">The vector to use when computing the minimum component.</param>
        /// <returns>The value of the minimum component of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cmin(fp4 x) { return min(min(x.x, x.y), min(x.z, x.w)); }


        /// <summary>Returns the maximum component of a float2 vector.</summary>
        /// <param name="x">The vector to use when computing the maximum component.</param>
        /// <returns>The value of the maximum component of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cmax(fp2 x) { return max(x.x, x.y); }

        /// <summary>Returns the maximum component of a float3 vector.</summary>
        /// <param name="x">The vector to use when computing the maximum component.</param>
        /// <returns>The value of the maximum component of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cmax(fp3 x) { return max(max(x.x, x.y), x.z); }

        /// <summary>Returns the maximum component of a float4 vector.</summary>
        /// <param name="x">The vector to use when computing the maximum component.</param>
        /// <returns>The value of the maximum component of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp cmax(fp4 x) { return max(max(x.x, x.y), max(x.z, x.w)); }


        /// <summary>Returns the horizontal sum of components of a float2 vector.</summary>
        /// <param name="x">The vector to use when computing the horizontal sum.</param>
        /// <returns>The horizontal sum of of components of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp csum(fp2 x) { return x.x + x.y; }

        /// <summary>Returns the horizontal sum of components of a float3 vector.</summary>
        /// <param name="x">The vector to use when computing the horizontal sum.</param>
        /// <returns>The horizontal sum of of components of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp csum(fp3 x) { return x.x + x.y + x.z; }

        /// <summary>Returns the horizontal sum of components of a float4 vector.</summary>
        /// <param name="x">The vector to use when computing the horizontal sum.</param>
        /// <returns>The horizontal sum of of components of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp csum(fp4 x) { return (x.x + x.y) + (x.z + x.w); }

        /// <summary>
        /// Computes the square (x * x) of the input argument x.
        /// </summary>
        /// <param name="x">Value to square.</param>
        /// <returns>Returns the square of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp square(fp x)
        {
            return x * x;
        }


        // /// <summary>
        // /// Generate an orthonormal basis given a single unit length normal vector.
        // /// </summary>
        // /// <remarks>
        // /// This implementation is from "Building an Orthonormal Basis, Revisited"
        // /// https://graphics.pixar.com/library/OrthonormalB/paper.pdf
        // /// </remarks>
        // /// <param name="normal">Unit length normal vector.</param>
        // /// <param name="basis1">Output unit length vector, orthogonal to normal vector.</param>
        // /// <param name="basis2">Output unit length vector, orthogonal to normal vector and basis1.</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static void orthonormal_basis(float3 normal, out float3 basis1, out float3 basis2)
        // {
        //     var sign = normal.z >= 0.0f ? 1.0f : -1.0f;
        //     var a = -1.0f / (sign + normal.z);
        //     var b = normal.x * normal.y * a;
        //     basis1.x = 1.0f + sign * normal.x * normal.x * a;
        //     basis1.y = sign * b;
        //     basis1.z = -sign * normal.x;
        //     basis2.x = b;
        //     basis2.y = sign + normal.y * normal.y * a;
        //     basis2.z = -normal.y;
        // }

        // /// <summary>
        // /// angle(Radian, not degree) between x and y
        // /// </summary>
        // /// <param name="x"></param>
        // /// <param name="y"></param>
        // /// <returns></returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static fp angle(fp2 x, fp2 y)
        // {
        //     long angle1 = fp.Atan2Raw(x.y.RawValue, x.x.RawValue);
        //     long angle2 = fp.Atan2Raw(y.y.RawValue, y.x.RawValue);
        //     x.x.RawValue = angle1 > angle2 ? angle1 - angle2 : angle2 - angle1;
        //     x.y.RawValue = x.x.RawValue > fp.RawPiLong ? fp.RawPiTimes2Long - x.x.RawValue : x.x.RawValue;
        //     return x.y;
        // }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static number angle(float3 x, float3 y)
        // {
        //     if (anyNaN(x) || anyNaN(y)) return number.NaN;
        //     var num = length(x) * length(y);
        //     if (num <= number.MinNormal) return number.zero;
        //     var ret = number.Acos(dot(x, y) / num);
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp angle(fp3 x, fp3 y, fp3 axis)
        {
            if (anyNaN(x) || anyNaN(y) || anyNaN(axis)) return fp.NaN;
            var axis0 = cross(x, y);
            var dotValue = dot(x, y);
            if (axis0.Equals(Deterministics.Math.fp3.zero)) return dotValue > 0 ? fp.zero : fp.PI;
            var num = length(x) * length(y);
            if (num <= fp.MinNormal) return fp.zero;
            var ang = fp.Acos(dotValue / num);
            return dot(axis0, axis) >= fp.zero ? ang : fp.PITimes2 - ang;
        }

        public static bool Approximately(float a, fp num, float toleranceRate = 0.001f)
        {
            float b = num;
            var x = 0.01f;
            var max = math.max(math.abs(a), math.abs(b)) * toleranceRate;
            x = math.max(max, x);

            return math.abs(a - b) < x;
        }

        public static bool Approximately(fp a, float b, float toleranceRate = 0.001f)
        {
            return Approximately(b, a, toleranceRate);
        }

        public static int asint(fp a)
        {
            return a.GetHashCode();
        }
    }
}
