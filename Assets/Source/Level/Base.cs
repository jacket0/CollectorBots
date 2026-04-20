using System;
using System.Collections;
using Assets.Source.Resourse;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Bot[] _bots;
    [SerializeField] private OreScanner _scanner;
    [SerializeField] private OreRepository _resourceRepository;
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
        var time = new WaitForSeconds(_scanInterval);

        while (true)
        {
            yield return time;
            _scanner.Scan();
            TryDispatch();
        }
    }

    private void TryDispatch()
    {
        Bot freeBot;

        while ((freeBot = FindFreeBot()) != null)
        {
            Ore ore = _resourceRepository.TakeFree();

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
        _resourceRepository.Remove(ore);
        _resourceCount++;
        ResourceCountChanged?.Invoke(_resourceCount);
    }
}

