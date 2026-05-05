using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceRepository))]
public class ResourceDispatcher : MonoBehaviour
{
    private ResourceRepository _repository;

    private readonly List<Base> _bases = new();
    private readonly Dictionary<Bot, Base> _botOwners = new();

    private void Awake()
    {
        _repository = GetComponent<ResourceRepository>();
    }

    private void OnEnable()
    {
        _repository.ResourceAdded += OnResourceAdded;
    }

    private void OnDisable()
    {
        _repository.ResourceAdded -= OnResourceAdded;
    }

    public void RegisterBase(Base baseToRegister)
    {
        if (baseToRegister == null || _bases.Contains(baseToRegister))
            return;

        _bases.Add(baseToRegister);

        baseToRegister.BotAdded += OnBotAdded;
        baseToRegister.BotRemoved += OnBotRemoved;

        foreach (Bot bot in baseToRegister.Bots)
        {
            if (bot == null)
                continue;

            RegisterBotOwner(bot, baseToRegister);
        }

        foreach (Bot bot in baseToRegister.Bots)
        {
            if (bot.IsIdle)
                TryDispatchForBot(bot);
        }
    }

    public void UnregisterBase(Base baseToUnregister)
    {
        if (baseToUnregister == null || !_bases.Remove(baseToUnregister))
            return;

        baseToUnregister.BotAdded -= OnBotAdded;
        baseToUnregister.BotRemoved -= OnBotRemoved;

        foreach (Bot bot in baseToUnregister.Bots)
        {
            if (bot == null)
                continue;

            bot.BecameIdle -= OnBotBecameIdle;
            _botOwners.Remove(bot);
        }
    }

    private void OnBotAdded(Bot bot)
    {
        if (bot == null)
            return;

        Base owner = bot.MainBase;

        if (owner != null)
            RegisterBotOwner(bot, owner);

        if (bot.IsIdle)
            TryDispatchForBot(bot);
    }

    private void OnBotRemoved(Bot bot)
    {
        if (bot == null)
            return;

        bot.BecameIdle -= OnBotBecameIdle;
        _botOwners.Remove(bot);
    }

    private void RegisterBotOwner(Bot bot, Base owner)
    {
        if (bot == null || owner == null)
            return;

        if (_botOwners.TryGetValue(bot, out Base currentOwner) && currentOwner == owner)
            return;

        _botOwners[bot] = owner;
        bot.BecameIdle -= OnBotBecameIdle;
        bot.BecameIdle += OnBotBecameIdle;
    }

    private void OnResourceAdded(Resource resource)
    {
        TryAssign(resource);
    }

    private void OnBotBecameIdle(Bot bot)
    {
        if (bot == null || _botOwners.ContainsKey(bot) == false)
            return;

        TryDispatchForBot(bot);
    }

    private void TryAssign(Resource resource)
    {
        if (resource == null)
            return;

        Base closestBase = FindBestBaseForResource(resource.transform.position);

        if (closestBase == null)
            return;

        Bot closestBot = closestBase.FindClosestIdleBot(resource.transform.position);

        if (closestBot == null)
            return;

        if (!_repository.TryTake(resource))
            return;

        closestBot.Collect(resource);
    }

    private void TryDispatchForBot(Bot bot)
    {
        if (bot == null)
            return;

        if (!_botOwners.TryGetValue(bot, out Base baseOwner) || baseOwner == null)
            return;

        Resource closestResource = FindClosestFreeResourceTo(bot.transform.position, baseOwner);

        if (closestResource == null)
            return;

        if (!_repository.TryTake(closestResource))
            return;

        bot.Collect(closestResource);
    }

    private Base FindBestBaseForResource(Vector3 position)
    {
        Base closest = null;
        float minDistance = float.MaxValue;

        foreach (Base baseItem in _bases)
        {
            if (baseItem == null)
                continue;

            if (baseItem.FindClosestIdleBot(position) == null)
                continue;

            float distance = (baseItem.transform.position - position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = baseItem;
            }
        }

        return closest;
    }

    private Resource FindClosestFreeResourceTo(Vector3 position, Base baseOwner)
    {
        Resource closest = null;
        float minDistance = float.MaxValue;

        foreach (Resource resource in _repository.FreeResources)
        {
            if (resource == null)
                continue;

            if (!IsBaseClosestToResource(baseOwner, resource.transform.position))
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

    private bool IsBaseClosestToResource(Base baseOwner, Vector3 resourcePosition)
    {
        if (baseOwner == null)
            return false;

        float ownerDistance = (baseOwner.transform.position - resourcePosition).sqrMagnitude;

        foreach (Base baseItem in _bases)
        {
            if (baseItem == null || baseItem == baseOwner)
                continue;

            float otherDistance = (baseItem.transform.position - resourcePosition).sqrMagnitude;

            if (otherDistance < ownerDistance)
                return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        _bases.Clear();
        _botOwners.Clear();
    }
}
