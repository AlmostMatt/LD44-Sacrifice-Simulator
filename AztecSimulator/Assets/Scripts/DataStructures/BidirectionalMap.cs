using System.Collections.Generic;

// A collection that maintains a 1:1 relationship between keys and values.
// If a new key:value pair overlaps with an existing key or value, the existing entry will be removed.
public class BidirectionalMap<T1,T2>
{
    private Dictionary<T1, T2> byKey = new Dictionary<T1, T2>();
    private Dictionary<T2, T1> byValue = new Dictionary<T2, T1>();

    public int Count { get { return (byKey.Count); } }
    public Dictionary<T1,T2>.KeyCollection Keys { get { return (byKey.Keys); } }
    public Dictionary<T1,T2>.ValueCollection Values { get { return (byKey.Values); } }

    public void Add(T1 key, T2 value)
    {
        if (ContainsKey(key))
        {
            RemoveKey(key);
        }
        if (ContainsValue(value))
        {
            RemoveValue(value);
        }
        byKey[key] = value;
        byValue[value] = key;
    }

    public T2 GetValue(T1 key)
    {
        return byKey[key];
    }

    public T1 GetKey(T2 value)
    {
        return byValue[value];
    }

    public bool ContainsKey(T1 key)
    {
        return byKey.ContainsKey(key);
    }

    public bool ContainsValue(T2 value)
    {
        return byValue.ContainsKey(value);
    }

    public void RemoveKey(T1 key)
    {
        byValue.Remove(byKey[key]);
        byKey.Remove(key);
    }

    public void RemoveValue(T2 value)
    {
        byKey.Remove(byValue[value]);
        byValue.Remove(value);
    }
}
