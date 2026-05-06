using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private ResourceRepository _repository;
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private BotSpawner _botSpawner;
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        _baseSpawner.Initialize(_repository, _botSpawner, _camera);
        _baseSpawner.Spawn();
    }
}
