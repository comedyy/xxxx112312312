using System;
using System.Collections.Generic;

public interface IObjectPoolItem
{
    void OnRelease();
}

public interface IObjectPoolCounter
{
    int countAll { get; }
    int countActive { get; }
    int countInactive { get; }
}

/// <summary>
/// ObjectPool
/// 对象池
/// </summary>
public class ObjectPool<T> : IObjectPoolCounter, IDisposable where T : new() 
{
    private Stack<T> m_Stack = new Stack<T>();
    private Action<T> m_ActionOnGet;
    private Action<T> m_ActionOnRelease;
    private readonly int m_MaximumRetained;

    public int countAll { get; private set; }
    public int countActive { get; private set; }
    public int countInactive { get { return m_Stack.Count; } }

    public ObjectPool(int maximumRetained = -1)
    {
        m_MaximumRetained = maximumRetained;
    }

    public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease, int maximumRetained = -1)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
        m_MaximumRetained = maximumRetained;
    }        

    public T Get()
    {
        T element;
        countActive++;
        if (m_Stack.Count == 0)
        {
            element = new T();
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
        {
            UnityEngine.Debug.LogError($"Internal error. Trying to destroy object that is already released to pool. element = {element}");
            return;
        }

        countActive--;

        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        
        if (element is IObjectPoolItem item)
            item.OnRelease();
        
        if (m_MaximumRetained == -1 || m_Stack.Count < m_MaximumRetained)
            m_Stack.Push(element);
    }

    public void Dispose()
    {
        m_Stack?.Clear();
    }
}