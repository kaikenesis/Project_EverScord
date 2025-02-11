using UnityEngine;
using Photon.Pun;

namespace EverScord.Character
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private AnimationInfo info;
        [SerializeField] private float transitionDampTime;
        [field: SerializeField] public float ShootStanceDuration { get; private set; }

        public Animator Anim => anim;
        public AnimationInfo AnimInfo => info;
        private PhotonView photonView;

        public void Init(PhotonView photonView)
        {
            this.photonView = photonView;
        }

        public void AnimateMovement(CharacterControl character, Vector3 moveDir)
        {
            if (!photonView.IsMine)
                return;

            if (!character.IsMoving)
            {
                anim.SetBool(ConstStrings.PARAM_ISMOVING, false);
                return;
            }

            anim.SetBool(ConstStrings.PARAM_ISMOVING, true);
            anim.SetFloat(ConstStrings.PARAM_HORIZONTAL, moveDir.x, transitionDampTime, Time.deltaTime);
            anim.SetFloat(ConstStrings.PARAM_VERTICAL, moveDir.z, transitionDampTime, Time.deltaTime);
        }

        public void Rotate(bool state)
        {
            if (!photonView.IsMine)
                return;

            anim.SetBool(ConstStrings.PARAM_ISROTATING, state);
        }

        [PunRPC]
        public void SyncSetBool(string name, bool state)
        {
            anim.SetBool(name, state);
        }

        [PunRPC]
        public void SyncPlay(string clipName)
        {
            anim.Play(clipName, -1, 0f);
        }

        public void SetBool(string name, bool state)
        {
            anim.SetBool(name, state);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC("SyncSetBool", RpcTarget.Others, name, state);
        }

        public void Play(AnimationClip clip)
        {
            Play(clip.name);
        }

        public void Play(string clipName)
        {
            anim.Play(clipName, -1, 0f);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC("SyncPlay", RpcTarget.Others, clipName);
        }
    }
}
