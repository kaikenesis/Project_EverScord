using EverScord;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;

    private void Start()
    {
        if (GameManager.Instance.PlayerData.difficulty == PlayerData.EDifficulty.Hard)
            textMesh.text = "Hard";
        else
            textMesh.text = "Normal";
    }
}
