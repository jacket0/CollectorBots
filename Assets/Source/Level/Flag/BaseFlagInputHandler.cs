using UnityEngine;
using UnityEngine.InputSystem;

public class BaseFlagInputHandler : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private Base _selectedBase;

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            _selectedBase = null;
            return;
        }

        Base clickedBase = hit.collider.GetComponentInParent<Base>();

        if (clickedBase != null)
        {
            _selectedBase = clickedBase;
            return;
        }

        if (_selectedBase == null)
            return;

        if (hit.collider.TryGetComponent(out FlagPlacementArea _))
            _selectedBase.PlaceFlag(hit.point);
    }
}