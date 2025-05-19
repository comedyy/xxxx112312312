using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using static Deterministics.Math.fpMath;

namespace Deterministics.Math
{
    public enum RotationOrder : byte
    {
        XYZ,
        XZY,
        YXZ,
        YZX,
        ZXY,
        ZYX,
        Default = ZXY
    }
    [System.Serializable]
    [Unity.IL2CPP.CompilerServices.Il2CppEagerStaticClassConstruction]
    public partial struct fpQuaternion : IEquatable<fpQuaternion>, IFormattable
    {
        public fp4 value;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="w">w</param>
        public fpQuaternion(fp x, fp y, fp z, fp w)
        {
            this.value = new fp4(x, y, z, w);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value">float4</param>
        public fpQuaternion(fp4 value)
        {
            this.value = value;
        }

        /// <summary>
        /// ctor. from matrix 3x3
        /// </summary>
        /// <param name="m">matrix</param>
        public fpQuaternion(fp3x3 m)
        {
            fp3 u = m.c0;
            fp3 v = m.c1;
            fp3 w = m.c2;
            fp s = new fp(constant.sign64);
            fp u_sign = u.x & s;
            fp t = u_sign.RawValue == 0L ? (v.y + w.z) : (v.y - w.z);
            fp4 u_mask = new fp4(u_sign >> 63);
            fp4 t_mask = new fp4(t >> 63);
            fp tr = fp.one + fp.Abs(u.x);
            fp4 sign_flips = new fp4(fp.zero, s, s, s) ^ (u_mask & new fp4(fp.zero, s, fp.zero, s)) 
                ^ (t_mask & new fp4(s, s, s, fp.zero));
            bool bSign0 = sign_flips.x.RawValue == 0L;
            bool bSign1 = sign_flips.y.RawValue == 0L;
            bool bSign2 = sign_flips.z.RawValue == 0L;
            bool bSign3 = sign_flips.w.RawValue == 0L;
            value = new fp4(tr, u.y, w.x, v.z) + new fp4(
                bSign0 ? t : -t,
                bSign1 ? v.x : -v.x,
                bSign2 ? u.z : -u.z,
                bSign3 ? w.y : -w.y);

            value = (value & ~u_mask) | (value.zwxy & u_mask);
            value = (value.wzyx & ~t_mask) | (value & t_mask);
            value = normalize(value);
        }

        // /// <summary>
        // /// get or set value of x/y/z/w by index
        // /// </summary>
        // /// <param name="index">index</param>
        // /// <returns>value of x/y/z/w</returns>
        // public number this[int index]
        // {
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     get
        //     {
        //         switch(index)
        //         {
        //             case 0: return this.x;
        //             case 1: return this.y;
        //             case 2: return this.z;
        //             case 3: return this.w;
        //             default: throw new IndexOutOfRangeException($"Invalid quaternion index! {index}");
        //         }
        //     }
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     set
        //     {
        //         switch (index)
        //         {
        //             case 0:
        //                 this.x = value;
        //                 break;
        //             case 1:
        //                 this.y = value;
        //                 break;
        //             case 2:
        //                 this.z = value;
        //                 break;
        //             case 3:
        //                 this.w = value;
        //                 break;
        //             default:
        //                 throw new IndexOutOfRangeException($"Invalid quaternion index! {index}");
        //         }
        //     }
        // }

        /// <summary>
        /// quaternion(0, 0, 0, 1)
        /// </summary>
        public static fpQuaternion identity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new fpQuaternion(fp.zero, fp.zero, fp.zero, fp.one);
        }

        // /// <summary>
        // /// get or set euler angles (ZXY)
        // /// </summary>
        // public float3 eulerAngles
        // {
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     get
        //     {
        //         //��ZXY˳�򣬽���ŷ����
        //         float3 euler;
        //         number t = w * x - y * z;
        //         if (number.Abs(t).RawValue > number.half.RawValue - number.SMALL_SQRT) // ������̬,������Ϊ��90��
        //         {
        //             bool bSign = (t.RawValue & long.MinValue) == 0L;
        //             euler.x = bSign ? number.PIDiv2 : -number.PIDiv2;//pitch
        //             //�˿̣�w * y + x * z = 0 �� number.half - x * x - y * y = 0�����Բ���ֱ�������·�ʽ����euler.y
        //             //euler.y = number.Atan2(w * y + x * z, number.half - x * x - y * y);//yaw
        //             euler.y = (bSign ? -2 : 2) * number.Atan2(z, w);
        //             euler.z = number.zero;//roll
        //         }
        //         else
        //         {
        //             euler.x = number.Asin(2 * t);
        //             euler.y = number.Atan2(w * y + x * z, number.half - x * x - y * y);
        //             euler.z = number.Atan2(w * z + x * y, number.half - x * x - z * z);
        //         }
        //         euler *= number.Rad2Deg;
        //         return euler;
        //     }
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     set
        //     {
        //         //��ZXY˳�򣬶�ӦUnity.Mathematics.quaternion.EulerZXY
        //         float3 rad = number.half * value * number.Deg2Rad;
        //         math.sincos(rad, out float3 s, out float3 c);
        //         x = s.x * c.y * c.z + c.x * s.y * s.z;
        //         y = c.x * s.y * c.z - s.x * c.y * s.z;
        //         z = c.x * c.y * s.z - s.x * s.y * c.z;
        //         w = c.x * c.y * c.z + s.x * s.y * s.z;
        //     }
        // }

        /// <summary>
        /// get normalized quaternion
        /// </summary>
        public fpQuaternion normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => fpQuaternion.Normalize(this);
        }

        // /// <summary>
        // /// get the angle between two quaternion a and b
        // /// </summary>
        // /// <param name="a"></param>
        // /// <param name="b"></param>
        // /// <returns></returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static number Angle(quaternion a, quaternion b)
        // {
        //     number dot = quaternion.Dot(a, b);
        //     bool isEqual = quaternion.IsEqualUsingDot(dot);
        //     return isEqual ? number.zero : (number.Acos(number.Min(number.Abs(dot), number.one)) * 2 * number.Rad2Deg);
        // }

        /// <summary>
        /// construct quaternion from axis and angle
        /// </summary>
        /// <param name="angle">the degree of angle</param>
        /// <param name="axis">the axis of normalized vector3</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static quaternion AngleAxis(number angle, float3 axis)
        // {
        //     math.sincos(number.half * angle * number.Deg2Rad, out number sina, out number cosa);
        //     return new quaternion(math.float4(axis * sina, cosa));
        // }

        /// <summary>
        /// construct quaternion from axis and angle
        /// </summary>
        /// <param name="axis">the aixs</param>
        /// <param name="angle">the radian of angle</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion AxisAngle(fp3 axis, fp angle)
        {
            sincos(fp.half * angle, out fp sina, out fp cosa);
            return new fpQuaternion(fp4(axis * sina, cosa));
        }

        /// <summary>
        /// dot value of two quaternion a and b
        /// </summary>
        /// <param name="a">quaternion a</param>
        /// <param name="b">quaternion b</param>
        /// <returns>dot value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp Dot(fpQuaternion a, fpQuaternion b) => dot(a.value, b.value);

#region  Euler
        /// <summary>
        /// construct quaternion from euler angles.(XYZ)
        /// </summary>
        /// <param name="euler">euler angles in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerXYZ(fp3 xyz)
        {
            sincos(fp.half * xyz, out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, 1, -1, 1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(XZY)
        /// </summary>
        /// <param name="euler">euler angles in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerXZY(fp3 xyz)
        {
            sincos(fp.half * xyz, out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, 1, -1, -1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(YXZ)
        /// </summary>
        /// <param name="euler">euler angles in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerYXZ(fp3 xyz)
        {
            sincos(fp.half * xyz, out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, 1, 1, -1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(YZX)
        /// </summary>
        /// <param name="euler">euler angles in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerYZX(fp3 xyz)
        {
            sincos(fp.half * xyz, out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, -1, 1, 1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(ZXY)
        /// </summary>
        /// <param name="euler">euler angles in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerZXY(fp3 xyz)
        {
            sincos(fp.half * xyz, out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, -1, -1, 1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(ZYX)
        /// </summary>
        /// <param name="euler">euler angles in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerZYX(fp3 xyz)
        {
            sincos(fp.half * xyz, out fp3 s, out fp3 c);
           return fpQuaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.x * s.z
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, -1, 1, -1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(XYZ)
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerXYZ(fp x, fp y, fp z)
        {
            sincos(fp.half * new fp3(x, y, z), out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, 1, -1, 1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(XZY)
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerXZY(fp x, fp y, fp z)
        {
            sincos(fp.half * new fp3(x, y, z), out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, 1, -1, -1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(YXZ)
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerYXZ(fp x, fp y, fp z)
        {
            sincos(fp.half * new fp3(x, y, z), out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z + s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, 1, 1, -1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(YZX)
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerYZX(fp x, fp y, fp z)
        {
            sincos(fp.half * new fp3(x, y, z), out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z - s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(-1, -1, 1, 1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(ZXY)
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerZXY(fp x, fp y, fp z)
        {
            sincos(fp.half * new fp3(x, y, z), out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y - s.x * s.y * c.z,
                // c.x * c.y * c.z + s.y * s.z * s.x
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, -1, -1, 1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.(ZYX)
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion EulerZYX(fp x, fp y, fp z)
        {
            sincos(fp.half * new fp3(x, y, z), out fp3 s, out fp3 c);
            return fpQuaternion(
                // s.x * c.y * c.z + s.y * s.z * c.x,
                // s.y * c.x * c.z - s.x * s.z * c.y,
                // s.z * c.x * c.y + s.x * s.y * c.z,
                // c.x * c.y * c.z - s.y * s.x * s.z
                fp4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * fp4(c.xyz, s.x) * fp4(1, -1, 1, -1)
                );
        }

        /// <summary>
        /// construct quaternion from euler angles.
        /// </summary>
        /// <param name="xyz">euler angles in radians</param>
        /// <param name="order">rotate order</param>
        /// <returns>quaternion</returns>
        public static fpQuaternion Euler(fp3 xyz, RotationOrder order = RotationOrder.ZXY)
        {
            switch(order)
            {
                case RotationOrder.XYZ : return fpQuaternion.EulerXYZ(xyz);
                case RotationOrder.XZY : return fpQuaternion.EulerXZY(xyz);
                case RotationOrder.YXZ : return fpQuaternion.EulerYXZ(xyz);
                case RotationOrder.YZX : return fpQuaternion.EulerYZX(xyz);
                case RotationOrder.ZXY : return fpQuaternion.EulerZXY(xyz);
                case RotationOrder.ZYX : return fpQuaternion.EulerZYX(xyz);
                default : return fpQuaternion.EulerZXY(xyz);
            }
        }

        /// <summary>
        /// construct quaternion from euler angles.
        /// </summary>
        /// <param name="x">the x euler angle in radians</param>
        /// <param name="y">the y euler angle in radians</param>
        /// <param name="z">the z euler angle in radians</param>
        /// <param name="order">rotate order</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion Euler(fp x, fp y, fp z, RotationOrder order = RotationOrder.ZXY)
        {
            return Euler(new fp3(x, y, z), order);
        }

#endregion

#region Rotate
        /// <summary>
        /// construct quaternion by rotating special angle in radians around the X Axis
        /// </summary>
        /// <param name="angle">the special angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion RotateX(fp angle)
        {
            sincos(fp.half * angle, out var sina, out var cosa);
            return new fpQuaternion(sina, fp.zero, fp.zero, cosa);
        }

        /// <summary>
        /// construct quaternion by rotating special angle in radians around the Y Axis
        /// </summary>
        /// <param name="angle">the special angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion RotateY(fp angle)
        {
            sincos(fp.half * angle, out var sina, out var cosa);
            return new fpQuaternion(fp.zero, sina, fp.zero, cosa);
        }

        /// <summary>
        /// construct quaternion by rotating special angle in radians around the Z Axis
        /// </summary>
        /// <param name="angle">the special angle in radians</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion RotateZ(fp angle){
            sincos(fp.half * angle, out var sina, out var cosa);
            return new fpQuaternion(fp.zero, fp.zero, sina, cosa);
        }
#endregion

        /// <summary>
        /// construct quaternion from euler angles.(ZXY)
        /// </summary>
        /// <param name="euler">radians of euler angles. z as roll, x as pitch, y as yaw</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public static quaternion EulerAngles(float3 euler)
        // {
        //     math.sincos(number.half * euler, out float3 s, out float3 c);
        //     return new quaternion(
        //         s.x * c.y * c.z + c.x * s.y * s.z,
        //         c.x * s.y * c.z - s.x * c.y * s.z,
        //         c.x * c.y * s.z - s.x * s.y * c.z,
        //         c.x * c.y * c.z + s.x * s.y * s.z);
        // }

        /// <summary>
        /// construct quaternion from euler angles.(ZXY)
        /// </summary>
        /// <param name="x">radian of pitch</param>
        /// <param name="y">radian of yaw</param>
        /// <param name="z">radian of roll</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public static quaternion EulerAngles(number x, number y, number z)
        // {
        //     math.sincos(number.half * new float3(x, y, z), out float3 s, out float3 c);
        //     return new quaternion(
        //         s.x * c.y * c.z + c.x * s.y * s.z,
        //         c.x * s.y * c.z - s.x * c.y * s.z,
        //         c.x * c.y * s.z - s.x * s.y * c.z,
        //         c.x * c.y * c.z + s.x * s.y * s.z);
        // }

        /// <summary>
        /// construct quaternion from euler angles, same as EulerAngles.(ZXY)
        /// </summary>
        /// <param name="x">radian of pitch</param>
        /// <param name="y">radian of yaw</param>
        /// <param name="z">radian of roll</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public static quaternion EulerRotation(number x, number y, number z)
        // {
        //     math.sincos(number.half * new float3(x, y, z), out float3 s, out float3 c);
        //     return new quaternion(
        //         s.x * c.y * c.z + c.x * s.y * s.z,
        //         c.x * s.y * c.z - s.x * c.y * s.z,
        //         c.x * c.y * s.z - s.x * s.y * c.z,
        //         c.x * c.y * c.z + s.x * s.y * s.z);
        // }

        /// <summary>
        /// construct quaternion from euler angles, same as EulerAngles.(ZXY)
        /// </summary>
        /// <param name="euler">the radians of euler angles. z as roll, x as pitch, y as yaw</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public static quaternion EulerRotation(float3 euler)
        // {
        //     math.sincos(number.half * euler, out float3 s, out float3 c);
        //     return new quaternion(
        //         s.x * c.y * c.z + c.x * s.y * s.z,
        //         c.x * s.y * c.z - s.x * c.y * s.z,
        //         c.x * c.y * s.z - s.x * s.y * c.z,
        //         c.x * c.y * c.z + s.x * s.y * s.z);
        // }

        /// <summary>
        /// construct quaternion by rotating one vector to another vector 
        /// </summary>
        /// <param name="fromDirection">from vector</param>
        /// <param name="toDirection">to vector</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static quaternion FromToRotation(float3 fromDirection, float3 toDirection)
        // {
        //     number angle = math.angle(fromDirection, toDirection);
        //     float3 axis = math.normalize(math.cross(fromDirection, toDirection));
        //     return AxisAngle(axis, angle);
        // }

        /// <summary>Returns the inverse of a quaternion value.</summary>
        /// <param name="q">The quaternion to invert.</param>
        /// <returns>The quaternion inverse of the input quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion inverse(fpQuaternion q)
        {
            var x = q.value;
            return fpQuaternion(rcp(dot(x, x)) * x * fp4(-1, -1, -1, 1));
        }
        /// <summary>
        /// Linear interpolation of two quaternions
        /// </summary>
        /// <param name="q1">quaternion a</param>
        /// <param name="q2">quaternion b</param>
        /// <param name="t">coefficient</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion LerpUnclamped(fpQuaternion q1, fpQuaternion q2, fp t)
        {
            bool opposite = Dot(q1, q2).RawValue < 0L;
            if(opposite)
            {
                q2.value = -q2.value;
            }

            return new fpQuaternion(lerp(q1.value, q2.value, t));
        }

        /// <summary>
        /// construct quaternion from forward vector. float3.up as default upwards vector.
        /// </summary>
        /// <param name="forward">forward vector</param>
        /// <returns>quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion LookRotation(fp3 forward) => fpQuaternion.LookRotation(forward, fp3.up);

        /// <summary>
        /// construct quaternion from forward vector and upwards vector
        /// </summary>
        /// <param name="forward">forward vector</param>
        /// <param name="upwards">upwards vector</param>
        /// <returns>quaternion</returns>
        public static fpQuaternion LookRotation(fp3 forward, fp3 upwards)
        {
            fp3 t = normalize(cross(upwards, forward));
            return new fpQuaternion(float3x3(t, cross(forward, t), forward));
        }

        /// <summary>
        /// construct quaternion from forward vector and upwards vector in safe mode
        /// </summary>
        /// <param name="forward">forward vector</param>
        /// <param name="upwards">upwards vector</param>
        /// <returns>quaternion</returns>
        public static fpQuaternion LookRotationSafe(fp3 forward, fp3 upwards)
        {
            fp forwardLengthSq = dot(forward, forward);
            fp upLengthSq = dot(upwards, upwards);
            forward *= rsqrt(forwardLengthSq);
            upwards *= rsqrt(upLengthSq);
            fp3 t = cross(upwards, forward);
            fp tLengthSq = dot(t, t);
            t *= rsqrt(tLengthSq);
            fp mn = min(min(forwardLengthSq, upLengthSq), tLengthSq);
            fp mx = max(max(forwardLengthSq, upLengthSq), tLengthSq);
            bool accept = mn > fp.MinValue && mx < fp.MaxValue && isfinite(forwardLengthSq) && isfinite(upLengthSq) && isfinite(tLengthSq);
            return accept ? new fpQuaternion(float3x3(t, upwards, forward)) : fpQuaternion.identity;
        }

        /// <summary>
        /// normalize the quaternion
        /// </summary>
        /// <param name="q">original quaternion</param>
        /// <returns>the normalized quaternion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion Normalize(fpQuaternion q)
        {
            fp mag = fp.Sqrt(fpQuaternion.Dot(q, q));
            if (mag.RawValue == 0L) return fpQuaternion.identity;
            return new fpQuaternion(q.value / mag);
        }

        /// <summary>
        /// construct quaternion by spherical interpolation of two quaternions and the giving radian of angle. But the interpolation quaternion does not exceed the to quaternion.
        /// </summary>
        /// <param name="from">from quaternion</param>
        /// <param name="to">to quaternion</param>
        /// <param name="maxDegreesDelta">the giving radian of angle between the from quaternion and the interpolation quaternion</param>
        /// <returns>quaternion</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static quaternion RotateTowards(quaternion from, quaternion to, number maxDegreesDelta)
        // {
        //     number angle = quaternion.Angle(from, to);
        //     if (angle.RawValue == 0L) return to;
        //     return quaternion.SlerpUnclamped(from, to, number.Min(number.one, maxDegreesDelta / angle));
        // }

        /// <summary>
        /// spherical interpolation of two quaternions
        /// </summary>
        /// <param name="q1">quaternion a</param>
        /// <param name="q2">quaternion b</param>
        /// <param name="t">coefficient</param>
        /// <returns>quaternion</returns>
        public static fpQuaternion SlerpUnclamped(fpQuaternion q1, fpQuaternion q2, fp t)
        {
            fp dt = Dot(q1, q2);
            if(dt < 0)
            {
                dt = -dt;
                q2.value = -q2.value;
            }

            if(dt < fp._0_9995)
            {
                var angle = acos(dt);
                var s = rsqrt(fp.one - dt * dt);    // 1 / sin(angle)
                var w1 = sin(angle * (fp.one - t)) * s;
                var w2 = sin(angle * t) * s;
                return fpQuaternion(q1.value * w1 + q2.value * w2);
            }
            else
            {
                return LerpUnclamped(q1, q2, t);
            }

            // if (number.Abs(dt).RawValue > number.ONE - number.SMALL_SQRT) return a;//a��b��Ȼ��෴����ʱ���ز�ֵ
            // number rad = number.Acos(number.Abs(dt));
            // number invSin = number.one / number.Sin(rad);
            // number inverse = number.Sin((number.one - t) * rad) * invSin;
            // number opposite = dt.RawValue < 0L ? -number.Sin(t * rad) * invSin : number.Sin(t * rad) * invSin;
            // return new quaternion(
            //     (inverse * a.x) + (opposite * b.x),
            //     (inverse * a.y) + (opposite * b.y),
            //     (inverse * a.z) + (opposite * b.z),
            //     (inverse * a.w) + (opposite * b.w));
        }

        // /// <summary>
        // /// get euler angles in radians of the giving quaternion
        // /// </summary>
        // /// <param name="rotation">the giving quaternion</param>
        // /// <returns>euler angles in radians</returns>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
        // public static float3 ToEulerAngles(quaternion rotation) => rotation.eulerAngles * number.Deg2Rad;

        public bool Equals(fpQuaternion x) { return value.x == x.value.x && value.y == x.value.y && value.z == x.value.z && value.w == x.value.w; }

        /// <summary>
        /// is this quaternion equal to the other object. 
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns>true if object is quaternion and equals to this, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is fpQuaternion qOther && this.Equals(qOther);

        /// <summary>
        /// get the hash code of this quaternion
        /// </summary>
        /// <returns>hash code</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => ((value.x.RawValue * 0x6E050B01u) + (value.y.RawValue * 0x750FDBF5u) +
            (value.z.RawValue * 0x7F3DD499u) + (value.w.RawValue * 0x52EAAEBBu)).GetHashCode();
            
        /// <summary>
        /// get the normalized quaternion of this one
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize() => this = fpQuaternion.Normalize(this);

        // /// <summary>
        // /// set x/y/z/w values
        // /// </summary>
        // /// <param name="newX">new value of X</param>
        // /// <param name="newY">new value of Y</param>
        // /// <param name="newZ">new value of Z</param>
        // /// <param name="newW">new value of W</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void Set(number newX, number newY, number newZ, number newW)
        // {
        //     x.RawValue = newX.RawValue;
        //     y.RawValue = newY.RawValue;
        //     z.RawValue = newZ.RawValue;
        //     w.RawValue = newW.RawValue;
        // }

        // /// <summary>
        // /// construct quaternion from axis and angle to this
        // /// </summary>
        // /// <param name="axis">the axis</param>
        // /// <param name="angle">the angle in radians</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
        // public void SetAxisAngle(float3 axis, number angle) => this = quaternion.AxisAngle(axis, angle);

        /// <summary>
        /// construct quaternion from euler angles to this.(ZXY)
        /// </summary>
        /// <param name="euler">euler angles in radians. z as roll, x as pitch, y as yaw</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public void SetEulerAngles(float3 euler) => this = quaternion.EulerAngles(euler);

        /// <summary>
        /// construct quaternion from euler angles to this. (ZXY)
        /// </summary>
        /// <param name="x">pitch angle in radians</param>
        /// <param name="y">yaw angle in radians</param>
        /// <param name="z">roll angle in radians</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public void SetEulerAngles(number x, number y, number z) => this = quaternion.EulerAngles(x, y, z);

        // /// <summary>
        // /// construct quaternion by euler angles to this, same as SetEulerAngles. (ZXY)
        // /// </summary>
        // /// <param name="euler">euler angles in radians. z as roll, x as pitch, y as yaw</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public void SetEulerRotation(float3 euler) => this = quaternion.EulerRotation(euler);

        // /// <summary>
        // /// construct quaternion from euler angles to this, same as SetEulerAngles. (ZXY)
        // /// </summary>
        // /// <param name="x">pitch angle in radians</param>
        // /// <param name="y">yaw angle in radians</param>
        // /// <param name="z">roll angle in radians</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // [Obsolete("Use quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        // public void SetEulerRotation(number x, number y, number z) => this = quaternion.EulerRotation(x, y, z);

        /// <summary>
        /// construct quaternion by rotating one vector to another vector, and set to this.
        /// </summary>
        /// <param name="fromDirection">the from vector</param>
        /// <param name="toDirection">the to vector</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void SetFromToRotation(float3 fromDirection, float3 toDirection) => this = quaternion.FromToRotation(fromDirection, toDirection);

        // /// <summary>
        // /// construct quaternion from forward vector and upwards vector, and set to this.  float3.up as default upwards vector.
        // /// </summary>
        // /// <param name="view">forward vector</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void SetLookRotation(fp3 view) => this = fpQuaternion.LookRotation(view);

        // /// <summary>
        // /// construct quaternion from forward vector and upwards vector, and set to this
        // /// </summary>
        // /// <param name="view">forward vector</param>
        // /// <param name="up">upwards vector</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public void SetLookRotation(fp3 view, fp3 up) => this = fpQuaternion.LookRotation(view, up);

        /// <summary>
        /// get a formatted string of the quaternion.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An object that specifies culture-specific formatting.</param>
        /// <returns>the formatted string</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            bool flag = string.IsNullOrEmpty(format);
            if (flag) format = "F5";
            return string.Format(formatProvider, "({0}, {1}, {2}, {3})", new object[]
            {
                value.x.ToString(format, formatProvider),
                value.y.ToString(format, formatProvider),
                value.z.ToString(format, formatProvider),
                value.w.ToString(format, formatProvider)
            });
        }

 
        /// <summary>
        /// get a formatted string of the quaternion.
        /// </summary>
        /// <returns>the formatted string</returns>
        public override string ToString() => this.ToString(null, CultureInfo.InvariantCulture.NumberFormat);

        /// <summary>
        /// is lhs quaternion equal to rhs quaternion. 
        /// </summary>
        /// <param name="lhs">lhs quaternion</param>
        /// <param name="rhs">rhs quaternion</param>
        /// <returns>true if equals, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(fpQuaternion lhs, fpQuaternion rhs) => fpQuaternion.IsEqualUsingDot(fpQuaternion.Dot(lhs, rhs));

        /// <summary>
        /// is lhs quaternion not equal to rhs quaternion. 
        /// </summary>
        /// <param name="lhs">lhs quaternion</param>
        /// <param name="rhs">rhs quaternion</param>
        /// <returns>true if not equals, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(fpQuaternion lhs, fpQuaternion rhs) => !(lhs == rhs);

        /// <summary>
        /// is two quaternions equals to each other by using dot.
        /// </summary>
        /// <param name="dot">the dot value</param>
        /// <returns>true if dot value equals to 1 within the accuracy, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEqualUsingDot(fp dot) => dot.RawValue >= fp.ONE - fp.SMALL_SQRT;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Unity.Mathematics.quaternion(fpQuaternion v) { return new Unity.Mathematics.quaternion(v.value); }
    }

    public static partial class fpMath
    {
          /// <summary>Returns a quaternion constructed from four float values.</summary>
        /// <param name="x">The x component of the quaternion.</param>
        /// <param name="y">The y component of the quaternion.</param>
        /// <param name="z">The z component of the quaternion.</param>
        /// <param name="w">The w component of the quaternion.</param>
        /// <returns>The quaternion constructed from individual components.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion fpQuaternion(fp x, fp y, fp z, fp w) { return new fpQuaternion(x, y, z, w); }

        /// <summary>Returns a fpQuaternion constructed from a float4 vector.</summary>
        /// <param name="value">The float4 containing the components of the fpQuaternion.</param>
        /// <returns>The fpQuaternion constructed from a float4.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion fpQuaternion(fp4 value) { return new fpQuaternion(value); }

        /// <summary>Returns a unit fpQuaternion constructed from a float3x3 rotation matrix. The matrix must be orthonormal.</summary>
        /// <param name="m">The float3x3 rotation matrix.</param>
        /// <returns>The fpQuaternion constructed from a float3x3 matrix.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion fpQuaternion(fp3x3 m) { return new fpQuaternion(m); }

        /// <summary>Returns the conjugate of a quaternion value.</summary>
       /// <param name="q">The quaternion to conjugate.</param>
       /// <returns>The conjugate of the input quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion conjugate(fpQuaternion q)
        {
            return fpQuaternion(q.value * fp4(-1, -1, -1, 1));
        }

        /// <summary>Returns the dot product of two quaternions.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp dot(fpQuaternion a, fpQuaternion b)
        {
            return Deterministics.Math.fpQuaternion.Dot(a, b);
        }

        /// <summary>Returns a normalized version of a quaternion q by scaling it by 1 / length(q).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion normalize(fpQuaternion q)
        {
            return Deterministics.Math.fpQuaternion.Normalize(q);
        }

        public static fpQuaternion slerp(fpQuaternion q1, fpQuaternion q2, fp t)
        {
            return Deterministics.Math.fpQuaternion.SlerpUnclamped(q1, q2, t);
        }

        /// <summary>Returns the result of a normalized linear interpolation between two quaternions q1 and a2 using an interpolation parameter t.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion nlerp(fpQuaternion q1, fpQuaternion q2, fp t)
        {
            return Deterministics.Math.fpQuaternion.LerpUnclamped(q1, q2, t);
        }

        /// <summary>Returns the result of transforming the quaternion b by the quaternion a.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion mul(fpQuaternion a, fpQuaternion b)
        {
            return fpQuaternion(a.value.wwww * b.value + (a.value.xyzx * b.value.wwwx + a.value.yzxy * b.value.zxyy) * fp4(1, 1, 1, -1) - a.value.zxyz * b.value.yzxz);
        }

        /// <summary>Returns the result of transforming a vector by a quaternion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 mul(fpQuaternion q, fp3 v)
        {
            var t = 2 * cross(q.value.xyz, v);
            return v + q.value.w * t + cross(q.value.xyz, t);
        }

        /// <summary>Returns the angle in radians between two unit quaternions.</summary>
        /// <param name="q1">The first quaternion.</param>
        /// <param name="q2">The second quaternion.</param>
        /// <returns>The angle between two unit quaternions.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp angle(fpQuaternion q1, fpQuaternion q2)
        {
            var diff = asin(length(normalize(mul(conjugate(q1), q2)).value.xyz));
            return diff + diff;
        }

        /// <summary>Returns the result of rotating a vector by a unit quaternion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 rotate(fpQuaternion q, fp3 v)
        {
            var t = 2 * cross(q.value.xyz, v);
            return v + q.value.w * t + cross(q.value.xyz, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fpQuaternion inverse(fpQuaternion q)
        {
            return Deterministics.Math.fpQuaternion.inverse(q);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static fp3 forward(fpQuaternion q) { return mul(q, fp3(0, 0, 1)); }  // for compatibility

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(fpQuaternion num2, Unity.Mathematics.quaternion b)
        {
            return Approximately(num2.value, b.value) ;
        }
    }
}
