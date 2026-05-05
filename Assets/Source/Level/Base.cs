using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceScanner))]
public class Base : MonoBehaviour
{
    [Header("Боты")]
    [SerializeField] private Bot _botPrefab;

    [Header("Ресурсы")]
    [SerializeField] private int _botCost = 3;
    [SerializeField] private int _colonizationCost = 5;

    [Header("Флаг")]
    [SerializeField] private BaseFlag _flagPrefab;

    [Header("Сканирование")]
    [SerializeField] private float _scanInterval;

    private ResourceScanner _scanner;
    private ResourceRepository _resourceRepository;
    private BaseFlag _flag;

    private readonly List<Bot> _bots = new();
    private int _resourceCount;
    private bool _isColonizationInProgress;
    private int _minColonizationBots = 1;

    public IReadOnlyList<Bot> Bots => _bots;

    public int ResourceCount => _resourceCount;

    public event Action<Bot> BotAdded;
    public event Action<Bot> BotRemoved;
    public event Action<int> ResourceCountChanged;
    public event Action<Base, Bot, Vector3> ColonizationRequested;

    public void Initialize(ResourceScanner scanner, ResourceRepository repository)
    {
        _scanner = scanner;
        _resourceRepository = repository;

        if (_scanner != null && _resourceRepository != null)
            _scanner.Initialize(repository);
    }

    private void Awake()
    {
        _scanner = GetComponent<ResourceScanner>();

        if (_resourceRepository == null)
            _resourceRepository = FindFirstObjectByType<ResourceRepository>();

        if (_scanner != null && _resourceRepository != null)
            _scanner.Initialize(_resourceRepository);
    }

    private void Start()
    {
        ResourceDispatcher dispatcher = FindFirstObjectByType<ResourceDispatcher>();
        dispatcher?.RegisterBase(this);

        StartCoroutine(ScanRoutine());
    }

    public void PlaceFlag(Vector3 position)
    {
        if (_flag == null)
            _flag = Instantiate(_flagPrefab, position, Quaternion.identity);
        else
            _flag.transform.position = position;
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
        bot.BindToBase(this);
        bot.ResourceDelivered += OnResourceDelivered;
    }

    private IEnumerator ScanRoutine()
    {
        if (_scanner == null || _resourceRepository == null)
            yield break;

        var interval = new WaitForSeconds(_scanInterval);

        while (true)
        {
            _scanner.Scan();
            yield return interval;
        }
    }

    private void OnResourceDelivered(Resource resource)
    {
        _resourceRepository.Remove(resource);
        _resourceCount++;
        ResourceCountChanged?.Invoke(_resourceCount);

        if (_flag != null)
            TryColonize();
        else
            TrySpawnBot();
    }

    private void TrySpawnBot()
    {
        if (_resourceCount < _botCost)
            return;

        _resourceCount -= _botCost;

        Bot bot = Instantiate(_botPrefab, transform.position, Quaternion.identity);
        _bots.Add(bot);
        ConfigureBot(bot);
        BotAdded?.Invoke(bot);

        ResourceCountChanged?.Invoke(_resourceCount);
    }

    private void OnDisable()
    {
        ResourceDispatcher dispatcher = FindFirstObjectByType<ResourceDispatcher>();
        dispatcher?.UnregisterBase(this);

        if (_flag != null)
            Destroy(_flag.gameObject);
    }

    private void TryColonize()
    {
        if (_isColonizationInProgress || _resourceCount < _colonizationCost || _bots.Count <= _minColonizationBots)
            return;

        Bot freeBot = null;

        foreach (var bot in _bots)
        {
            if (bot.IsIdle)
            {
                freeBot = bot;
                break;
            }
        }

        if (freeBot == null)
            return;

        _isColonizationInProgress = true;
        _resourceCount -= _colonizationCost;
        ResourceCountChanged?.Invoke(_resourceCount);

        Vector3 flagPosition = _flag.transform.position;
        RemoveBot(freeBot);
        freeBot.GoToFlag(flagPosition, OnBotArrivedAtFlag);
    }

    private void OnBotArrivedAtFlag(Bot bot)
    {
        Vector3 position = bot.transform.position;

        Destroy(_flag.gameObject);
        _flag = null;

        ColonizationRequested?.Invoke(this, bot, position);

        _isColonizationInProgress = false;
    }
}
