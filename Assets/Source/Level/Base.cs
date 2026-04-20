using System;
using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Bot[] _bots;
    [SerializeField] private OreScanner _scanner;
    [SerializeField] private float _scanInterval;

    private int _resourceCount;

    public int ResourceCount => _resourceCount;

    public event Action<int> ResourceCountChanged;

    private void Start()
    {
        foreach (var bot in _bots)
        {
            bot.Initialize(transform);
            bot.OreDelivered += OnOreDelivered;
        }

        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_scanInterval);
            TryDispatch();
        }
    }

    private void TryDispatch()
    {
        Bot freeBot;

        while ((freeBot = FindFreeBot()) != null)
        {
            Ore ore = _scanner.FindFreeOre();

            if (ore == null)
                return;

            freeBot.Collect(ore);
        }
    }

    private Bot FindFreeBot()
    {
        foreach (var bot in _bots)
        {
            if (bot.IsIdle)
                return bot;
        }

        return null;
    }

    private void OnOreDelivered(Ore ore)
    {
        _resourceCount++;
        ResourceCountChanged?.Invoke(_resourceCount);
    }
}

