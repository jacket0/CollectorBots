using System;
using UnityEngine;

public class Ore : MonoBehaviour, IReleasable<Ore>
{
    [SerializeField] private Material _material;

    public event Action<Ore> Released;

    public void Release()
    {
        Released?.Invoke(this);
    }
}
