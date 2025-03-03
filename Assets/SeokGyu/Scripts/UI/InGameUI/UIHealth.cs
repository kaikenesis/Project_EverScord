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
            Initialize();

            CharacterControl.OnHealthUpdated += HandleHealthUpdated;
            LevelControl.OnProgressUpdated += HandleStageProgressUpdated;
            LevelControl.OnLevelUpdated += HandleLevelUpdated;
        }

        private void OnDestroy()
        {
            CharacterControl.OnHealthUpdated -= HandleHealthUpdated;
            LevelControl.OnProgressUpdated -= HandleStageProgressUpdated;
            LevelControl.OnLevelUpdated -= HandleLevelUpdated;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                UpdateFillAmount(value);
            }
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

        private void HandleLevelUpdated(int curStageNum, bool bCoverSreen)
        {
            if (type != EType.StageProgress || bCoverSreen == false)
                return;

            UpdateProgress(0f);
        }

        private void Initialize()
        {
            if(type != EType.BossHealth)
            {
                UpdateProgress(0f);
            }
            else
            {
                UpdateProgress(1f);
            }
        }

        private void UpdateProgress(float value)
        {
            this.value = value;
            UpdateFillAmount(value);
        }
    }
}
