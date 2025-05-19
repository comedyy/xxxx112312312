
using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.Profiling;

enum ThreadRunningState
{
    NoRunning,
    Running,
    Closing,
    Closed
}

public class MsPathThread
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        const string kFindPathDllName = "RVO_PathFind";

#elif (UNITY_IOS || UNITY_IPHONE)
        const string kFindPathDllName = "__Internal";
#else
        const string kFindPathDllName = "libProject";
#endif

    public bool IsInRunningState => _currentFinishedFrame < _targetFrame;

    Thread _runningThread;
    ThreadRunningState _runningState = ThreadRunningState.NoRunning;
    string _guid = "";
    const bool simulateInMainThread = false;

    public void Init(long deltaTime, string guid)
    {
        _guid = guid;
        InitSystem(deltaTime);

        _runningState = ThreadRunningState.Running;
        ThreadStart threadStart = new ThreadStart(DoStepThread);
        _runningThread = new Thread(threadStart)
        {
            Priority = ThreadPriority.BelowNormal
        };
        _runningThread.Start();
    }

    int _currentFinishedFrame = 0;
    int _targetFrame = 0;
    int _rvoFrame = 0;
    private void DoStepThread()
    {
        Profiler.BeginThreadProfiling("MSPathSystem", "Running");
        while(_runningState == ThreadRunningState.Running)
        {
            if(_currentFinishedFrame == _targetFrame) 
            {
                Thread.Sleep(1);
                continue;
            }

            SimulateRVO(false);

            _currentFinishedFrame = _currentFinishedFrame + 1;
        }

        Profiler.EndThreadProfiling();
        Shutdown();

        _runningThread = null;
        _runningState = ThreadRunningState.Closed;
    }

    public void Close()
    {
        _runningState = ThreadRunningState.Closing;

        UnityEngine.Profiling.Profiler.BeginSample("WaitStepClose");
        while(_runningState != ThreadRunningState.Closed)
        {
            Thread.Sleep(1);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void WaitStepEnd()
    {
        UnityEngine.Profiling.Profiler.BeginSample("WaitStepEnd");
        while(IsInRunningState)
        {
            Thread.Sleep(1);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void DoStep()
    {
        WaitStepEnd();
        _targetFrame++;

        if(_targetFrame > _rvoFrame)
        {
            UnityEngine.Debug.LogError("RVO frame less than target frame");
        }

        SimulateRVO(true);

        var value = getOverflowValue();
        if(value != 0)
        {
            UnityEngine.Debug.LogError($"rvo overflow found : {value}  battleGUID :${_guid}");
        }
    }

    void SimulateRVO(bool mainThread)
    {
        if(mainThread != simulateInMainThread) return;

        Profiler.BeginSample("computeNeighbour");
        computeNeighbour();
        Profiler.EndSample();
        Profiler.BeginSample("doStepVelocity");
        doStepVelocity();
        Profiler.EndSample();
        Profiler.BeginSample("DoStepUpdate");
        DoStepUpdate();
        Profiler.EndSample();
    }

    /// <summary>
    /// 主循环Update中进行调用
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern void computeNeighbour();
        /// <summary>
    /// 主循环Update中进行调用
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern void doStepVelocity();
    /// <summary>
    /// 主循环Update中进行调用
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern void DoStepUpdate();

    /// <summary>
    /// 主循环Update中进行调用
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern void DoStepBuildTree();

    /// <summary>
    /// 关闭系统,每个关卡战斗结束后进行调用
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern void Shutdown();
    /// <summary>
    ///  初始化
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern void InitSystem(long deltaTime);

    /// <summary>
    ///  初始化
    /// </summary>
    [DllImport(kFindPathDllName)]
    static extern long getOverflowValue();

    internal void MakeKDTree()
    {
        _rvoFrame++;
        UnityEngine.Profiling.Profiler.BeginSample("DoStepBuildTree");
        DoStepBuildTree();
        UnityEngine.Profiling.Profiler.EndSample();
    }
}