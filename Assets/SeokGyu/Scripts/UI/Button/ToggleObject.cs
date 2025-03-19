using DG.Tweening;
using UnityEngine;

namespace EverScord
{
    public class ToggleObject : MonoBehaviour, ISound
    {
        [SerializeField] protected GameObject[] toggleObjects;
        [SerializeField] protected ObjectOption[] objectOptions;
        [SerializeField] private string doTweenID;
        private bool bReverse = false;
        private bool bActive = false;

        private void Awake()
        {
            for (int i = 0; i < objectOptions.Length; i++)
            {
                objectOptions[i].Initialize();
            }

            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public virtual void OnActivateObject(int index)
        {
            if (index >= toggleObjects.Length || index < 0) return;

            toggleObjects[index].SetActive(true);
        }

        public virtual void OnActivateObjects()
        {
            for (int i = 0; i < toggleObjects.Length; i++)
            {
                toggleObjects[i].SetActive(true);
            }
        }

        public virtual void OnDeactivateObject(int index)
        {
            if (index >= toggleObjects.Length || index < 0) return;

            toggleObjects[index].SetActive(false);
        }

        public virtual void OnDeactivateObjects()
        {
            for (int i = 0; i < toggleObjects.Length; i++)
            {
                toggleObjects[i].SetActive(false);
            }
        }

        public virtual void OnToggleObject(int index)
        {
            if (index >= toggleObjects.Length || index < 0) return;

            toggleObjects[index].SetActive(!toggleObjects[index].activeSelf);
        }

        public virtual void OnToggleObjects()
        {
            for (int i = 0; i < toggleObjects.Length; i++)
            {
                toggleObjects[i].SetActive(!toggleObjects[i].activeSelf);
            }
        }

        public void PlayButtonSound()
        {
            SoundManager.Instance.PlaySound("ButtonSound");
        }

        public void PlayDoTween(bool bReverse)
        {
            if(bActive == false && bReverse == false)
            {
                bActive = true;
                this.bReverse = true;
                OnActivateObjects();
                DOTween.PlayForward(doTweenID);
            }
            else if(bActive == true && bReverse == true)
            {
                bActive = false;
                this.bReverse = false;
                DOTween.PlayBackwards(doTweenID);
            }
        }

        public void PlayDoTweenToggleObjects()
        {
            if (!bReverse)
            {
                bActive = true;
                bReverse = true;
                OnActivateObjects();
                DOTween.PlayForward(doTweenID);
            }
            else
            {
                bActive = false;
                bReverse = false;
                DOTween.PlayBackwards(doTweenID);
            }
        }

        [System.Serializable]
        public class ObjectOption
        {
            public enum EPosPreset
            {
                None,
                MiddleCenter,
                Custom
            }

            [SerializeField] private GameObject targetObject;
            [SerializeField] private EPosPreset positionPreset;
            [SerializeField] private Canvas canvas;
            [SerializeField] private Vector2 offset;

            public void Initialize()
            {
                switch(positionPreset)
                {
                    case EPosPreset.MiddleCenter:
                        {
                            targetObject.transform.position = canvas.transform.position;
                            targetObject.GetComponent<RectTransform>().sizeDelta += offset;
                        }
                        break;
                    case EPosPreset.Custom:
                        {
                            targetObject.transform.position += new Vector3(offset.x, offset.y, 0f);
                        }
                        break;
                }
            }
        }
    }
}
