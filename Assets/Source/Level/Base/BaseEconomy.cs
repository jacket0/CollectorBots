using System;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class BaseEconomy : MonoBehaviour
{
    [Header("Стоимость")]
    [SerializeField] private int _botCost = 3;
    [SerializeField] private int _colonizationCost = 5;

    [Header("Флаг")]
    [SerializeField] private BaseFlag _flagPrefab;
    [SerializeField] private int _flagPoolCapacity = 1;

    private Base _base;
    private Pool<BaseFlag> _flagPool;
    private BaseFlag _activeFlag;

    private int _resourceCount;
    private bool _isColonizationInProgress;
    private int _minColonizationBots = 1;

    public int ResourceCount => _resourceCount;
    public bool HasFlag => _activeFlag != null;

    public event Action<int> ResourceCountChanged;
    public event Action<BaseEconomy> BotSpawnRequested;
    public event Action<BaseEconomy, Bot, Vector3> ColonizationRequested;

    private void Awake()
    {
        _base = GetComponent<Base>();
        _flagPool = new Pool<BaseFlag>(_flagPrefab, _flagPoolCapacity, _flagPoolCapacity);
    }

    private void OnEnable()
    {
        _base.BotAdded += OnBotAdded;
        _base.BotRemoved += OnBotRemoved;
    }

    private void OnDisable()
    {
        _base.BotAdded -= OnBotAdded;
        _base.BotRemoved -= OnBotRemoved;
    }

    public void PlaceFlag(Vector3 position)
    {
        if (_activeFlag == null)
        {
            _activeFlag = _flagPool.GetObject();
            _activeFlag.Released += OnFlagReleased;
        }

        _activeFlag.transform.position = position;
    }

    public void OnResourceDelivered()
    {
        _resourceCount++;
        ResourceCountChanged?.Invoke(_resourceCount);

        if (HasFlag)
            TryColonize();
        else
            TrySpawnBot();
    }

    private void OnBotAdded(Bot bot) { }

    private void OnBotRemoved(Bot bot) { }

    private void TrySpawnBot()
    {
        if (_resourceCount < _botCost)
            return;

        _resourceCount -= _botCost;
        ResourceCountChanged?.Invoke(_resourceCount);
        BotSpawnRequested?.Invoke(this);
    }

    private void TryColonize()
    {
        if (_isColonizationInProgress || _resourceCount < _colonizationCost || _base.Bots.Count <= _minColonizationBots)
            return;

        Bot freeBot = null;

        foreach (Bot bot in _base.Bots)
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

        Vector3 flagPosition = _activeFlag.transform.position;
        _base.RemoveBot(freeBot);
        freeBot.GoToFlag(flagPosition, OnBotArrivedAtFlag);
    }

    private void OnBotArrivedAtFlag(Bot bot)
    {
        Vector3 position = bot.transform.position;

        _activeFlag.Release();
        _isColonizationInProgress = false;

        ColonizationRequested?.Invoke(this, bot, position);
    }

    private void OnFlagReleased(BaseFlag flag)
    {
        flag.Released -= OnFlagReleased;
        _activeFlag = null;
    }
}