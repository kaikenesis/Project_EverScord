using UnityEngine;
using UnityEngine.Animations.Rigging;
using EverScord.Weapons;
using System.Collections;

namespace EverScord.Character
{
    public class CharacterRigControl : MonoBehaviour
    {
        [field: SerializeField] public MultiAimConstraint BodyAim       { get; private set; }
        [field: SerializeField] public MultiAimConstraint HeadAim       { get; private set; }
        [field: SerializeField] public MultiAimConstraint Aim           { get; private set; }
        [field: SerializeField] public TwoBoneIKConstraint LeftHandIK   { get; private set; }
        public RigBuilder Builder                                       { get; private set; }

        private WaitForSeconds wait = new WaitForSeconds(0.1f);

        public void Init(Transform root, Animator anim, Weapon weapon)
        {
            Builder = root.GetComponent<RigBuilder>();
            Builder.layers.Clear();
            Builder.layers.Add(new RigLayer(GetComponent<Rig>(), true));

            BodyAim.data.constrainedObject  = anim.GetBoneTransform(HumanBodyBones.Chest);
            HeadAim.data.constrainedObject  = anim.GetBoneTransform(HumanBodyBones.Head);
            LeftHandIK.data.root            = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            LeftHandIK.data.mid             = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            LeftHandIK.data.tip             = anim.GetBoneTransform(HumanBodyBones.LeftHand);

            Aim.data.constrainedObject      = weapon.transform.parent;
            LeftHandIK.data.target          = weapon.LeftTarget;
            LeftHandIK.data.hint            = weapon.LeftHint;
        }

        public void SetAimWeight(bool forceRig)
        {
            float result = forceRig ? 1f : 0f;
            Aim.weight = result;
            LeftHandIK.weight = result;
        }
    }
}
