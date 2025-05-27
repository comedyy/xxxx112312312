using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Entities;
using System.Linq;

public enum CheckSumType
{
    BattleMessageHash,
    beforeGetPositionFromRvo,
    afterGetPositionFromRvo,
    preRvoPosition,
    pos,
    hp,
    randomValue,
    Input,
    Count,
}

public class CheckSumHistory
{
    public List<int> listDetail = new List<int>();
    public List<int> listOrder = new List<int>();
    public List<string> listParam = new List<string>();
    public int frame;
    public int hash;
    public int hashIndex;

    internal void Clear()
    {
        listDetail.Clear();
        listOrder.Clear();
        listParam.Clear();
        frame = 0;
        this.hashIndex = 0;
        hash = 0;
    }
}

public class CheckSum
{
    const int DetailCount = 64;
    public CheckSum(CheckSumType name){
        this.name = name;

    #if DEBUG_1
        for(int i = 0; i < DetailCount; i++)
        {
            _lstHistory.Add(new CheckSumHistory());
        }

        _current = _lstHistory.Last();
    #endif
    }
    private CheckSum(){}
    public CheckSumType name{get; private set;}
    int m_Frame;
    public int m_HashCode;

#if DEBUG_1
    CheckSumHistory _current;

    public List<CheckSumHistory> _lstHistory = new List<CheckSumHistory>();
#endif


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CombineHashCode(int seed, int hashcode)
    {
        seed ^= unchecked(hashcode + (int)0x9e3779b9 + (seed << 6) + (seed >> 2));
        return seed;
    }

    // release模式使用，关键数据即可。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CheckValue(Entity entity, int hash, params int[] tipValue)
    {
        var entityHash = (entity.Index << 16) + entity.Version;
        #if DEBUG_1
        if(MainNet.isCheckSumDetail)
        {
            _current.listDetail.Add(hash);
            _current.listDetail.AddRange(tipValue);
            _current.listOrder.Add(entityHash);
        }
        #endif

        m_HashCode = CombineHashCode(m_HashCode, entityHash);
        m_HashCode = CombineHashCode(m_HashCode, hash);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Conditional("DEBUG_1")]
    public void CheckValueInDebug(Entity entity, int hash, params int[] additional)
    {
        var entityHash = (entity.Index << 16) + entity.Version;
        #if DEBUG_1
        if(MainNet.isCheckSumDetail)
        {
            _current.listDetail.Add(hash);
            _current.listDetail.AddRange(additional);
            _current.listOrder.Add(entityHash);
        }
        #endif

        m_HashCode = CombineHashCode(m_HashCode, entityHash);
        m_HashCode = CombineHashCode(m_HashCode, hash);

        if(additional != null && additional.Length > 0)
        {
            foreach(var x in additional)
            {
                m_HashCode = CombineHashCode(m_HashCode, x);
            }
        }
    }


    public bool hasStringParam{get;set;}= false;
    [Conditional("DEBUG_1")]
    public void CheckValueWithParam(Entity entity, int value, string param, params int[] list)
    {
        var entityHash = (entity.Index << 16) + entity.Version;
        #if DEBUG_1
        if(MainNet.isCheckSumDetail)
        {
            hasStringParam = true;
            _current.listDetail.Add(value);
            _current.listDetail.AddRange(list);

            _current.listOrder.Add(entityHash);
            _current.listParam.Add(param);
        }
        #endif

        m_HashCode = CombineHashCode(m_HashCode, entityHash);
        m_HashCode = CombineHashCode(m_HashCode, value);

        if(list != null && list.Length > 0)
        {
            foreach(var x in list)
            {
                m_HashCode = CombineHashCode(m_HashCode, x);
            }
        }
    }

    [Conditional("DEBUG_1")]
    public void CheckValueWithStacktrace(Entity entity, int value,  params int[] list)
    {
        var entityHash = (entity.Index << 16) + entity.Version;
        #if DEBUG_1
        if(MainNet.isCheckSumDetail)
        {
            hasStringParam = true;
            _current.listDetail.Add(value);
            _current.listDetail.AddRange(list);

            _current.listOrder.Add(entityHash);
            _current.listParam.Add(UnityEngine.StackTraceUtility.ExtractStackTrace ());
        }
        #endif

        m_HashCode = CombineHashCode(m_HashCode, entityHash);
        m_HashCode = CombineHashCode(m_HashCode, value);

        if(list != null && list.Length > 0)
        {
            foreach(var x in list)
            {
                m_HashCode = CombineHashCode(m_HashCode, x);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCheckSum() => m_HashCode;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SaveCheckSum(int frameCount, int hashIndex)
    {
        totalHash = m_HashCode;
        totalFrame = frameCount;
        currentHashIndex = hashIndex;

        #if DEBUG_1
        _current.frame = frameCount;
        _current.hashIndex = hashIndex;
        _current.hash = totalHash;

        if(MainNet.isCheckSumDetail)
        {
            var x = _lstHistory[0];
            _lstHistory.RemoveAt(0);
            x.Clear();
            _lstHistory.Add(x);
            _current = x;
        }
        #endif
    }


    public int totalHash{get; private set;}
    public int totalFrame{get; private set;}

    public int currentHashIndex;


    public FrameHashItem GetHashItem(int checkHashIndex, int checkFrame)
    {
        var hashIndex = checkHashIndex == -1 ? currentHashIndex : checkHashIndex;

        var x = new FrameHashItem(){
            hashType = (byte)name,
        };

        #if DEBUG_1
        if(MainNet.isCheckSumDetail)
        {
            var diff = currentHashIndex - hashIndex;
            if(diff < 0)
            {
                UnityEngine.Debug.LogError($"diff < 0 {currentHashIndex} {hashIndex}");
                return default;
            }

            var Index = DetailCount - diff - 2;
            if(Index < 0) 
            {
                UnityEngine.Debug.LogError($"hashdetail已过期 {currentHashIndex} {hashIndex}");
                return default;
            }

            var item = _lstHistory[Index];
            x.listValue =  item.listDetail;
            x.listEntity = item.listOrder;
            x.lstParam = item.listParam;
            x.hash = item.hash;
            x.hashType = (byte)name;
            x.frame = item.frame;

            if(checkFrame != -1 && item.frame != checkFrame)
            {
                UnityEngine.Debug.LogError($"checkFrame error :{item.frame} != {checkFrame} name：{name}");
            }
            
            if(checkHashIndex != -1 && item.hashIndex != checkHashIndex)
            {
                UnityEngine.Debug.LogError($"checkHashIndex error :{item.hashIndex} != {checkHashIndex} name：{name}");
            }
        }
        #endif
        return x;
    }
}