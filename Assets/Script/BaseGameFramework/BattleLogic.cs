using System;
using Deterministics.Math;
using Game.BattleShare.ECS.SystemGroup;
using Unity.Entities;

public class BattleLogic
{
    World _world;

    public void Initialize()
    {
        // Initialize the battle logic here
        _world = new World("battleWorld");

        LocalFrame localFrame = new LocalFrame(0, BattleType.Client);

        // init
        InitializationSystemGroup initializationSystemGroup = _world.GetOrCreateSystemManaged<InitializationSystemGroup>();
        initializationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<ControllerUserSystem>());
        initializationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<UpdateLocalFrameSystem>());

        // simulation
        SimulationSystemGroup simulationSystemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
        var logicSystemGroup = _world.CreateSystemManaged<LogicUpdateSystemGroup>();
        simulationSystemGroup.AddSystemToUpdateList(logicSystemGroup);
        logicSystemGroup.Inject(localFrame);

        var inputSystem = _world.CreateSystemManaged<InputUserSystem>();
        inputSystem.fetchFrame = localFrame.syncFrameInputCache;
        
        logicSystemGroup.AddSystemToUpdateList(inputSystem);
        logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<PreRvoSystemGroup>());
        logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<RvoSystemGroup>());
        logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<AfterRvoSystemGroup>());

        // presentation
        PresentationSystemGroup presentationSystemGroup = _world.GetOrCreateSystemManaged<PresentationSystemGroup>();
        var unsortedPresentationSystemGroup = _world.CreateSystemManaged<UnsortedPresentationSystemGroup>();
        presentationSystemGroup.AddSystemToUpdateList(unsortedPresentationSystemGroup);

        unsortedPresentationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<VLerpTransformSystem>());
        unsortedPresentationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<DrawEntitySystem>());

        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);

        // Add User
        AddUser();
        World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ComFrameCount));
        World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ComGameState));
        var randomEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ComRandom));
        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(randomEntity, new ComRandom { random = new fpRandom(1000) });
    }

    private void AddUser()
    {
        // Add user logic here
        // For example, you might want to create entities or set up components
        // Example: Create a new entity and add components to it
        EntityManager entityManager = _world.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(typeof(LTransform), typeof(GameobjectrComponent), typeof(UserMoveSpeedComponet));
        Entity entity = entityManager.CreateEntity(archetype);
        entityManager.SetComponentData(entity, new LTransform { position = new fp3(0, 0, 0) });
        entityManager.SetComponentData(entity, new UserMoveSpeedComponet { speed = 8 });
        entityManager.SetComponentData(entity, new GameobjectrComponent { gameObject = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube) });
    }
}