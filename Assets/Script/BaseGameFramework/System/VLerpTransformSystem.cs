using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class VLerpTransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var frameCount = SystemAPI.GetSingleton<ComFrameCount>();
        var isLogicFrame = frameCount.frameUnity == UnityEngine.Time.frameCount;
        var deltaTime = UnityEngine.Time.deltaTime;
        var timeInterval = ComFrameCount.DELTA_TIME;

        Entities.ForEach((ref LTransform lTransform, ref VLerpTransform vLerp, ref VTransform vTransform) =>
        {
            vLerp.lerpTime += deltaTime;
            var percent = math.clamp(vLerp.lerpTime / timeInterval, 0, 1);
            vTransform.position = math.lerp(vLerp.pre.position, vLerp.target.position, percent);
            vTransform.quaternion = math.nlerp(vLerp.pre.quaternion, vLerp.target.quaternion, percent);

            if (isLogicFrame)
            {
                vLerp.pre = vTransform;
                vLerp.target = new VTransform() { position = lTransform.position, quaternion = lTransform.quaternion };
                vLerp.lerpTime = 0;
            }
        }).Run();
    }
}
