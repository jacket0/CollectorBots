using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceScanner))]
[RequireComponent(typeof(BaseEconomy))]
public class Base : MonoBehaviour, IDeliveryPoint
{
    [Header("Сканирование")]
    [SerializeField] private float _scanInterval;

    private ResourceScanner _scanner;
    private ResourceRepository _resourceRepository;
    private BaseEconomy _economy;

    private readonly List<Bot> _bots = new();

    public IReadOnlyList<Bot> Bots => _bots;
    public Vector3 Position => transform.position;

    public event Action<Bot> BotAdded;
    public event Action<Bot> BotRemoved;

    private void Awake()
    {
        _scanner = GetComponent<ResourceScanner>();
        _economy = GetComponent<BaseEconomy>();
    }

    private void Start()
    {
        StartCoroutine(ScanRoutine());
    }

    public void Initialize(ResourceRepository repository)
    {
        _resourceRepository = repository;
        _scanner.Initialize(_resourceRepository.Pull);
    }

    public void PlaceFlag(Vector3 position)
    {
        _economy.PlaceFlag(position);
    }

    public Bot FindClosestIdleBot(Vector3 position)
    {
        Bot closestBot = null;
        float minDistance = float.MaxValue;

        foreach (Bot bot in _bots)
        {
            if (!bot.IsIdle)
                continue;

            float distance = (bot.transform.position - position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                closestBot = bot;
            }
        }

        return closestBot;
    }

    public void AddBot(Bot bot)
    {
        if (_bots.Contains(bot))
            return;

        _bots.Add(bot);
        ConfigureBot(bot);
        BotAdded?.Invoke(bot);
    }

    public void RemoveBot(Bot bot)
    {
        if (!_bots.Contains(bot))
            return;

        bot.ResourceDelivered -= OnResourceDelivered;
        _bots.Remove(bot);
        BotRemoved?.Invoke(bot);
    }

    private void ConfigureBot(Bot bot)
    {
        bot.SetDeliveryPoint(this);
        bot.ResourceDelivered += OnResourceDelivered;
    }

    private IEnumerator ScanRoutine()
    {
        if (_scanner == null)
            yield break;

        var interval = new WaitForSeconds(_scanInterval);

        while (enabled)
        {
            _scanner.Scan();
            yield return interval;
        }
    }

    private void OnResourceDelivered(Resource resource)
    {
        _resourceRepository.Remove(resource);
        _economy.OnResourceDelivered();
    }
}
