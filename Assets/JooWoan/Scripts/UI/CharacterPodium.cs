using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EverScord.UI
{
    public class CharacterPodium : MonoBehaviour
    {
        [SerializeField] private GameObject characterHub;
        [SerializeField] private LayerMask playerLayer, uiLayer;
        [SerializeField] private float rotationSpeed;
        private GraphicRaycaster[] raycasters;
        [field: SerializeField] public List<CharacterInfo> CharacterList { get; private set; }

        private EventSystem eventSystem;
        private TitleControl titleControl;
        private Transform currentCharacter;
        private InputInfo playerInput;
        private bool isInteractingUI;

        private void Awake()
        {
            UIDisplayRoom.OnVisibleObject += HandleVisibleObject;
            UISelect.OnChangeCharacter += HandleChangeCharacter;

            SetCharacterTransform();
        }

        private void OnDestroy()
        {
            UIDisplayRoom.OnVisibleObject -= HandleVisibleObject;
            UISelect.OnChangeCharacter -= HandleChangeCharacter;
        }

        void Start()
        {
            isInteractingUI = false;

            titleControl = GameManager.Instance.TitleController;
            eventSystem = GetComponent<EventSystem>();
            raycasters = FindObjectsOfType<GraphicRaycaster>();
        }

        private void HandleVisibleObject()
        {
            ShowCharacters();
        }

        private void HandleChangeCharacter()
        {
            SwitchPlayer(GameManager.Instance.PlayerData.character);
        }

        void Update()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            if (titleControl.IsExaminingAlteration)
                return;

            playerInput = InputControl.ReceiveInput();
            RotatePlayer();
        }

        private void SetCharacterTransform()
        {
            if (CharacterList.Count == 0)
                return;

            if (GameManager.Instance.PlayerData != null)
            {
                PlayerData.ECharacter type = GameManager.Instance.PlayerData.character;
                currentCharacter = CharacterList[(int)type].Character;
                SwitchPlayer(type);
            }
            else
            {
                currentCharacter = CharacterList[0].Character;
            }
        }

        private void ShowCharacters()
        {
            characterHub.SetActive(true);
        }

        private void HideCharacters()
        {
            characterHub.SetActive(false);
        }

        public bool GetIsInteractingUI()
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = playerInput.mousePosition;

            for (int i = 0; i < raycasters.Length; i++)
            {
                List<RaycastResult> results = new();
                raycasters[i].Raycast(pointerEventData, results);

                for (int j = 0; j < results.Count; j++)
                {
                    int layerMask = 1 << results[j].gameObject.layer;

                    if ((layerMask & uiLayer) != 0)
                        return true;
                }
            }

            return false;
        }

        private void RotatePlayer()
        {
            if (playerInput.pressedLeftMouseButton)
                isInteractingUI = GetIsInteractingUI();

            if (isInteractingUI)
                return;

            if (!playerInput.holdLeftMouseButton)
                return;

            currentCharacter.Rotate(
                0, -playerInput.mouseAxisX * rotationSpeed * Time.deltaTime, 0,
                Space.World
            );
        }

        private void SwitchPlayer(PlayerData.ECharacter type)
        {
            Quaternion previousRotation = currentCharacter.rotation;
            currentCharacter.gameObject.SetActive(false);

            currentCharacter = GetCharacterTransform(type);

            currentCharacter.rotation = previousRotation;
            currentCharacter.gameObject.SetActive(true);
        }

        private Transform GetCharacterTransform(PlayerData.ECharacter type)
        {
            for (int i = 0; i < CharacterList.Count; i++)
            {
                if (CharacterList[i].Type == type)
                    return CharacterList[i].Character;
            }

            return null;
        }

        [System.Serializable]
        public class CharacterInfo
        {
            [field: SerializeField] public PlayerData.ECharacter Type   { get; private set; }
            [field: SerializeField] public Transform Character  { get; private set; }
        }
    }
}