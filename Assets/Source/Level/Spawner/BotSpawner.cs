using UnityEngine;

public class BotSpawner : Spawner
{
    [SerializeField] private Bot _botPrefab;
    [SerializeField] private int _capacity = 5;
    [SerializeField] private int _maxSize = 20;

    private Pool<Bot> _pool;

    private void Awake()
    {
        _pool = new Pool<Bot>(_botPrefab, _capacity, _maxSize);
    }

    public override void Spawn()
    {
        _pool.GetObject();
    }

    public Bot SpawnAt(Vector3 position)
    {
        Bot bot = _pool.GetObject();
        bot.transform.position = position;
        return bot;
    }
}
