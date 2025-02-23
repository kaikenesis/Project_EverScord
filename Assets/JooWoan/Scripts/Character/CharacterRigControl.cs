using UnityEngine;
using UnityEngine.Animations.Rigging;
using EverScord.Weapons;

namespace EverScord.Character
{
    public class CharacterRigControl : MonoBehaviour
    {
        [field: SerializeField] public MultiAimConstraint BodyAim       { get; private set; }
        [field: SerializeField] public MultiAimConstraint HeadAim       { get; private set; }
        [field: SerializeField] public MultiAimConstraint Aim           { get; private set; }
        [field: SerializeField] public TwoBoneIKConstraint LeftHandIK   { get; private set; }
        public RigBuilder Builder                                       { get; private set; }
        private Rig mainRig;

        public void Init(Transform root, Animator anim, Weapon weapon)
        {
            mainRig = GetComponent<Rig>();

            Builder = root.GetComponent<RigBuilder>();
            Builder.layers.Clear();
            Builder.layers.Add(new RigLayer(GetComponent<Rig>(), true));

            BodyAim.data.constrainedObject  = anim.GetBoneTransform(HumanBodyBones.Chest);
            HeadAim.data.constrainedObject  = anim.GetBoneTransform(HumanBodyBones.Head);
            LeftHandIK.data.root            = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            LeftHandIK.data.mid             = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            LeftHandIK.data.tip             = anim.GetBoneTransform(HumanBodyBones.LeftHand);

            Aim.data.constrainedObject      = weapon.WeaponTransform.parent;

            if (weapon.LeftTarget)
            {
                LeftHandIK.data.target = weapon.LeftTarget;
                LeftHandIK.data.hint = weapon.LeftHint;
            }

            MultiAimConstraint[] constraints = { Aim, BodyAim, HeadAim };

            for (int i = 0; i < constraints.Length; i++)
            {
                var data = constraints[i].data.sourceObjects;

                data.Clear();
                data.Add(new WeightedTransform(weapon.AimPoint, 1));

                constraints[i].data.sourceObjects = data;
            }

            Builder.Build();
        }

        public void SetAimWeight(bool state)
        {
            float result = state ? 1f : 0f;
            Aim.weight = result;
            LeftHandIK.weight = result;
        }

        public void SetMainRigWeight(bool state)
        {
            float result = state ? 1f : 0f;
            mainRig.weight = result;
        }
    }
}
