using System;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private LayerMask _oreLayer;

    private ResourceRepository _resourceRepository;

    public void Initialize(ResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public void Scan()
    {
        if (_resourceRepository == null)
            throw new NullReferenceException();

        Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius, _oreLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Resource resource))
                _resourceRepository.Pull(resource);
        }
    }
}
