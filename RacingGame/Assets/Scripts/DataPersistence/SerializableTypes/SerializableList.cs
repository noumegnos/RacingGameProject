using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableList<T> : List<T>, ISerializationCallbackReceiver
{
    //public List<T> list;
    [SerializeField] private List<T> list = new List<T>();

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach (T item in this)
        {
            list.Add(item);
        }
    }
    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            this.Add(list[i]);
        }
    }
}
