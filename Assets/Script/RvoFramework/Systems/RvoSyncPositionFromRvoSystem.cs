using Deterministics.Math;
using Game.Battle.CommonLib;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

public partial class RvoSyncPositionFromRvoSystem : SystemBase
{
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        // Initialize the system here if needed

        _entityQuery = GetEntityQuery(ComponentType.ReadOnly<RvoComponent>(), ComponentType.ReadOnly<LTransformComponent>());
    }

    protected override void OnUpdate()
    {
        GetPositionFromRvoJob job = new GetPositionFromRvoJob(){
            TranslationChunkType = GetComponentTypeHandle<LTransformComponent>(),
            AgentChunkType = GetComponentTypeHandle<RvoComponent>(),
            Min = default, Max = default, ignoreRVOSize = default
        };
        job.ScheduleParallel(_entityQuery, Dependency).Complete();
    }

    
    [BurstCompile]
    struct GetPositionFromRvoJob : IJobChunk
    {
        public ComponentTypeHandle<LTransformComponent> TranslationChunkType;
        public ComponentTypeHandle<RvoComponent> AgentChunkType;
        [ReadOnly] public fp2 Min;
        [ReadOnly] public fp2 Max;
        [ReadOnly] public bool ignoreRVOSize;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var translationArray = chunk.GetNativeArray(ref TranslationChunkType);
            var agentTypeArray = chunk.GetNativeArray(ref AgentChunkType);

            for (int i = 0; i < agentTypeArray.Length; i++)
            {
                var nowTranslation = translationArray[i];
                var nowAgent = agentTypeArray[i];

                AgentVector2 agentPos1 = MSPathSystem.GetAgentPositionCS(nowAgent.AgentType, nowAgent.AgentId);
                var pos = new fp3(agentPos1.x, 0, agentPos1.y);
                nowTranslation.position = pos;

                translationArray[i] = nowTranslation;

                // if(!ignoreRVOSize)
                // {
                //     var inside = DropRange.ContainsVector3(pos, Min, Max);
                //     int neighborCount = inside ? nowAgent.RvoNeighborCount : 1;
                //     MSPathSystem.setAgentMaxNeighbors(nowAgent.AgentType, nowAgent.AgentIndex, neighborCount);
                // }
            }
        }
    }
}