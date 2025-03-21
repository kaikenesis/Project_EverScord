using System;
using EPOOutline.Demo;
using EverScord.GameCamera;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class MinimapCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject minimapCamera;
        [SerializeField] private GameObject minimapBackgroundUI;
        private GameObject backgroundObj;
        private GameObject cameraObj;

        private void Awake()
        {
            Init();

            LevelControl.OnLevelUpdated += HandleLevelUpdated;
        }

        private void OnDestroy()
        {
            LevelControl.OnLevelUpdated -= HandleLevelUpdated;
        }

        private void HandleLevelUpdated(int curStageNum, bool bCoverScreen)
        {
            if (bCoverScreen == false)
                return;

            MinimapData.CameraTransform cameraData = GameManager.Instance.MinimapData.CameraOptions[curStageNum];
            cameraObj.transform.SetLocalPositionAndRotation(cameraData.Position, cameraData.Rotation);
            cameraObj.GetComponent<Camera>().orthographicSize = GameManager.Instance.MinimapData.CameraOptions[curStageNum].OrthographicSize;

            MinimapData.StageMap stageMapData = GameManager.Instance.MinimapData.StageMaps[curStageNum];
            backgroundObj.GetComponent<Image>().sprite = stageMapData.SourceImg;
            backgroundObj.transform.SetLocalPositionAndRotation(stageMapData.Position, stageMapData.Rotation);
            backgroundObj.GetComponent<RectTransform>().sizeDelta = stageMapData.Size;
        }

        private void Init()
        {
            cameraObj = Instantiate(minimapCamera, CharacterCamera.Root);
            MinimapData.CameraTransform cameraData = GameManager.Instance.MinimapData.CameraOptions[0];
            cameraObj.transform.SetLocalPositionAndRotation(cameraData.Position, cameraData.Rotation);
            cameraObj.GetComponent<Camera>().orthographicSize = GameManager.Instance.MinimapData.CameraOptions[0].OrthographicSize;

            backgroundObj = Instantiate(minimapBackgroundUI);
            MinimapData.StageMap stageMapData = GameManager.Instance.MinimapData.StageMaps[0];
            backgroundObj.GetComponent<Image>().sprite = stageMapData.SourceImg;
            backgroundObj.transform.SetLocalPositionAndRotation(stageMapData.Position, stageMapData.Rotation);
            backgroundObj.GetComponent<RectTransform>().sizeDelta = stageMapData.Size;
        }

        
    }
}

