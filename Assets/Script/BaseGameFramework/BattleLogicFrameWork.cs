using System;
using System.Collections.Generic;
using Deterministics.Math;
using Unity.Entities;

public class BattleLogicFrameWork
{
    World _world;

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

    public void StartSingleClientBattle(BattleStartMessage battleStartMessage, BattleType battleType)
    {
        Start(battleStartMessage, battleType, new PlaybackController(battleStartMessage.guid, PlaybackMode.Write), battleStartMessage.joins[0].UserId);
    }
    
    public void StartOnlineBattle(BattleStartMessage battleStartMessage, int controllerId)
    {
        Start(battleStartMessage, BattleType.OnlineBattle, new PlaybackController(battleStartMessage.guid, PlaybackMode.Write), controllerId);
    }

    public void Start(BattleStartMessage battleStartMessage, BattleType battleType, PlaybackController playbackController, int controllerId = 0)
    {
        // Add Controller 
        BattleControllerMgr.Instance.AddController(new BattleDataController(battleStartMessage));
        BattleControllerMgr.Instance.AddController(playbackController);
        BattleControllerMgr.Instance.AddController(new CheckSumMgr());

        // Initialize the battle logic here
        _world = new World("battleWorld");

        LocalFrame localFrame = new LocalFrame(controllerId, battleType);

        var systemGroup = _world.GetOrCreateSystemManaged<BattleRootSystemGroup>();
        CreateSystemByList(_world, systemGroup, EcsSystemList.nodeDescriptorList);
        InjectSystem(localFrame);

        World.DefaultGameObjectInjectionWorld = _world;
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);

        // Add User
        AddUser(battleStartMessage, controllerId);
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

    private void AddUser(BattleStartMessage battleStartMessage, int idController)
    {
        EntityManager entityManager = _world.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(typeof(LComPosition),
                            typeof(LComRotation), typeof(LComHp), typeof(GameobjectComponent), typeof(UserMoveSpeedComponet), typeof(VTransform), typeof(VLerpTransform), typeof(ComHeroId));
        var entityComUser = entityManager.CreateSingletonBuffer<BufferUserEntity>();
        var dynamicBuffer = entityManager.GetBuffer<BufferUserEntity>(entityComUser);
        Entity selfEntity = default;

        foreach (var x in battleStartMessage.joins)
        {
            Entity entity = entityManager.CreateEntity(archetype);
            entityManager.SetComponentData(entity, new LComPosition { Value = new fp3(0, 0, 0) });
            entityManager.SetComponentData(entity, new LComRotation { Value = fpQuaternion.identity });
            entityManager.SetComponentData(entity, new LComHp { Value = 100 });
            entityManager.SetComponentData(entity, new ComHeroId { id = x.UserId });
            entityManager.SetComponentData(entity, new UserMoveSpeedComponet { speed = 8 });
            entityManager.SetComponentData(entity, new GameobjectComponent { gameObject = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube) });

            dynamicBuffer.Add(new BufferUserEntity() { id = x.UserId, entity = entity });

            if (idController == x.UserId)
            {
                selfEntity = entity;
            }
        }

        BattleControllerMgr.Instance.AddController(new SelfUserController() {  selfControllerId = idController, _selfEntity = selfEntity});
    }

}