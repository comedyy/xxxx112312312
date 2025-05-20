using Game.BattleShare.ECS.SystemGroup;
using Unity.Entities;

public class BattleLogic
{
    World _world;

    public void Initialize()
    {
        // Initialize the battle logic here
        _world = new World("battleWorld");

        // Create the system groups
        SimulationSystemGroup simulationSystemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
        var fixedTimeSystemGroup = _world.CreateSystemManaged<FixedTimeSystemGroup>();
        simulationSystemGroup.AddSystemToUpdateList(fixedTimeSystemGroup);
        fixedTimeSystemGroup.AddSystemToUpdateList(_world.CreateSystem<PreRvoSystemGroup>());
        fixedTimeSystemGroup.AddSystemToUpdateList(_world.CreateSystem<RvoSystemGroup>());
        fixedTimeSystemGroup.AddSystemToUpdateList(_world.CreateSystem<AfterRvoSystemGroup>());

        PresentationSystemGroup presentationSystemGroup = _world.GetOrCreateSystemManaged<PresentationSystemGroup>();
        var unsortedPresentationSystemGroup = _world.CreateSystemManaged<UnsortedPresentationSystemGroup>();
        presentationSystemGroup.AddSystemToUpdateList(unsortedPresentationSystemGroup);
        unsortedPresentationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<DrawEntitySystem>());


        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
    }
}