using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [Header("Префабы")]
    [SerializeField] private Base _basePrefab;
    [SerializeField] private Bot _botPrefab;

    [Header("Старт")]
    [SerializeField] private Transform _firstBaseSpawnPoint;
    [SerializeField] private int _initialBotsCount = 3;

    private ResourceRepository _resourceRepository;

    private void Awake()
    {
        _resourceRepository = FindFirstObjectByType<ResourceRepository>();
    }

    private void Start()
    {
        Vector3 position = _firstBaseSpawnPoint != null ? _firstBaseSpawnPoint.position : Vector3.zero;
        Base firstBase = SpawnBase(position);

        for (int i = 0; i < _initialBotsCount; i++)
        {
            Bot bot = Instantiate(_botPrefab, position, Quaternion.identity);
            firstBase.AddBot(bot);
        }
    }

    private Base SpawnBase(Vector3 position)
    {
        Base newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.Initialize(newBase.GetComponent<ResourceScanner>(), _resourceRepository);
        newBase.ColonizationRequested += OnColonizationRequested;
        return newBase;
    }

    private void OnColonizationRequested(Base source, Bot bot, Vector3 position)
    {
        Base newBase = SpawnBase(position);
        bot.TransferTo(newBase);
        newBase.AddBot(bot);
    }
}
