using UnityEditor.Experimental.GraphView;
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

        private void Awake()
        {
            Init();
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

            //bossIcon.GetComponent<Image>();
            bossIcon.SetActive(false);
        }
    }
}
