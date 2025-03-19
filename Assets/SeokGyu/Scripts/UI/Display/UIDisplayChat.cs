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
            PhotonChatController.OnSendMsgDead += HandleDisplayMsgDead;
            PhotonChatController.OnSendMsgAlive += HandleDisplayMsgAlive;

            Initialize();
        }

        private void OnDestroy()
        {
            PhotonChatController.OnSendMsgDead -= HandleDisplayMsgDead;
            PhotonChatController.OnSendMsgAlive -= HandleDisplayMsgAlive;
        }

        private async void Initialize()
        {
            await ResourceManager.Instance.CreatePool("SystemMsg", 10);
        }

        #region Handle Methods
        private void HandleDisplayMsgDead(string message)
        {
            GameObject obj = ResourceManager.Instance.GetFromPool("SystemMsg", Vector3.zero, Quaternion.identity);
            obj.GetComponent<TMP_Text>().text = message;
            obj.transform.parent = containor;
            if(queueTextObject.Count >= 9)
            {
                ResourceManager.Instance.ReturnToPool(queueTextObject.Peek(), "SystemMsg");
                queueTextObject.Dequeue();
            }
            queueTextObject.Enqueue(obj);
        }
        private void HandleDisplayMsgAlive(string message)
        {
            GameObject obj = ResourceManager.Instance.GetFromPool("SystemMsg", Vector3.zero, Quaternion.identity);
            obj.GetComponent<TMP_Text>().text = message;
            obj.transform.parent = containor;
            if (queueTextObject.Count >= 9)
            {
                ResourceManager.Instance.ReturnToPool(queueTextObject.Peek(), "SystemMsg");
                queueTextObject.Dequeue();
            }
            queueTextObject.Enqueue(obj);
        }
        #endregion
    }
}
