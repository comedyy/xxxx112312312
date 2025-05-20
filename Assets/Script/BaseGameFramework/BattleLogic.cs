using Game.BattleShare.ECS.SystemGroup;
using Unity.Entities;

public class BattleLogic
{
    World _world;

    public void Initialize()
    {
        // Initialize the battle logic here
        _world = new World("battleWorld");

        InitializationSystemGroup initializationSystemGroup = _world.GetOrCreateSystemManaged<InitializationSystemGroup>();
        initializationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<UpdateLocalFrameSystem>());

        LocalFrame localFrame = new LocalFrame(fp._0_05, 0, BattleType.Client);

        // Create the system groups
        SimulationSystemGroup simulationSystemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
        var logicSystemGroup = _world.CreateSystemManaged<LogicUpdateSystemGroup>();
        simulationSystemGroup.AddSystemToUpdateList(logicSystemGroup);
        logicSystemGroup.Inject(localFrame);

        logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<PreRvoSystemGroup>());
        logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<RvoSystemGroup>());
        logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<AfterRvoSystemGroup>());

        PresentationSystemGroup presentationSystemGroup = _world.GetOrCreateSystemManaged<PresentationSystemGroup>();
        var unsortedPresentationSystemGroup = _world.CreateSystemManaged<UnsortedPresentationSystemGroup>();
        presentationSystemGroup.AddSystemToUpdateList(unsortedPresentationSystemGroup);
        unsortedPresentationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<DrawEntitySystem>());


        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
    }
}