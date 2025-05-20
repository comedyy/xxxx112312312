using Game.Battle.CommonLib;
using Unity.Entities;

public partial class RvoMakeKdTreeSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the system here if needed
    }

    protected override void OnUpdate()
    {
        RvoStepUpdater.UpdateRVO();
    }
    
}