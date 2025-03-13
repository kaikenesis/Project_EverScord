using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;
using Photon.Pun;
using System;

namespace EverScord.UI
{
    public class CharacterPodium : MonoBehaviour
    {
        [SerializeField] private GameObject characterHub;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float rotationSpeed;
        [field: SerializeField] public List<CharacterInfo> CharacterList { get; private set; }

        private Transform currentCharacter;
        private InputInfo playerInput;

        public static Action OnChangeCharacter = delegate { };

        private void Awake()
        {
            UIDisplayRoom.OnVisibleObject += HandleVisibleObject;
            SetCharacterTransform();
            HideCharacters();
        }

        void Start()
        {
            //if(!PhotonNetwork.InRoom)
            //{
            //    SetCharacterTransform();
            //    HideCharacters();
            //}
        }

        private void OnDestroy()
        {
            UIDisplayRoom.OnVisibleObject -= HandleVisibleObject;
        }

        private void HandleVisibleObject()
        {
            ShowCharacters();
        }

        void Update()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            playerInput = InputControl.ReceiveInput();
            RotatePlayer();
        }

        private void SetCharacterTransform()
        {
            if (CharacterList.Count == 0)
                return;

            if(GameManager.Instance.PlayerData != null)
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

        private void RotatePlayer()
        {
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

        public void SetCharacterUni()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Uni) return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Uni;
            SwitchPlayer(PlayerData.ECharacter.Uni);
            OnChangeCharacter?.Invoke();
        }

        public void SetCharacterNed()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Ned) return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Ned;
            SwitchPlayer(PlayerData.ECharacter.Ned);
            OnChangeCharacter?.Invoke();
        }

        public void SetCharacterUs()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Us) return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Us;
            SwitchPlayer(PlayerData.ECharacter.Us);
            OnChangeCharacter?.Invoke();
        }

        [System.Serializable]
        public class CharacterInfo
        {
            [field: SerializeField] public PlayerData.ECharacter Type   { get; private set; }
            [field: SerializeField] public Transform Character  { get; private set; }
        }
    }
}