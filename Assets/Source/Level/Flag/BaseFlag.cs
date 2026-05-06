using System;
using UnityEngine;

public class BaseFlag : MonoBehaviour, IReleasable<BaseFlag>
{
    public event Action<BaseFlag> Released;

    public void Release()
    {
        Released?.Invoke(this);
    }
}
