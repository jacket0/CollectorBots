using System.Collections;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    [SerializeField] private Ore _ore;
    [SerializeField] private float _spawnInterval;
    [SerializeField] private int _capacity = 5;
    [SerializeField] private int _maxSize = 50;

    private Transform[] _spawnPoints;
    private Pool<Ore> _pool;

    private void Start()
    {
        _spawnPoints = GetComponentsInChildren<Transform>(true);
        _pool = new Pool<Ore>(_ore, _capacity, _maxSize);
        StartCoroutine(SpawnRoutine());
    }

    public Ore Spawn()
    {
        Ore ore = _pool.GetObject();
        ore.transform.position = GetRandomSpawnPoint();
        return ore;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int index = Random.Range(1, _spawnPoints.Length);
        return _spawnPoints[index].position;
    }
}
