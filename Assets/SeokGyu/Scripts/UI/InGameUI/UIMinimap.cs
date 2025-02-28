using EverScord.Character;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIMinimap : MonoBehaviour
    {
        [SerializeField] private GameObject map;
        [SerializeField] private GameObject portalIcon;
        [SerializeField] private GameObject bossIcon;
        [SerializeField] private GameObject deathIcon;

        private int curStageNum = 0;
        private MinimapData.StageMap curStageMap;
        private MinimapData.IconTransform curPortal;
        private List<GameObject> deathIcons;

        private void Awake()
        {
            Init();

            CharacterControl.OnPhotonViewListUpdated += HandlePhotonViewListUpdated;
        }

        private void OnDestroy()
        {
            CharacterControl.OnPhotonViewListUpdated -= HandlePhotonViewListUpdated;
        }

        private void HandlePhotonViewListUpdated()
        {
            // Callback함수로 호출할것 없이 최대플레이어 수만큼 생성해두고 사용하면 될듯
            //List<PhotonView> list = GameManager.Instance.playerPhotonViews;
            //Debug.Log(list.Count);

            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (deathIcons.Count < list.Count)
            //    {
            //        GameObject obj;
            //        obj = Instantiate(deathIcon, transform);
            //        deathIcons.Add(obj);
            //        obj.SetActive(false);
            //    }
            //}
        }

        private void Init()
        {
            // MapImage
            GameObject mapObj = Instantiate(map, transform);
            curStageMap = GameManager.Instance.MinimapData.StageMaps[curStageNum];

            mapObj.GetComponent<Image>().sprite = curStageMap.SourceImg;

            RectTransform mapRT = mapObj.GetComponent<RectTransform>();
            mapRT.SetLocalPositionAndRotation(curStageMap.Position, curStageMap.Rotation);
            mapRT.sizeDelta = curStageMap.Size;

            // Portal
            GameObject portalObj = Instantiate(portalIcon, transform);
            curPortal = GameManager.Instance.MinimapData.PortalIconPos[curStageNum];

            RectTransform portalRT = portalObj.GetComponent<RectTransform>();
            portalRT.SetLocalPositionAndRotation(curPortal.Position, curPortal.Rotation);
            portalIcon.SetActive(false);

            // Boss
            GameObject bossObj = Instantiate(bossIcon, transform);
            bossObj.SetActive(false);
        }
    }
}
