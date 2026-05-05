using System;
using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _resourceOffset = 0.1f;

    private Resource _targetResource;
    private Base _mainBase;

    public bool IsIdle { get; private set; } = true;

    public event Action<Resource> ResourceDelivered;
    public event Action<Bot> BecameIdle;

    public Base MainBase => _mainBase;

    public void TransferTo(Base newBase)
    {
        _mainBase = newBase;
    }

    public void Collect(Resource resource)
    {
        if (resource == null)
            return;

        if (!IsIdle)
            return;

        IsIdle = false;
        _targetResource = resource;
        StartCoroutine(CollectRoutine());
    }

    public void GoToFlag(Vector3 position, Action<Bot> onArrived)
    {
        if (!IsIdle)
            return;

        IsIdle = false;
        StartCoroutine(GoToFlagRoutine(position, onArrived));
    }

    private IEnumerator GoToFlagRoutine(Vector3 position, Action<Bot> onArrived)
    {
        transform.LookAt(position);
        yield return MoveTo(position);
        onArrived?.Invoke(this);
        IsIdle = true;
        BecameIdle?.Invoke(this);
    }

    private IEnumerator CollectRoutine()
    {
        Base homeBase = _mainBase;
        Resource targetResource = _targetResource;

        if (homeBase == null || targetResource == null)
        {
            IsIdle = true;
            BecameIdle?.Invoke(this);
            yield break;
        }

        Vector3 orePosition = targetResource.transform.position;

        transform.LookAt(orePosition);
        yield return MoveTo(orePosition);

        if (targetResource == null)
        {
            IsIdle = true;
            BecameIdle?.Invoke(this);
            yield break;
        }

        targetResource.transform.SetParent(transform);
        targetResource.transform.localPosition = new Vector3(0, _resourceOffset, 0);

        if (homeBase == null)
        {
            IsIdle = true;
            BecameIdle?.Invoke(this);
            yield break;
        }

        transform.LookAt(homeBase.transform.position);
        yield return MoveTo(homeBase.transform.position);

        if (targetResource == null)
        {
            IsIdle = true;
            BecameIdle?.Invoke(this);
            yield break;
        }

        Resource delivered = targetResource;
        delivered.transform.SetParent(null);
        _targetResource = null;

        IsIdle = true;
        ResourceDelivered?.Invoke(delivered);
        BecameIdle?.Invoke(this);
        delivered.Release();
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        target.y = transform.position.y;

        while ((transform.position - target).sqrMagnitude > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

