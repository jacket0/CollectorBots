using System;
using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _oreOffset = 0.1f;
    private Ore _targetOre;
    private Transform _base;

    public bool IsIdle { get; private set; } = true;

    public event Action<Ore> OreDelivered;

    public void Initialize(Transform baseTransform)
    {
        _base = baseTransform;
    }

    public void Collect(Ore ore)
    {
        IsIdle = false;
        _targetOre = ore;
        _targetOre.Reserve();
        StartCoroutine(CollectRoutine());
    }

    private IEnumerator CollectRoutine()
    {
        Vector3 orePosition = _targetOre.transform.position;

        transform.LookAt(orePosition);
        yield return MoveTo(orePosition);

        _targetOre.transform.SetParent(transform);
        _targetOre.transform.localPosition = new Vector3(0, _oreOffset, 0);

        transform.LookAt(_base.position);
        yield return MoveTo(_base.position);

        Ore delivered = _targetOre;
        delivered.transform.SetParent(null);
        _targetOre = null;

        IsIdle = true;
        OreDelivered?.Invoke(delivered);
        delivered.Release();
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        target.y = transform.position.y;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

