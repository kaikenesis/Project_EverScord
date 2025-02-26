using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;
using Photon.Pun;

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

        void Start()
        {
            SetCharacterTransform();
            HideCharacters();
            UIDisplayRoom.OnVisibleObject += HandleVisibleObject;
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

            currentCharacter = CharacterList[0].Character;
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

        private void SwitchPlayer(CharacterType type)
        {
            Quaternion previousRotation = currentCharacter.rotation;
            currentCharacter.gameObject.SetActive(false);

            currentCharacter = GetCharacterTransform(type);

            currentCharacter.rotation = previousRotation;
            currentCharacter.gameObject.SetActive(true);
        }

        private Transform GetCharacterTransform(CharacterType type)
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
            SwitchPlayer(CharacterType.UNI);
        }

        public void SetCharacterNed()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Ned) return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Ned;
            SwitchPlayer(CharacterType.NED);
        }

        public void SetCharacterUs()
        {
            if (GameManager.Instance.PlayerData.character == PlayerData.ECharacter.Us) return;

            GameManager.Instance.PlayerData.character = PlayerData.ECharacter.Us;
            SwitchPlayer(CharacterType.US);
        }

        public enum CharacterType
        {
            NED,
            UNI,
            US
        }

        [System.Serializable]
        public class CharacterInfo
        {
            [field: SerializeField] public CharacterType Type   { get; private set; }
            [field: SerializeField] public Transform Character  { get; private set; }
        }
    }
}