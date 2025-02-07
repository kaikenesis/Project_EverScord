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
        // 선택한 옵션 최대수치 적용
        Debug.Log($"MaxValue : {value}");
        OnSelectOption?.Invoke();
    }
}
