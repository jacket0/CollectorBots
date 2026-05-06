using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRepository : MonoBehaviour
{
    private readonly List<Resource> _freeResources = new();
    private readonly List<Resource> _busyResources = new();

    private readonly List<Base> _bases = new();
    private readonly Dictionary<Bot, Base> _botOwners = new();

    public IReadOnlyList<Resource> FreeResources => _freeResources;

    public event Action<Resource> ResourceAdded;

    public void RegisterBase(Base baseToRegister)
    {
        if (baseToRegister == null || _bases.Contains(baseToRegister))
            return;

        _bases.Add(baseToRegister);

        baseToRegister.BotAdded += OnBotAdded;
        baseToRegister.BotRemoved += OnBotRemoved;

        foreach (Bot bot in baseToRegister.Bots)
            RegisterBot(bot, baseToRegister);
    }

    public void UnregisterBase(Base baseToUnregister)
    {
        if (baseToUnregister == null || !_bases.Remove(baseToUnregister))
            return;

        baseToUnregister.BotAdded -= OnBotAdded;
        baseToUnregister.BotRemoved -= OnBotRemoved;

        foreach (Bot bot in baseToUnregister.Bots)
            UnregisterBot(bot);
    }

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

    private void OnEnable()
    {
        ResourceAdded += OnResourceAdded;
    }

    private void OnDisable()
    {
        ResourceAdded -= OnResourceAdded;
    }

    private void OnResourceAdded(Resource resource)
    {
        TryAssignResource(resource);
    }

    private void OnBotAdded(Bot bot)
    {
        if (bot == null)
            return;

        Base owner = FindBaseFor(bot);

        if (owner != null)
            RegisterBot(bot, owner);

        if (bot.IsIdle)
            TryDispatchBot(bot);
    }

    private void OnBotRemoved(Bot bot)
    {
        UnregisterBot(bot);
    }

    private void OnBotBecameIdle(Bot bot)
    {
        if (bot != null)
            TryDispatchBot(bot);
    }

    private void RegisterBot(Bot bot, Base owner)
    {
        if (bot == null || owner == null)
            return;

        if (_botOwners.TryGetValue(bot, out Base current) && current == owner)
            return;

        _botOwners[bot] = owner;
        bot.BecameIdle -= OnBotBecameIdle;
        bot.BecameIdle += OnBotBecameIdle;
    }

    private void UnregisterBot(Bot bot)
    {
        if (bot == null)
            return;

        bot.BecameIdle -= OnBotBecameIdle;
        _botOwners.Remove(bot);
    }

    private void TryAssignResource(Resource resource)
    {
        if (resource == null)
            return;

        Base bestBase = FindBestBaseForResource(resource.transform.position);

        if (bestBase == null)
            return;

        Bot bot = bestBase.FindClosestIdleBot(resource.transform.position);

        if (bot == null)
            return;

        if (!TryTake(resource))
            return;

        bot.Collect(resource);
    }

    private void TryDispatchBot(Bot bot)
    {
        if (bot == null)
            return;

        if (!_botOwners.TryGetValue(bot, out Base owner) || owner == null)
            return;

        Resource resource = FindClosestFreeResource(bot.transform.position, owner);

        if (resource == null)
            return;

        if (!TryTake(resource))
            return;

        bot.Collect(resource);
    }

    private Base FindBestBaseForResource(Vector3 position)
    {
        Base best = null;
        float minDistance = float.MaxValue;

        foreach (Base baseItem in _bases)
        {
            if (baseItem == null || baseItem.FindClosestIdleBot(position) == null)
                continue;

            float distance = (baseItem.transform.position - position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                best = baseItem;
            }
        }

        return best;
    }

    private Resource FindClosestFreeResource(Vector3 position, Base owner)
    {
        Resource closest = null;
        float minDistance = float.MaxValue;

        foreach (Resource resource in _freeResources)
        {
            if (resource == null)
                continue;

            if (!IsClosestBaseTo(owner, resource.transform.position))
                continue;

            float distance = (resource.transform.position - position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = resource;
            }
        }

        return closest;
    }

    private bool IsClosestBaseTo(Base owner, Vector3 resourcePosition)
    {
        float ownerDist = (owner.transform.position - resourcePosition).sqrMagnitude;

        foreach (Base baseItem in _bases)
        {
            if (baseItem == null || baseItem == owner)
                continue;

            if ((baseItem.transform.position - resourcePosition).sqrMagnitude < ownerDist)
                return false;
        }

        return true;
    }

    private Base FindBaseFor(Bot bot)
    {
        foreach (Base baseItem in _bases)
        {
            foreach (Bot b in baseItem.Bots)
            {
                if (b == bot)
                    return baseItem;
            }
        }

        return null;
    }
}
