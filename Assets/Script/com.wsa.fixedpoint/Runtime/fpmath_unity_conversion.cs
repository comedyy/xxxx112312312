#if !UNITY_DOTSPLAYER
using UnityEngine;

#pragma warning disable 0660, 0661

namespace Deterministics.Math
{
    public partial struct fp2
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(fp2 v)     { return new Vector2(v.x, v.y); }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator fp2(Vector2 v)     { return new fp2((fp)v.x, (fp)v.y); }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator fp2(Unity.Mathematics.float2 v)     { return new fp2((fp)v.x, (fp)v.y); }
    }

    public partial struct fp3
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(fp3 v)     { return new Vector3(v.x, v.y, v.z); }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator  fp3(Vector3 v)     { return new fp3((fp)v.x, (fp)v.y, (fp)v.z); }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator fp3(Unity.Mathematics.float3 v) { return new fp3((fp)v.x, (fp)v.y, (fp)v.z); }
    }

    public partial struct fpQuaternion
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Quaternion(fpQuaternion q)  { return new Quaternion(q.value.x, q.value.y, q.value.z, q.value.w); }

        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator fpQuaternion(Unity.Mathematics.quaternion v) { 
            return new fpQuaternion(new fp4((fp)v.value.x, (fp)v.value.y, (fp)v.value.z, (fp)v.value.w)); 
        }
    }

    public partial struct fpRect
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static implicit operator Rect(fpRect rect)  { return new Rect(rect.x, rect.y, rect.width, rect.height); }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static explicit operator fpRect(Rect v) { 
            return new fpRect((fp2)v.min, (fp2)v.size); 
        }
    }
}
#endif
