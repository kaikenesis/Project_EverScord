using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace EverScord.Character
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private AnimationInfo info;
        [SerializeField] private float transitionDampTime;
        [SerializeField] private float lerpMaskSpeed = 1f;
        [field: SerializeField] public float ShootStanceDuration { get; private set; }
        public Animator Anim => anim;
        public AnimationInfo AnimInfo => info;
        private PhotonView photonView;
        private Coroutine lerpMaskCoroutine;

        private int upperMaskLayerIndex;

        void OnEnable()
        {
            SetUpperMask(false, true);
        }

        public void Init(PhotonView photonView)
        {
            this.photonView = photonView;
            upperMaskLayerIndex = anim.GetLayerIndex(ConstStrings.ANIMLAYER_UPPERMASK);
            SetUpperMask(false, true);
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

        public void SetUpperMask(bool state, bool isImmediate = false)
        {
            if (lerpMaskCoroutine != null)
                StopCoroutine(lerpMaskCoroutine);

            if (state)
                anim.SetLayerWeight(upperMaskLayerIndex, 1f);

            else if (!isImmediate)
                lerpMaskCoroutine = StartCoroutine(LerpOutUpperMask());

            else
                anim.SetLayerWeight(upperMaskLayerIndex, 0f);
        }

        public void SetUpperMask(float weight)
        {
            weight = Mathf.Clamp(weight, 0f, 1f);
            anim.SetLayerWeight(upperMaskLayerIndex, weight);
        }

        private IEnumerator LerpOutUpperMask()
        {
            anim.SetLayerWeight(upperMaskLayerIndex, 1f);

            for (float t = 1f; anim.GetLayerWeight(upperMaskLayerIndex) >= 0f; t -= Time.deltaTime * lerpMaskSpeed)
            {
                anim.SetLayerWeight(upperMaskLayerIndex, t);
                yield return null;
            }

            anim.SetLayerWeight(upperMaskLayerIndex, 0f);
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

        public void SetAnimatorEnabled(bool state)
        {
            anim.enabled = state;
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
