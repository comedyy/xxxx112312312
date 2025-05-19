
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

#if USE_BURST
namespace LutAutoLoad
{
    public struct LutPointer<T> : IDisposable where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        internal unsafe T* lut;
        private int m_Length;
        private Allocator m_AllocatorLabel;

        public unsafe T this[int index]
        {
            get => lut[index];
            set
            {
                lut[index] = value;
            }
        }
        public int Length
        {
            get => this.m_Length;
        }
 
        public unsafe LutPointer(int length, Allocator type)
        {
            long totalSize = (long)UnsafeUtility.SizeOf<T>() * length;
            this.m_AllocatorLabel = type;
            this.m_Length = length;
            this.lut = (T*)UnsafeUtility.Malloc(totalSize, UnsafeUtility.AlignOf<T>(), m_AllocatorLabel);
            UnsafeUtility.MemClear(this.lut, totalSize);
        }

        [WriteAccessRequired]
        public unsafe void Dispose()
        {
            if (this.lut == null) throw new ObjectDisposedException("The LutPointer is already disposed.");
            if (this.m_AllocatorLabel == Allocator.Invalid)
                throw new InvalidOperationException("The LutPointer can not be Disposed because it was not allocated with a valid allocator.");
            if (this.m_AllocatorLabel > Allocator.None)
            {
                UnsafeUtility.Free(this.lut, this.m_AllocatorLabel);
                this.m_AllocatorLabel = Allocator.Invalid;
            }
            this.lut = null;
            this.m_Length = 0;
        }
    }
}

#endif