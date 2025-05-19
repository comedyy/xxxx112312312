using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class InstanceDrawer : MonoBehaviour
{
    private ComputeBuffer m_ArgsBuffer;
    private ComputeBuffer m_ObjectPositionBuffer;
    public Material m_Material;
    public Mesh m_Mesh;
    private readonly uint[] r_IndirectArgs = { 0, 0, 0, 0, 0 };
    private MaterialPropertyBlock m_MaterialPropertyBlock;

    private void Awake() {
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
    }

    public void Update1(List<Vector3> vector3s)
    {
        if(m_ObjectPositionBuffer == null)
        {
            Init();
        }

        int count = vector3s.Count;
        if (count == 0) return;

        if (m_Mesh == null || m_Material == null) return;

        UnityEngine.Profiling.Profiler.BeginSample("normal.CommandBuffer.SetData");
        NativeArray<float3> array = new Unity.Collections.NativeArray<float3>(vector3s.Count, Allocator.Temp);
        for (int i = 0; i < count; i++)
        {
            array[i] = vector3s[i];
        }
        
        m_ObjectPositionBuffer.SetData(array, 0, 0, count);

        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("normal.CommandBuffer.SetData1");
        r_IndirectArgs[1] = (uint)count;
        m_ArgsBuffer.SetData(r_IndirectArgs);
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Graphics.DrawMeshInstancedIndirect");
        Graphics.DrawMeshInstancedIndirect(m_Mesh, 0, m_Material, new Bounds(UnityEngine.Vector3.zero, 10000 * UnityEngine.Vector3.one),
            m_ArgsBuffer, 0, m_MaterialPropertyBlock, UnityEngine.Rendering.ShadowCastingMode.Off, true, 0, Camera.main);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void Init()
    {
        m_ObjectPositionBuffer = new ComputeBuffer(1024, 12);
        m_ArgsBuffer = new ComputeBuffer(r_IndirectArgs.Length, sizeof(uint), ComputeBufferType.IndirectArguments);
        r_IndirectArgs[0] = m_Mesh.GetIndexCount(0);
        m_Material.SetBuffer("objectPositionBuffer", m_ObjectPositionBuffer);
    }
}
