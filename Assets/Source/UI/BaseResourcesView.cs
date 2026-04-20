using System;
using TMPro;
using UnityEngine;

public class BaseResourcesView : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private TextMeshProUGUI _resourcesText;

    private string _resourcesTextPrefix = "Количество доступных ресурсов: ";

    private void OnEnable()
    {
        if (_base != null)
            _base.ResourceCountChanged += OnResourceCountChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (_base != null)
            _base.ResourceCountChanged -= OnResourceCountChanged;
    }

    private void OnResourceCountChanged(int count)
    {
        _resourcesText.text = count.ToString();
    }

    private void Refresh()
    {
        if (_base == null || _resourcesText == null)
            throw new NullReferenceException();

        _resourcesText.text = _resourcesTextPrefix + _base.ResourceCount.ToString();
    }
}
