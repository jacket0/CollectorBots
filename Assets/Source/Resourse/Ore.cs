using System;
using UnityEngine;

public class Ore : MonoBehaviour, IReleasable<Ore>
{
    [SerializeField] private Material _material;

    private Collider _collider;

    public bool IsReserved { get; private set; }

    public event Action<Ore> Released;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void Reserve()
    {
        IsReserved = true;
        _collider.enabled = false;
    }

    public void Release()
    {
        IsReserved = false;
        _collider.enabled = true;
        Released?.Invoke(this);
    }
}
