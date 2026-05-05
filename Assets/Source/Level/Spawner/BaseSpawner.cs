using UnityEngine;

public class BaseSpawner : Spawner
{
    [Header("База")]
    [SerializeField] private Base _basePrefab;
    [SerializeField] private Vector3 _spawnPosition;

    [Header("Боты")]
    [SerializeField] private Bot _botPrefab;
    [SerializeField] private int _initialBotCount = 1;

    private ResourceRepository _repository;
    private ResourceDispatcher _dispatcher;

    private void Start()
    {
        _repository = FindFirstObjectByType<ResourceRepository>();
        _dispatcher = FindFirstObjectByType<ResourceDispatcher>();

        Spawn();
    }

    public override void Spawn()
    {
        Base newBase = SpawnBase(_spawnPosition);

        for (int i = 0; i < _initialBotCount; i++)
        {
            Bot bot = Instantiate(_botPrefab, _spawnPosition, Quaternion.identity);
            newBase.AddBot(bot);
        }
    }

    public Base SpawnAt(Vector3 position)
    {
        return SpawnBase(position);
    }

    private Base SpawnBase(Vector3 position)
    {
        Base newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.Initialize(newBase.GetComponent<ResourceScanner>(), _repository);

        newBase.BotSpawnRequested += OnBotSpawnRequested;
        newBase.ColonizationRequested += OnColonizationRequested;

        _dispatcher?.RegisterBase(newBase);

        return newBase;
    }

    private void OnBotSpawnRequested(Base ownerBase)
    {
        Bot bot = Instantiate(_botPrefab, ownerBase.transform.position, Quaternion.identity);
        ownerBase.AddBot(bot);
    }

    private void OnColonizationRequested(Base sourceBase, Bot bot, Vector3 position)
    {
        Base newBase = SpawnBase(position);
        newBase.AddBot(bot);
    }
}
