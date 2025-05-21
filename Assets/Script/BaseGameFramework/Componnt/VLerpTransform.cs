using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct VLerpTransform : IComponentData
{
    public VTransform pre;
    public VTransform target;
    public float lerpTime;
}
