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

            if (character.HasState(CharacterState.SKILL_STANCE))
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

        public void SetBool(string name, bool state)
        {
            anim.SetBool(name, state);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC(nameof(SyncSetBool), RpcTarget.Others, name, state);
        }

        public void CrossFade(AnimationClip clip, float transition)
        {
            if (!clip)
                return;

            anim.CrossFade(clip.name, transition, -1, 0f);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC(nameof(SyncCrossFade), RpcTarget.Others, clip.name, transition);
        }

        public void Play(AnimationClip clip)
        {
            if (!clip)
                return;

            Play(clip.name);
        }

        public void Play(string clipName)
        {
            anim.Play(clipName, -1, 0f);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC(nameof(SyncPlay), RpcTarget.Others, clipName);
        }

        public void SetAnimatorSpeed(float speed = 1f)
        {
            anim.speed = speed;
        }

        #region PUN RPC
        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

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

        [PunRPC]
        public void SyncCrossFade(string clipName, float transition)
        {
            anim.CrossFade(clipName, transition, -1, 0f);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
        #endregion
    }
}
