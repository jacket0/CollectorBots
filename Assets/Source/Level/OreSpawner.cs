using System.Collections;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private float _spawnInterval;
    [SerializeField] private int _capacity = 5;
    [SerializeField] private int _maxSize = 50;

    private Transform[] _spawnPoints;
    private Pool<Resource> _pool;

    private void Start()
    {
        _spawnPoints = GetComponentsInChildren<Transform>(true);
        _pool = new Pool<Resource>(_resource, _capacity, _maxSize);
        StartCoroutine(SpawnRoutine());
    }

    public Resource Spawn()
    {
        Resource resource = _pool.GetObject();
        resource.transform.position = GetRandomSpawnPoint();
        return resource;
    }

    private IEnumerator SpawnRoutine()
    {
        var time = new WaitForSeconds(_spawnInterval);

        while (true)
        {
            Spawn();
            yield return time;
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int index = Random.Range(1, _spawnPoints.Length);
        return _spawnPoints[index].position;
    }
}
