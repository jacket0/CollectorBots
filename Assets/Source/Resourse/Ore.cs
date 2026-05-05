using System;
using UnityEngine;

public class Resource : MonoBehaviour, IReleasable<Resource>
{
    [SerializeField] private Material _material;

    public event Action<Resource> Released;

    public void Release()
    {
        Released?.Invoke(this);
    }
}
