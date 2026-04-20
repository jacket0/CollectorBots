using Assets.Source.Resourse;
using System;
using UnityEngine;

public class OreScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private LayerMask _oreLayer;
    [SerializeField] private OreRepository _resoursesRepository;

    public void Scan()
    {
        if (_resoursesRepository == null)
            throw new NullReferenceException();

        Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius, _oreLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Ore ore))
                _resoursesRepository.Pull(ore);
        }
    }
}
