
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

public class CheckSumMgr : IBattleController
{
    public CheckSum[] _allHashList;
    public FrameHashItem[] _allFrameItemList;

    public CheckSumMgr()
    {
        _allHashList = new CheckSum[(int)CheckSumType.Count];
        for (int i = 0; i < (int)CheckSumType.Count; i++)
        {
            CheckSumType type = (CheckSumType)i;
            CheckSum checkSum = new CheckSum(type);
            _allHashList[i] = checkSum;
        }

        _allFrameItemList = new FrameHashItem[_allHashList.Length];
    }

    public CheckSum GetCheckSum(CheckSumType type)
    {
        return _allHashList[(int)type];
    }

    public CheckSum[] GetAllCheckSums()
    {
        return _allHashList;
    }
    
    public FrameHashItem[] CalFrameHashItems(int hashIndex, int frame) 
    {
        // if(MainNet.isCheckSumDetail)
        {
            for (int i = 0; i < _allFrameItemList.Length; i++)
            {
                _allFrameItemList[i] = _allHashList[i].GetHashItem(hashIndex, frame);
            };

            return _allFrameItemList;
        }
        
        return null;
    }

    public int GetResultHash() 
    {
        int hash = 0;
        for (int i = 0; i < _allHashList.Length; i++) 
        {
            hash = CheckSum.CombineHashCode(hash, _allHashList[i].totalHash);
        }
        return hash;
    }

    public int GetValueHash() 
    {
        return GetResultHash();
    }

    public int GetEntityHash() 
    {
        return GetResultHash();
    }

#if DEBUG_1
    internal MessageHash GetMessageHash()
    {
        return new MessageHash() { hash = GetResultHash(), allHashDetails = CalFrameHashItems(-1, -1), escaped = (int)World.DefaultGameObjectInjectionWorld.Time.ElapsedTimeLogic.RawValue, appendDetail = MainNet.isCheckSumDetail };
    }
#endif

    internal MessageHash GetMessageHash(int hashIndex, ushort frame)
    {
        var x =  new MessageHash(){hash = 0, allHashDetails = CalFrameHashItems(hashIndex, frame), escaped = 0, appendDetail = true, frame = frame};
        return x;
    }
}