using Game.BattleShare.ECS.SystemGroup;

public partial class RvoSystemGroup : BaseUnsortSystemGroup
{
    public RvoSystemGroup()
    {
        EnableSystemSorting = false;
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the RVO system group here
        // For example, you can add systems to this group
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoMakeKdTreeSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoSyncPositionFromRvoSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoChangeAgentVectorSystemGroup>());
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoDoStepSystem>());
    }

    protected override void OnUpdate()
    {
        UnityEngine.Debug.Log("RvoSystemGroup OnUpdate");
        base.OnUpdate();
    }
}