using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRepository : MonoBehaviour
{
    private readonly List<Resource> _freeResources = new();
    private readonly List<Resource> _busyResources = new();

    public IReadOnlyList<Resource> FreeResources => _freeResources;

    public event Action<Resource> ResourceAdded;

    public void Pull(Resource resource)
    {
        if (resource == null)
            throw new NullReferenceException();

        if (_freeResources.Contains(resource) || _busyResources.Contains(resource))
            return;

        _freeResources.Add(resource);
        ResourceAdded?.Invoke(resource);
    }

    public bool TryTake(Resource resource)
    {
        if (resource == null)
            throw new NullReferenceException();

        if (!_freeResources.Remove(resource))
            return false;

        _busyResources.Add(resource);

        return true;
    }

    public void Remove(Resource resource)
    {
        if (resource == null)
            throw new NullReferenceException();

        _freeResources.Remove(resource);
        _busyResources.Remove(resource);
    }
}
