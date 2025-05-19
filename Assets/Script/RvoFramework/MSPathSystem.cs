
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.Profiling;
using Deterministics.Math;

namespace Game.Battle.CommonLib
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AgentVector2
    {
        public fp x;
        public fp y;
    }

    public enum RVOMonsterType // 跟rvo保持一致
    {
        NormalGround = 0,
        NormalFly = 1,
        SpecialGround = 2,
		IgnoreAllFly = 3,
		IgnoreAllGround = 4, 
        Hero = 5,
    };

    public class MSPathSystem
    {
        public static bool IsValid { get; internal set; }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        const string kFindPathDllName = "RVO_PathFind";

#elif (UNITY_IOS || UNITY_IPHONE)
        const string kFindPathDllName = "__Internal";
#else
        const string kFindPathDllName = "libProject";
#endif

        static MsPathThread _msPathThread;
      
#region extern function
        /// <summary>
        /// 处理障碍物
        ///   Note:  初始化时添加(AddObstacle)完所有障碍物之后进行调用
        /// </summary>
        [DllImport(kFindPathDllName)]
        public static extern void ProcessObstacles();


        /// <summary>
        /// 添加Agent
        /// </summary>
        /// <param name="type"> 0: 地面   1: 飞行 </param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="neighborDist"></param>
        /// <param name="maxNeighbors"></param>
        /// <param name="timeHorizon"></param>
        /// <param name="timeHorizonObst"></param>
        /// <param name="radius"></param>
        /// <param name="maxSpeed"> </param>
        /// <param name="velocityX"> 初始x加速度, 默认都为0. </param>
        /// <param name="velocityY"> 初始z加速度, 默认都为0. </param>
        /// <returns></returns>
        [DllImport(kFindPathDllName)]
        static extern int AddAgent(int type, long posX, long posY, long neighborDist, int maxNeighbors, long timeHorizon, 
            long timeHorizonObst, long radius, long maxSpeed, long mass, long velocityX, long velocityY, long csIdentity);
        
        /// <summary>
        /// 添加障碍物.
        /// </summary>
        /// <param name="points"> [x, z] [x, z]</param>
        /// <param name="count"> 数组长度. </param>
        /// <returns></returns>
        [DllImport(kFindPathDllName)]
        static extern int AddObstacle(long[] points, int length);

        /// <summary>
        /// 移除Agent
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        [DllImport(kFindPathDllName)]
        static extern void RemoveAgent(int type, int agentIndex);


        /// <summary>
        /// 设置单个Agent最大速度.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="maxSpeed"></param>
        [DllImport(kFindPathDllName)]
        static extern void SetAgentMaxSpeed(int type, int agentIndex, long maxSpeed);
        /// <summary>
        /// 获取Agent最大速度
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <returns></returns>
        [DllImport(kFindPathDllName)]
        static extern long GetAgentMaxSpeed(int type, int agentIndex);


        /// <summary>
        /// 设置Agent半径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="radius"></param>
        [DllImport(kFindPathDllName)]
        static extern void SetAgentRadius(int type, int agentIndex, long radius);

        /// <summary>
        /// 获取Agent位置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="position"></param>
        [DllImport(kFindPathDllName)]
        static extern void GetAgentPosition(int type, int agentIndex, ref AgentVector2 position);

        /// <summary>
        /// 设置Agent位置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [DllImport(kFindPathDllName)]
        public static extern void SetAgentPosition(int type, int agentIndex, long x, long y);

        /// <summary>
        /// 设置Agent加速度
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [DllImport(kFindPathDllName)]
        static extern void SetAgentVelocityPref(int type, int agentIndex, long x, long y);
  

        /// <summary>
        /// 设置Agent 质量,
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="mass"></param>
        [DllImport(kFindPathDllName)]
        static extern void SetAgentMass(int type, int agentIndex, long mass);

        /// <summary>
        /// 用来查询当前位置的单位数量
        /// </summary>
        [DllImport(kFindPathDllName)]
        public static extern long GetNearestAgent(long x, long y);


        /// <summary>
        /// 用来查询当前位置的单位数量
        /// </summary>
        [DllImport(kFindPathDllName)]
        unsafe private static extern int GetNearByAgents(long x, long y, void* ptr, int size, long range);

                /// <summary>
        /// 用来查询当前位置的单位数量
        /// </summary>
        [DllImport(kFindPathDllName)]
        unsafe private static extern int GetNeibourUses(int type, int id, void* ptr);

        /// <summary>
        /// 获取位置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="position"></param>
        [DllImport(kFindPathDllName)]
        static extern void CalVelocity(long x, long y, long xDir, long yDir, ref AgentVector2 position);

        /// <summary>
        /// 设置最大的neighbor距离
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentIndex"></param>
        /// <param name="position"></param>
        [DllImport(kFindPathDllName)]
        public static extern void setAgentMaxNeighbors(int type, int agentIndex, int neighborCount);
        
        public static long ToFix16(fp v)
        {
            return v.RawValue;
        }

        public static fp Tofp(long p)
        {
            return fp.FromRaw(p);
        }

        [DllImport(kFindPathDllName)]
        static extern int getAgentCount();
        
        [DllImport(kFindPathDllName)]
        static extern void ClearObstacle();

        #endregion

        // ------------------------------------------------------------------
        #region CS Call
        public static AgentVector2 GetAgentPositionCS(int type, int agentIndex)
        {
            #if DEBUG_1
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("MSPathSystem.IsInThreadRunning in running");
            }
            #endif

            AgentVector2 position = new AgentVector2();
            GetAgentPosition(type, agentIndex, ref position);
            return position;
        }

        public static void SetAgentMaxSpeed(int type, int agentIndex, fp maxSpeed)
        {
#if DEBUG_1
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("MSPathSystem.IsInThreadRunning in running");
            }
#endif
            SetAgentMaxSpeed(type, agentIndex, ToFix16(maxSpeed));
        }

        public static fp GetAgentMaxSpeedCS(int type, int agentIndex)
        {
            return Tofp(GetAgentMaxSpeed(type, agentIndex));
        }

        public static void SetAgentVelocityPrefCS(int type, int agentIndex, fp x, fp y)
        {
            #if DEBUG_1
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("MSPathSystem.IsInThreadRunning in running");
            }
            #endif

            SetAgentVelocityPref(type, agentIndex, ToFix16(x), ToFix16(y));
        }
        
        public static void SetAgentMassCS(int type, int agentIndex, fp mass)
        {
            #if DEBUG_1
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("MSPathSystem.IsInThreadRunning in running");
            }
            #endif

            SetAgentMass(type, agentIndex, ToFix16(mass));
        }

        public unsafe static int GetNeibourUsesCS(int type, int agentIndex, Unity.Collections.NativeArray<long> nativeByteArray)
        {
            void* ptr = Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr(nativeByteArray);
            return GetNeibourUses(type, agentIndex, ptr);
        }

        public static void SetAgentPositionCS(int type, int agentIndex, fp x, fp y)
        {
            #if DEBUG_1
            if(MSPathSystem.IsInThreadRunning)
            {
                _rvoOptCheckSum.CheckValue(Entity.Null, 97);
                throw new Exception("MSPathSystem.IsInThreadRunning in running");
            }
            #endif

            SetAgentPosition(type, agentIndex, ToFix16(x), ToFix16(y));
        }

        public static void WaitStepEnd()
        {
            _msPathThread?.WaitStepEnd();
        }

        public static bool IsInThreadRunning => _msPathThread != null && _msPathThread.IsInRunningState;

        static string _guidBattle;
        public static void InitSystemCS(fp deltaTime, string guid)
        {
            if(IsValid)
            {
                ShutdownCS(null);
            }
            
            _guidBattle = guid;
            IsValid = true;
            _msPathThread = new MsPathThread();
            _msPathThread.Init(ToFix16(deltaTime), guid);

            s_obstacleCount = 0;

            #if DEBUG_1
            _rvoOptCheckSum.CheckValue(Entity.Null, 99);
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("OnProcessPos in running");
            }
            #endif
        }

        public static void ProcessObstaclesCS()
        {
            if(!IsValid) return;

#if DEBUG_1
            _rvoOptCheckSum.CheckValue(Entity.Null, 5);
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("OnProcessPos in running");
            }
#endif
            Profiler.BeginSample("ProcessObstacle");
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            ProcessObstacles();
            Profiler.EndSample();
            // UnityEngine.Debug.LogError($"processObstacle [{s_obstacleCount}] {1000f *watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency}");
        }

        
        public static void ClearObstacleCS()
        {
            if(IsValid)
            {
#if DEBUG_1
                _rvoOptCheckSum.CheckValue(Entity.Null, 3);
                if(MSPathSystem.IsInThreadRunning)
                {
                    throw new Exception("OnProcessPos in running");
                }
#endif
                Profiler.BeginSample("ClearObstacle");
                ClearObstacle();
                Profiler.EndSample();

                s_obstacleCount = 0;
                // UnityEngine.Debug.LogError($"ClearObstacle");
            }
        }


        public static void ShutdownCS(string guid)
        {
            if(guid != null && guid != _guidBattle)
            {
                UnityEngine.Debug.LogError($"shutdown mspathSystem error {guid}, current {_guidBattle}" );
                return;
            }

            if(!IsValid)
            {
                return;
            }

            IsValid = false;
            _msPathThread.Close();
            _msPathThread = null;
        }

        
        public static int AddAgentCS(int type, fp posX, fp posY, fp neighborDist, int maxNeighbors, fp timeHorizon, 
            fp timeHorizonObst, fp radius, fp maxSpeed, fp mass, fp velocityX, fp velocityY, long csIdentity)
        {
            #if DEBUG_1
            _rvoOptCheckSum.CheckValue(Entity.Null, 1);
            _rvoOptCheckSum.CheckValue(Entity.Null, type);
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(posX));
            _rvoOptCheckSum.CheckValue(Entity.Null,  fpMath.asint(posY));
            _rvoOptCheckSum.CheckValue(Entity.Null, maxNeighbors);
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(timeHorizon));
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(timeHorizonObst));
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(radius));
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(maxSpeed));
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(mass));
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(velocityX));
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(velocityY));

            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("OnProcessPos in running");
            }
            #endif
            return AddAgent(type, ToFix16(posX), ToFix16(posY), ToFix16(neighborDist), maxNeighbors, ToFix16(timeHorizon), ToFix16(timeHorizonObst), ToFix16(radius), ToFix16(maxSpeed), ToFix16(mass), ToFix16(velocityX), ToFix16(velocityY), csIdentity);
        }


        static int s_obstacleCount = 0;
        public static long[] obstaclesTemp = new long[256];     // 传递数据用的
        public static void AddObstacleCS(fp[] points)
        {
            if(!IsValid)
            {
                return;
            }

            #if DEBUG_1
            _rvoOptCheckSum.CheckValue(Entity.Null, 4);
            for(int i = 0;i < points.Length; i++)
            {
             _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(points[i]));
            }
            _rvoOptCheckSum.CheckValueWithStacktrace(Entity.Null, points.Length);

            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("OnProcessPos in running");
            }
            #endif
            
            Profiler.BeginSample("AddObstacle");
            for(int i = 0; i < points.Length; i++)
            {
                obstaclesTemp[i] = ToFix16(points[i]);
            }

            AddObstacle(obstaclesTemp, points.Length);
            Profiler.EndSample();

            s_obstacleCount += points.Length;
            // UnityEngine.Debug.LogError($"AddObstacle {length}");
        }

        
        public static void RemoveAgentCS(int type, int agentIndex)
        {
            #if DEBUG_1
            _rvoOptCheckSum.CheckValue(Entity.Null, 2);
            _rvoOptCheckSum.CheckValue(Entity.Null, type);
            _rvoOptCheckSum.CheckValue(Entity.Null, agentIndex);
            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("OnProcessPos in running");
            }
            #endif
            RemoveAgent(type, agentIndex);
        }

        public static void SetAgentRadiusCS(int type, int agentIndex, fp radius)
        {
            #if DEBUG_1
            _rvoOptCheckSum.CheckValue(Entity.Null, 6);
            _rvoOptCheckSum.CheckValue(Entity.Null, type);
            _rvoOptCheckSum.CheckValue(Entity.Null, agentIndex);
            _rvoOptCheckSum.CheckValue(Entity.Null, fpMath.asint(radius));

            if(MSPathSystem.IsInThreadRunning)
            {
                throw new Exception("OnProcessPos in running");
            }
            #endif
            SetAgentRadius(type, agentIndex, ToFix16(radius));
        }


        /// <summary>
        /// 主循环Update中进行调用
        /// </summary>
        public static void DoStep()
        {
            _msPathThread?.DoStep();
        }
        
        public unsafe static int GetNearByAgents(fp x, fp y, Unity.Collections.NativeArray<long> nativeByteArray, fp searchRange)
        {
            void* ptr = Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr(nativeByteArray);
            return GetNearByAgents(ToFix16(x), ToFix16(y), ptr, nativeByteArray.Length, ToFix16(searchRange));
        }
        
        public unsafe static int GetNearByAgents(fp x, fp y, Unity.Collections.NativeArray<long> nativeArray, fp searchRange, int length)
        {
            void* ptr = Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr(nativeArray);
            return GetNearByAgents(ToFix16(x), ToFix16(y), ptr, length, ToFix16(searchRange));
        }

        public static void CalVelocityCS(fp x, fp y, fp xDir, fp yDir, ref AgentVector2 position)
        {
            if(!IsValid) return;

            if(_msPathThread.IsInRunningState)
            {
                position.x = 0;
                position.y = 0;
            }
            else
            {
                CalVelocity(ToFix16(x), ToFix16(y), ToFix16(xDir), ToFix16(yDir), ref position);
            }
        }

        internal static int GetAgentCount()
        {
            if(!IsValid) return 0;

            return getAgentCount();
        }

        internal static void MakeKDTree()
        {
            if(!IsValid) return;

            _msPathThread.MakeKDTree();
        }
        #endregion
    }
}