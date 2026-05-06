using TMPro;
using UnityEngine;

[RequireComponent(typeof(BaseEconomy))]
public class BaseResourcesView : MonoBehaviour
{
    [SerializeField] private TextMeshPro _resourcesText;

    private BaseEconomy _economy;
    private Camera _camera;

    public void Initialize(Camera camera)
    {
        _camera = camera;
    }

    private void Awake()
    {
        _economy = GetComponent<BaseEconomy>();
        _economy.ResourceCountChanged += OnResourceCountChanged;
        UpdateText(0);
    }

    private void OnDestroy()
    {
        _economy.ResourceCountChanged -= OnResourceCountChanged;
    }

    private void LateUpdate()
    {
        if (_camera == null)
            return;

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
