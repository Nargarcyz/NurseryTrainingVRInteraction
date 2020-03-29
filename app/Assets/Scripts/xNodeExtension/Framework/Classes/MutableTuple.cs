using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MutableTuple<T1, T2> // IComparable, IStructuralEquatable, IStructuralComparable
{
    [SerializeField]
    public T1 Item1 { get; set; }
    [SerializeField]
    public T2 Item2 { get; set; }

    public MutableTuple(T1 item1, T2 item2)
    {
        this.Item1 = item1;
        this.Item2 = item2;
    }
    
    
}
