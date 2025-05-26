using System;
using System.Collections.Generic;
using Deterministics.Math;
using Unity.Entities;

public class BattleLogic
{
    World _world;

    public void Dispose()
    {
        _world.Dispose();
    }

    public void Initialize(BattleStartMessage battleStartMessage)
    {
        // Initialize the battle logic here
        _world = new World("battleWorld");

        LocalFrame localFrame = new LocalFrame(0, BattleType.Client);

        var systemGroup = _world.GetOrCreateSystemManaged<BattleRootSystemGroup>();
        CreateSystemByList(_world, systemGroup, EcsSystemList.nodeDescriptorList);
        InjectSystem(localFrame);

        // // init
        // InitializationSystemGroup initializationSystemGroup = _world.GetOrCreateSystemManaged<InitializationSystemGroup>();
        // initializationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<ControllerUserSystem>());
        // initializationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<UpdateLocalFrameSystem>());

        // // simulation
        // SimulationSystemGroup simulationSystemGroup = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
        // var logicSystemGroup = _world.CreateSystemManaged<LogicUpdateSystemGroup>();
        // simulationSystemGroup.AddSystemToUpdateList(logicSystemGroup);
        // logicSystemGroup.Inject(localFrame);

        // var inputSystem = _world.CreateSystemManaged<InputUserSystem>();
        // inputSystem.fetchFrame = localFrame.syncFrameInputCache;
        // logicSystemGroup.AddSystemToUpdateList(inputSystem);

        // logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<PreRvoSystemGroup>());
        // logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<RvoSystemGroup>());
        // logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<AfterRvoSystemGroup>());

        // // logicSystemGroup.AddSystemToUpdateList(_world.CreateSystem<WritePlaybackDataSystem>());

        // // presentation
        // PresentationSystemGroup presentationSystemGroup = _world.GetOrCreateSystemManaged<PresentationSystemGroup>();
        // var unsortedPresentationSystemGroup = _world.CreateSystemManaged<UnsortedPresentationSystemGroup>();
        // presentationSystemGroup.AddSystemToUpdateList(unsortedPresentationSystemGroup);

        // unsortedPresentationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<VLerpTransformSystem>());
        // unsortedPresentationSystemGroup.AddSystemToUpdateList(_world.CreateSystem<DrawEntitySystem>());

        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);

        // Add User
        AddUser();
        _world.EntityManager.CreateSingleton(new ComFrameCount());
        _world.EntityManager.CreateSingleton(new ComGameState());
        _world.EntityManager.CreateSingleton(new ComRandomValue { random = new fpRandom(battleStartMessage.seed) });

        // Add Controller 
        BattleControllerMgr.Instance.AddController(new BattleDataController(battleStartMessage));
        BattleControllerMgr.Instance.AddController(new PlaybackController(battleStartMessage.guid, PlaybackMode.Write));
        BattleControllerMgr.Instance.AddController(new CheckSumMgr());
    }

    private void InjectSystem(LocalFrame localFrame)
    {
        var logicSystemGroup = _world.GetOrCreateSystemManaged<LogicUpdateSystemGroup>();
        logicSystemGroup.Inject(localFrame);

        var inputSystem = _world.GetOrCreateSystemManaged<InputUserSystem>();
        inputSystem.fetchFrame = localFrame.syncFrameInputCache;
    }

    private void CreateSystemByList(World world, ComponentSystemGroup systemGroup, List<EcsSystemNodeDescriptor> nodeDescriptorList)
    {
        foreach (var node in nodeDescriptorList)
        {
            if (node.rootType == null) continue;

            var system = world.GetOrCreateSystemManaged(node.rootType);
            if (system == null)
            {
                UnityEngine.Debug.LogError($"Failed to create system for type: {node.rootType}");
                continue;
            }

            if (systemGroup != null)
            {
                systemGroup.AddSystemToUpdateList(system);
            }

            if (node.list != null && node.list.Count > 0)
            {
                CreateSystemByList(world, (ComponentSystemGroup)system, node.list);
            }
        }
    }

    private void AddUser()
    {
        // Add user logic here
        // For example, you might want to create entities or set up components
        // Example: Create a new entity and add components to it
        EntityManager entityManager = _world.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(typeof(LComPosition), typeof(LComRotation), typeof(LComHp),  typeof(GameobjectrComponent), typeof(UserMoveSpeedComponet));
        Entity entity = entityManager.CreateEntity(archetype);
        entityManager.SetComponentData(entity, new LComPosition { Value = new fp3(0, 0, 0) });
        entityManager.SetComponentData(entity, new LComRotation { Value = fpQuaternion.identity });
        entityManager.SetComponentData(entity, new LComHp { Value = 100 });
        entityManager.SetComponentData(entity, new UserMoveSpeedComponet { speed = 8 });
        entityManager.SetComponentData(entity, new GameobjectrComponent { gameObject = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube) });
    }
}