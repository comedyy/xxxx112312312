using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Battle.CommonLib
{
    // 外部驱动的。为了防止误用，这里创建一个类来演示
    // ---游戏逻辑---- -| rvo step 阻塞完成 |设置RVO的一些事件 | KDTree 更新 | 逻辑从kdtree寻找范围 |从rvo更新数据到逻辑 | 通过逻辑更新RVO的方向 | step rvo 其它线程开始 | ----- 游戏逻辑---
    public class RvoStepUpdater
    {
        // 最好在逻辑的最开始调用
        public static void UpdateRVO()
        {
            MSPathSystem.WaitStepEnd();

            MsPathEventController.InputEvent();

            MSPathSystem.MakeKDTree();
        }

        // 最好在逻辑的最后调用
        public static void DoStepRVO()
        {
            MSPathSystem.DoStep();
        }
    }
}
