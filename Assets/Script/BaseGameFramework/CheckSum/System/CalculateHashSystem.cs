using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using Deterministics.Math;
using LiteNetLib.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// 计算hash的system
public partial class CalculateHashSystem : SystemBase
{
    EntityQuery _queryTransform;
    NativeArray<int> _tempNativeArray;
    NetDataWriter _netDataWriter;

    protected override void OnCreate()
    {
        base.OnCreate();

        _queryTransform = GetEntityQuery(ComponentType.ReadOnly<LComPosition>(), ComponentType.ReadOnly<LComRotation>(), ComponentType.ReadOnly<LComHp>());
        _tempNativeArray = new NativeArray<int>(2, Allocator.Persistent);
        _netDataWriter = new NetDataWriter();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _tempNativeArray.Dispose();
    }

    protected override void OnUpdate()
    {
        var frameCount = SystemAPI.GetSingleton<ComFrameCount>().frameLogic;
        if (!NeedCalHash(frameCount, true, out var hashIndex))
        {
            return;
        }

        var checkSum = BattleControllerMgr.Instance.GetController<CheckSumMgr>();

        // write msg hash
        if (frameCount == 1)
        {
            var checkSumMsg = checkSum.GetCheckSum(CheckSumType.BattleMessageHash);
            _netDataWriter.Reset();
            var message = BattleControllerMgr.Instance.GetController<BattleDataController>().battleStartMessage;
            _netDataWriter.Put(message);
            ReadOnlySpan<byte> dataSpan = _netDataWriter.Data.AsSpan(0, _netDataWriter.Length);
            checkSumMsg.CheckValue(Entity.Null, (int)XxHash32.HashToUInt32(dataSpan));
        }

        // check input
        var inputCache = InputUserSystem._fetchInputList;
        if (inputCache.Count > 0)
        {
            _netDataWriter.Reset();
            var inputCheckSum = checkSum.GetCheckSum(CheckSumType.Input);
            foreach (var input in inputCache)
            {
                _netDataWriter.Put(input);
            }

            ReadOnlySpan<byte> dataSpan = _netDataWriter.Data.AsSpan(0, _netDataWriter.Length);
            var hash = CheckSum.CombineHashCode((int)XxHash32.HashToUInt32(dataSpan), inputCache.Count);
            inputCheckSum.CheckValue(Entity.Null, hash);
        }

        var positionChecksum = checkSum.GetCheckSum(CheckSumType.pos);
        var hpCheckSum = checkSum.GetCheckSum(CheckSumType.hp);
        
        CalMonsterUserHash(positionChecksum);

#if DEBUG_1
        Entities
        .WithoutBurst()
        .ForEach((Entity entity, ref MonsterHealthComponent health)=>{
            hpCheckSum.CheckValue(entity, math.asint(health.NowHp));
        }).Run();
#else
        CalHpHash(_tempNativeArray, hpCheckSum, _queryTransform);
#endif

#if DEBUG_1
        var skillDamangeSum = checkSum.GetCheckSum(CheckSumType.SkillDamageAttrib);
        Entities.WithoutBurst().ForEach((Entity entity, ref BattleShare.ECS.Component.Skill.SkillCommon.SkillDamageComponent x)=>{
            skillDamangeSum.CheckValue(entity, math.asint(x.AtkRate), math.asint(x.DamageRate));
        }).Run();
#endif

        // random
        checkSum.GetCheckSum(CheckSumType.randomValue).CheckValue(Entity.Null, (int)SystemAPI.GetSingleton<ComRandomValue>().random.state);

        foreach (var x in checkSum._allHashList)
        {
            x.SaveCheckSum(frameCount, hashIndex);
        }
    }

#if !DEBUG_1
    void CalMonsterUserHash(CheckSum checkSum)
    {
        CalTranslationHash(_tempNativeArray, checkSum, _queryTransform);
    }

    private void CalTranslationHash(NativeArray<int> temp, CheckSum checkSum, EntityQuery queryAllDrop)
    {
        temp[0] = 0;
        temp[1] = 0;

        var translateType = GetComponentTypeHandle<LComPosition>();
        var rotationType = GetComponentTypeHandle<LComRotation>();
        var entityType = GetEntityTypeHandle();
        var transformHashJob = new TransformHashJob()
        {
            TranslationType = translateType,
            RotationType = rotationType,
            EntityChunkType = entityType,
            hashArray = temp,
        };

        transformHashJob.Run(queryAllDrop);

        checkSum.m_HashCode = CombineHashCode(temp[0], temp[1]);
    }
    private void CalHpHash(NativeArray<int> temp, CheckSum dropCheckSum, EntityQuery queryAllDrop)
    {
        temp[0] = 0;
        temp[1] = 0;

        var hpType = GetComponentTypeHandle<LComHp>();
        var entityType = GetEntityTypeHandle();
        var transformHashJob = new MonsterHpHashJob()
        {
            HealthType = hpType,
            EntityChunkType = entityType,
            hashArray = temp,
        };

        transformHashJob.Run(queryAllDrop);

        dropCheckSum.m_HashCode = CombineHashCode(temp[0], temp[1]);
    }

    [BurstCompile]
    struct TransformHashJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<LComPosition> TranslationType;
        [ReadOnly] public ComponentTypeHandle<LComRotation> RotationType;
        [ReadOnly] public EntityTypeHandle EntityChunkType;

        public NativeArray<int> hashArray;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in Unity.Burst.Intrinsics.v128 chunkEnabledMask)
        {
            var positoinArray = chunk.GetNativeArray(ref TranslationType);
            var rotationArray = chunk.GetNativeArray(ref RotationType);
            var entityArray = chunk.GetNativeArray(EntityChunkType);

            var posHash = hashArray[0];
            var entityHash = hashArray[1];
            for (int i = 0; i < chunk.Count; i++)
            {
                var pos = positoinArray[i];
                var rotaiton = rotationArray[i];
                var entity = entityArray[i];

                posHash = CombineHashCode(posHash, pos.Value.GetHashCode());
                posHash = CombineHashCode(posHash, rotaiton.Value.GetHashCode());
                entityHash = CombineHashCode(entityHash, (entity.Index << 16) + entity.Version);
            }

            hashArray[0] = posHash;
            hashArray[1] = entityHash;
        }
    }

    [BurstCompile]
    struct MonsterHpHashJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<LComHp> HealthType;
        [ReadOnly] public EntityTypeHandle EntityChunkType;

        public NativeArray<int> hashArray;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in Unity.Burst.Intrinsics.v128 chunkEnabledMask)
        {
            var healthArray = chunk.GetNativeArray(ref HealthType);
            var entityArray = chunk.GetNativeArray(EntityChunkType);

            var posHash = hashArray[0];
            var entityHash = hashArray[1];
            for (int i = 0; i < chunk.Count; i++)
            {
                var healthComponent = healthArray[i];
                var entity = entityArray[i];

                posHash = CombineHashCode(posHash, healthComponent.Value.GetHashCode());
                entityHash = CombineHashCode(entityHash, (entity.Index << 16) + entity.Version);
            }

            hashArray[0] = posHash;
            hashArray[1] = entityHash;
        }
    }
#endif

    static Dictionary<Type, int> _dicHash = new Dictionary<Type, int>();
    public static int GetTypeName(Type type)
    {
        if (_dicHash.TryGetValue(type, out var hash))
        {
            return hash;
        }

        var s = type.ToString();
        hash = GetHash(s);

        _dicHash.Add(type, hash);

        return hash;
    }

    public static int GetHash(string s)
    {
        var result = 99999999;
        foreach (var item in s)
        {
            result = CombineHashCode(result, (int)item);
        }

        return result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CombineHashCode(int seed, int hashcode)
    {
        seed ^= unchecked(hashcode + (int)0x9e3779b9 + (seed << 6) + (seed >> 2));
        return seed;
    }

    public static bool NeedCalHash(int frameCount, bool needHashIndex, out int hashIndex)
    {
        hashIndex = 0;

#if DEBUG_1 || DEBUG_2
        hashIndex = frameCount;
        return true;
#else

        var isOk = false;
        isOk |= frameCount < 100;
        isOk |= frameCount % 100 == 0;
        

        hashIndex = frameCount <= 100 ? frameCount : 99 + frameCount / 100 ;

        return isOk;
        #endif
    }
}
