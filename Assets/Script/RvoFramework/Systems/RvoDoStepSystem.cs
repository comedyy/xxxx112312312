using Game.Battle.CommonLib;
using Unity.Entities;

public partial class RvoDoStepSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the system here if needed
    }

    protected override void OnUpdate()
    {
        RvoStepUpdater.DoStepRVO();
    }
    
}