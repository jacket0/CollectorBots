using UnityEngine;

public class BaseSpawner : Spawner
{
    [SerializeField] private Base _basePrefab;
    [SerializeField] private Vector3 _spawnPosition;
    [SerializeField] private int _initialBotCount = 1;

    private ResourceRepository _repository;
    private BotSpawner _botSpawner;
    private Camera _camera;

    public void Initialize(ResourceRepository repository, BotSpawner botSpawner, Camera camera)
    {
        _repository = repository;
        _botSpawner = botSpawner;
        _camera = camera;
    }

    public override void Spawn()
    {
        Base newBase = SpawnBase(_spawnPosition);

        for (int i = 0; i < _initialBotCount; i++)
            newBase.AddBot(_botSpawner.SpawnAt(_spawnPosition));
    }

    public Base SpawnAt(Vector3 position)
    {
        return SpawnBase(position);
    }

    private Base SpawnBase(Vector3 position)
    {
        Base newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.Initialize(_repository);

        BaseEconomy economy = newBase.GetComponent<BaseEconomy>();
        economy.BotSpawnRequested += OnBotSpawnRequested;
        economy.ColonizationRequested += OnColonizationRequested;

        _repository.RegisterBase(newBase);

        BaseResourcesView view = newBase.GetComponent<BaseResourcesView>();
        view?.Initialize(_camera);

        return newBase;
    }

    private void OnBotSpawnRequested(BaseEconomy economy)
    {
        Base ownerBase = economy.GetComponent<Base>();
        Bot bot = _botSpawner.SpawnAt(ownerBase.transform.position);
        ownerBase.AddBot(bot);
    }

    private void OnColonizationRequested(BaseEconomy economy, Bot bot, Vector3 position)
    {
        Base newBase = SpawnBase(position);
        newBase.AddBot(bot);
    }
}
