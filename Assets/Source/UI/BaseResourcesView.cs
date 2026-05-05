using TMPro;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class BaseResourcesView : MonoBehaviour
{
    [SerializeField] private TextMeshPro _resourcesText;

    private Base _base;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        _base = GetComponent<Base>();

        if (_base != null)
            _base.ResourceCountChanged += OnResourceCountChanged;

        UpdateText(_base != null ? _base.ResourceCount : 0);
    }

    private void OnDestroy()
    {
        if (_base != null)
            _base.ResourceCountChanged -= OnResourceCountChanged;
    }

    private void LateUpdate()
    {
        if (_camera != null)
        {
            _resourcesText.transform.LookAt(_camera.transform.position, Vector3.up);
            _resourcesText.transform.forward = _camera.transform.forward;
        }

        Vector3 direction = _resourcesText.transform.position - _camera.transform.position;
        direction.y = 0f;
        _resourcesText.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void OnResourceCountChanged(int count)
    {
        UpdateText(count);
    }

    private void UpdateText(int count)
    {
        _resourcesText.text = $"Ресурсы: {count}";
    }
}
