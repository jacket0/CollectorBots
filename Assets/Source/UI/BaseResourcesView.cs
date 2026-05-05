using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseResourcesView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesText;

    private static readonly Dictionary<Base, BaseResourcesView> RegisteredViews = new();

    private Base _base;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_camera != null)
            transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }

    public void Initialize(Base baseOwner)
    {
        if (_base != null)
        {
            _base.ResourceCountChanged -= OnResourceCountChanged;
        }

        _base = baseOwner;

        if (_base != null)
        {
            _base.ResourceCountChanged += OnResourceCountChanged;
            UpdateText(_base.ResourceCount);
        }
    }

    private void OnDestroy()
    {
        if (_base != null)
        {
            _base.ResourceCountChanged -= OnResourceCountChanged;
            if (RegisteredViews.TryGetValue(_base, out BaseResourcesView registeredView) && registeredView == this)
                RegisteredViews.Remove(_base);
        }
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
