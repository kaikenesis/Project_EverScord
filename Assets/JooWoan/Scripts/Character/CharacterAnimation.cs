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

            if (character.HasState(CharState.SKILL_STANCE))
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

        public void CrossFade(AnimationParam param)
        {
            anim.CrossFade(param.ClipName, param.Transition, param.Layer, param.NormalizedTimeOffset, param.NormalizedTransitionTime);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC(nameof(SyncCrossFade), RpcTarget.Others, param.ClipName, param.Transition, param.Layer, param.NormalizedTimeOffset, param.NormalizedTransitionTime);
        }

        public void Play(AnimationClip clip, int layer = -1, float normalizedTime = 0f)
        {
            if (!clip)
                return;

            Play(clip.name, layer, normalizedTime);
        }

        public void Play(string clipName, int layer = -1, float normalizedTime = 0f)
        {
            anim.Play(clipName, layer, normalizedTime);

            if (!PhotonNetwork.IsConnected || !photonView.IsMine)
                return;

            photonView.RPC(nameof(SyncPlay), RpcTarget.Others, clipName, layer, normalizedTime);
        }

        public void SetAnimatorSpeed(float speed = 1f)
        {
            anim.speed = speed;
        }

        #region PUN RPC
        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////////////////////////////////////

        [PunRPC]
        public void SyncSetBool(string name, bool state)
        {
            anim.SetBool(name, state);
        }

        [PunRPC]
        public void SyncPlay(string clipName, int layer, float normalizedTime)
        {
            anim.Play(clipName, layer, normalizedTime);
        }

        [PunRPC]
        public void SyncCrossFade(string clipName, float transition, int layer, float normalizedTimeOffset, float normalizedTransitionTime)
        {
            anim.CrossFade(clipName, transition, layer, normalizedTimeOffset, normalizedTransitionTime);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////////////////////////////////////
        #endregion
    }

    public struct AnimationParam
    {
        public string ClipName;
        public float Transition;
        public int Layer;
        public float NormalizedTimeOffset;
        public float NormalizedTransitionTime;

        public AnimationParam(string clipName, float transition, int layer = -1, float normalizedTimeOffset = 0f, float normalizedTransitionTime = 0f)
        {
            ClipName = clipName;
            Transition = transition;
            Layer = layer;
            NormalizedTimeOffset = normalizedTimeOffset;
            NormalizedTransitionTime = normalizedTransitionTime;
        }
    }
}
