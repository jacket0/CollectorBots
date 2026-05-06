using System;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private LayerMask _oreLayer;

    private Action<Resource> ResourceFound;

    public void Initialize(Action<Resource> onResourceFound)
    {
        ResourceFound = onResourceFound;
    }

    public void Scan()
    {
        if (ResourceFound == null)
            throw new InvalidOperationException($"{nameof(ResourceScanner)} не инициализирован.");

        Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius, _oreLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Resource resource))
                ResourceFound(resource);
        }
    }
}
