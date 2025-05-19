using System;
using System.Collections;
using System.Collections.Generic;
using Game.Battle.CommonLib;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<int> allInts = new List<int>();
    List<Vector3> allPoint = new List<Vector3>();
    public InstanceDrawer instanceDrawer;
    public Transform targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        MSPathSystem.InitSystemCS(fp._0_05, new System.Guid().ToString());

        for (int i = 0; i < 1000; i++)
        {
            allInts.Add(MSPathSystem.AddAgentCS(0, i, 1, 3, 5, fp._0_05, fp._0_05, 1, 5, 1, 0, 0, i));
            allPoint.Add(new Vector3(i, 0, 1)); 
        }
    }

    // Update is called once per frame
    float _t = 0;
    int _frameCount;
    void Update()
    {
        _frameCount++;
        RvoStepUpdater.UpdateRVO(_frameCount);

        // 获取到每个对象的位置。
        GetPosFromRVO(allInts, allPoint);

        UpdateDirs();

        RvoStepUpdater.DoStepRVO(_frameCount);

        instanceDrawer.Update1(allPoint);

        // 其它逻辑
        if (allInts.Count > 0)
        {
            if (UnityEngine.Random.Range(0, 10) == 0)
            {
                MsPathEventController.AddRvoOpt(GameEventBattleRvoOpt.CreateEvent(GameEventBattleRvoOpt.Opt.RemoveAgent, 0, allInts[0], null));
            }

            if (UnityEngine.Random.Range(0, 10) == 0)
            {
                MsPathEventController.AddRvoOpt(GameEventBattleRvoOpt.CreateEvent(GameEventBattleRvoOpt.Opt.SetAgentRadius, 0, allInts[UnityEngine.Random.Range(0, allInts.Count)], new fp[] { 2 }));
            }

            if (UnityEngine.Random.Range(0, 10) == 0)
            {
                MsPathEventController.AddSetPosEvent(GameEventBattleRvoOptSetPos.CreateEvent(0, allInts[UnityEngine.Random.Range(0, allInts.Count)], UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100)));
            }
        }
    }

    private void UpdateDirs()
    {
        // for (int i = 0; i < allInts.Count; i++)
        // {
        //     var pos = allPoint[i];
        //     var dir = (targetTransform.position - new Vector3(pos.x, 0, pos.y)).normalized * 3;
        //     MSPathSystem.SetAgentVelocityPrefCS(0, allInts[i], (fp)dir.x, (fp)dir.z);
        // }
        foreach(var x in allInts)
        {
            var pos = MSPathSystem.GetAgentPositionCS(0, x);
            var dir = (targetTransform.position - new Vector3(pos.x, 0, pos.y)).normalized * 3;
            MSPathSystem.SetAgentVelocityPrefCS(0, x, (fp)dir.x, (fp)dir.z);
        }
    }

    private void GetPosFromRVO(List<int> allInts, List<Vector3> allPois)
    {
        for (int i = 0; i < allInts.Count; i++)
        {
            var pos = MSPathSystem.GetAgentPositionCS(0, allInts[i]);
            allPois[i] = new Vector3(pos.x, 0, pos.y);
        }
    }
}
