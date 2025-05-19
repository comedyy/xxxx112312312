using Unity.Entities;

public class BattleLogic
{
    World _world;

    public void Initialize()
    {
        // Initialize the battle logic here
        _world = new World("battleWorld");

        // Create the system groups
        var rvoSystemGroup = _world.CreateSystem<RvoSystemGroup>();
        SimulationSystemGroup simulationSystemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
        simulationSystemGroup.AddSystemToUpdateList(rvoSystemGroup);

        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
    }
}