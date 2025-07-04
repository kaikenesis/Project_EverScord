using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayChat : MonoBehaviour
    {
        [SerializeField] private GameObject textObject;
        [SerializeField] private RectTransform containor;
        private Queue<GameObject> queueTextObject = new Queue<GameObject>();

        private void Awake()
        {
            PhotonChatController.OnSendSystemMsg += HandleDisplayMsg;

            Initialize();
        }

        private void OnDestroy()
        {
            PhotonChatController.OnSendSystemMsg -= HandleDisplayMsg;
        }

        private async void Initialize()
        {
            await ResourceManager.Instance.CreatePool("SystemMsg", 15);
        }

        #region Handle Methods
        private void HandleDisplayMsg(string message)
        {
            GameObject obj = ResourceManager.Instance.GetFromPool("SystemMsg", Vector3.zero, Quaternion.identity);
            if (obj != null)
            {
                obj.GetComponent<TMP_Text>().text = message;
                obj.transform.parent = containor;
                obj.transform.SetAsLastSibling();
                if (queueTextObject.Count >= 9)
                {
                    ResourceManager.Instance.ReturnToPool(queueTextObject.Peek(), "SystemMsg");
                    queueTextObject.Dequeue();
                }
                queueTextObject.Enqueue(obj);
            }
        }
        #endregion
    }
}
