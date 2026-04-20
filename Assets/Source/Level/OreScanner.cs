using UnityEngine;

public class OreScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private LayerMask _oreLayer;

    public Ore FindFreeOre()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius, _oreLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Ore ore) && ore.IsReserved == false)
                return ore;
        }

        return null;
    }
}
