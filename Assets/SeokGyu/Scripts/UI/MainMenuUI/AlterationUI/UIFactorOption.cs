using System;
using TMPro;
using UnityEngine;

public class UIFactorOption : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    private float value;

    public static Action OnSelectOption = delegate { };

    public void Initialize(string name, float value)
    {
        nameText.text = name;
        this.value = value;
    }

    public void OnClicked()
    {
        // ������ �ɼ� �ִ��ġ ����
        Debug.Log($"MaxValue : {value}");
        OnSelectOption?.Invoke();
    }
}
