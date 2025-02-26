using EverScord.Character;
using System;
using UnityEngine;

namespace EverScord
{
    public class UIHealth : UIProgress
    {
        [Range(0.0f,1.0f)]
        [SerializeField] private float value;

        private void Awake()
        {
            CharacterControl.OnHealthUpdated += HandleHealthUpdated;
            LevelControl.OnProgressUpdated += HandleStageProgressUpdated;
        }

        private void OnDestroy()
        {
            CharacterControl.OnHealthUpdated -= HandleHealthUpdated;
            LevelControl.OnProgressUpdated -= HandleStageProgressUpdated;
        }

        private void HandleHealthUpdated(float curHP)
        {
            if (type != EType.PlayerHealth)
                return;

            UpdateProgress(curHP);
        }

        private void HandleStageProgressUpdated(float value)
        {
            if (type != EType.StageProgress)
                return;

            UpdateProgress(value);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                UpdateFillAmount(value);
            }
        }
        
        private void UpdateProgress(float value)
        {
            this.value = value;
            UpdateFillAmount(value);
        }
    }
}
