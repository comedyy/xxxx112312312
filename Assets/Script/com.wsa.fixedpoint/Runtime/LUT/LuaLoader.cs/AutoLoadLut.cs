using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

#if USE_BURST
namespace LutAutoLoad
{
    class AutoLoadReference
    {
        Action _disposeCallback;
        public AutoLoadReference(Action disposeCallback)
        {
            _disposeCallback = disposeCallback;
        }

        ~AutoLoadReference()
        {
            _disposeCallback();
        }
    }

    public class AutoLoadLutMono
    {
        public unsafe static SharedStatic<LutPointer<ushort>> sqrtLut = Deterministics.Math.fixlut.sqrtLut;
        public unsafe static SharedStatic<LutPointer<int>> asinLut = Deterministics.Math.fixlut.asinLut;
        public unsafe static SharedStatic<LutPointer<int>> sinLut = Deterministics.Math.fixlut.sinLut;
        public unsafe static SharedStatic<LutPointer<int>> sincosLut = Deterministics.Math.fixlut.sinCosLut;
        public unsafe static SharedStatic<LutPointer<int>> tanLut = Deterministics.Math.fixlut.tanLut;
        
        static AutoLoadReference autoLoadReference;

        static bool _isLoaded = false;

    #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
    #else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    #endif
        static void AutoLoad()
        {
            if(_isLoaded) return;
            autoLoadReference = new AutoLoadReference(Dispose);

            _isLoaded = true;
            LoadLutUshortIncrease("sqrt", sqrtLut);
            LoadLutIntIncrease("asin", asinLut);
            LoadLutInt("sin", sinLut);
            LoadLutInt("sincos", sincosLut);
            LoadLutInt("tan", tanLut);
        }

        private static void LoadLutUshortIncrease(string path, SharedStatic<LutPointer<ushort>> lut)
        {
            var x = Resources.Load<TextAsset>(path).bytes;
            MemoryStream s = new MemoryStream(x);
            BinaryReader reader = new BinaryReader(s);
            var size = reader.ReadInt32();
            lut.Data = new LutPointer<ushort>(size, Allocator.Persistent);
            ushort t = 0;
            for(int i = 0; i < size; i++)
            {
                ushort v = (ushort)(t + reader.ReadByte());
                lut.Data[i] = v;
                t = v;
            }

            // List<int> ints = new List<int>();
            // for(int i = 0; i < size; i++)
            // {
            //     ints.Add(lut.Data[i]);
            // }

            // File.WriteAllText($"d://{path}.txt", string.Join(",", ints));
        }

        
        private static void LoadLutIntIncrease(string path, SharedStatic<LutPointer<int>> lut)
        {
            var x = Resources.Load<TextAsset>(path).bytes;
            MemoryStream s = new MemoryStream(x);
            BinaryReader reader = new BinaryReader(s);
            var size = reader.ReadInt32();
            lut.Data = new LutPointer<int>(size, Allocator.Persistent);
            int t = 0;
            for(int i = 0; i < size; i++)
            {
                int v = (int)(t + reader.ReadByte());
                lut.Data[i] = v;
                t = v;
            }
            
            // List<int> ints = new List<int>();
            // for(int i = 0; i < size; i++)
            // {
            //     ints.Add(lut.Data[i]);
            // }

            // File.WriteAllText($"d://{path}.txt", string.Join(",", ints));
        }


        private static void LoadLutInt(string path, SharedStatic<LutPointer<int>> lut)
        {
            var x = Resources.Load<TextAsset>(path).bytes;
            MemoryStream s = new MemoryStream(x);
            BinaryReader reader = new BinaryReader(s);
            var size = reader.ReadInt32();
            lut.Data = new LutPointer<int>(size, Allocator.Persistent);
            for(int i = 0; i < size; i++)
            {
                lut.Data[i] = reader.ReadInt32();
            }
            
            // List<int> ints = new List<int>();
            // for(int i = 0; i < size; i++)
            // {
            //     ints.Add(lut.Data[i]);
            // }

            // File.WriteAllText($"d://{path}.txt", string.Join(",", ints));
        }

        static void Dispose()
        {
            if(!_isLoaded) return;

            _isLoaded = false;
            sqrtLut.Data.Dispose();
            asinLut.Data.Dispose();
            sinLut.Data.Dispose();
            sincosLut.Data.Dispose();
            tanLut.Data.Dispose();
        }
    }

}
#endif