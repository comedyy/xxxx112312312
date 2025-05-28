using System;
using System.Collections.Generic;
using Deterministics.Math;
using Unity.Entities;

public class BattleLogic
{
    World _world;

    public BattleLogic()
    {
    }

    public void Dispose()
    {
        _world.Dispose();
    }


    public void StartContinueBattle(string guid)
    {
        PlaybackController playbackController = new PlaybackController(guid, PlaybackMode.ReadPlayOnGoing | PlaybackMode.Write);
        Start(playbackController.Reader.GetBattleStartMessage(), BattleType.ContinueBattle, playbackController);
    }

    public void StartReplay(string guid)
    {
        PlaybackController playbackController = new PlaybackController(guid, PlaybackMode.ReadPlaybackZip);
        Start(playbackController.Reader.GetBattleStartMessage(), BattleType.Replay, playbackController);
    }

    public void StartBattle(BattleStartMessage battleStartMessage, BattleType battleType)
    {
        Start(battleStartMessage, battleType, new PlaybackController(battleStartMessage.guid, PlaybackMode.Write));
    }
    
    public void StartOnlineBattle(BattleStartMessage battleStartMessage)
    {
        Start(battleStartMessage, BattleType.OnlineBattle, new PlaybackController(battleStartMessage.guid, PlaybackMode.Write));
    }

    public void Start(BattleStartMessage battleStartMessage, BattleType battleType, PlaybackController playbackController)
    {
        // Add Controller 
        BattleControllerMgr.Instance.AddController(new BattleDataController(battleStartMessage));
        BattleControllerMgr.Instance.AddController(playbackController);
        BattleControllerMgr.Instance.AddController(new CheckSumMgr());

        // Initialize the battle logic here
        _world = new World("battleWorld");

        LocalFrame localFrame = new LocalFrame(battleType);

        var systemGroup = _world.GetOrCreateSystemManaged<BattleRootSystemGroup>();
        CreateSystemByList(_world, systemGroup, EcsSystemList.nodeDescriptorList);
        InjectSystem(localFrame);

        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);

        // Add User
        AddUser();
        _world.EntityManager.CreateSingleton(new ComFrameCount());
        _world.EntityManager.CreateSingleton(new ComGameState());
        _world.EntityManager.CreateSingleton(new ComRandomValue { random = new fpRandom(battleStartMessage.seed) });
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
        EntityArchetype archetype = entityManager.CreateArchetype(typeof(LComPosition), typeof(LComRotation), typeof(LComHp),  typeof(GameobjectrComponent), typeof(UserMoveSpeedComponet), typeof(VTransform), typeof(VLerpTransform));
        Entity entity = entityManager.CreateEntity(archetype);
        entityManager.SetComponentData(entity, new LComPosition { Value = new fp3(0, 0, 0) });
        entityManager.SetComponentData(entity, new LComRotation { Value = fpQuaternion.identity });
        entityManager.SetComponentData(entity, new LComHp { Value = 100 });
        entityManager.SetComponentData(entity, new UserMoveSpeedComponet { speed = 8 });
        entityManager.SetComponentData(entity, new GameobjectrComponent { gameObject = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube) });
    }

}