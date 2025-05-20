using Game.Battle.CommonLib;
using Game.BattleShare.ECS.SystemGroup;

public partial class RvoSystemGroup : BaseUnsortSystemGroup
{
    string _guid;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        MSPathSystem.ShutdownCS(_guid);
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        _guid =  new System.Guid().ToString();
        MSPathSystem.InitSystemCS(fp._0_05, _guid);

        // Initialize the RVO system group here
        // For example, you can add systems to this group
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoMakeKdTreeSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoSyncPositionFromRvoSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoCustomAgentPropertySystemGroup>());
        AddSystemToUpdateList(World.GetOrCreateSystem<RvoDoStepSystem>());
    }
}