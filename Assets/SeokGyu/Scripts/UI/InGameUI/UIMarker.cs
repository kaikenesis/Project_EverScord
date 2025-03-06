using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIMarker : MonoBehaviour
    {
        [SerializeField] private PointMarkData.EType type;
        private GameObject canvasObj;
        private List<GameObject> markers = new List<GameObject>();
        private PhotonView pv;

        public void Initialize(PointMarkData.EType type)
        {
            this.type = type;

            canvasObj = new GameObject("MarkerUI");
            canvasObj.transform.SetParent(ResourceManager.PoolRoot);
            canvasObj.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            canvasObj.layer = 13;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            pv = GetComponent<PhotonView>();

            GameObject markerObj = ResourceManager.Instance.GetAsset<GameObject>("PointMark");
            GameObject obj = Instantiate(markerObj, canvasObj.transform);
            Image img = obj.GetComponent<Image>();
            RectTransform markRT = obj.GetComponent<RectTransform>();
            Vector2 size = new Vector2();
            GameManager.Instance.PointMarkData.SetMarker((int)type, img, out size);
            markRT.sizeDelta = size;
            markers.Add(obj);

            switch (type)
            {
                case PointMarkData.EType.Player:
                    {
                        if(!pv.IsMine)
                        {
                            GameManager.Instance.PointMarkData.SetMarker((int)type + 1, img, out size);
                            markRT.sizeDelta = size;
                        }

                        GameObject death = Instantiate(markerObj, canvasObj.transform);
                        Image deathImg = death.GetComponent<Image>();
                        RectTransform deathMarkRT = death.GetComponent<RectTransform>();
                        GameManager.Instance.PointMarkData.SetMarker((int)type + 2, deathImg, out size);
                        deathMarkRT.sizeDelta = size;
                        markers.Add(death);
                        death.SetActive(false);
                    }
                    break;
            }
        }

        public void UpdatePosition(Vector3 position)
        {
            canvasObj.transform.position = new Vector3(position.x, 1f, position.z);
        }

        public void ToggleDeathIcon()
        {
            if (type != PointMarkData.EType.Player) return;

            markers[0].SetActive(!markers[0].activeSelf);
            markers[1].SetActive(!markers[1].activeSelf);
        }

        public void SetActivate(bool bActive)
        {
            if (canvasObj != null)
                canvasObj.SetActive(bActive);
        }
    }
}
