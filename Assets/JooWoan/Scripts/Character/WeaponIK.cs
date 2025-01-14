using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Character
{
    public class WeaponIK : MonoBehaviour
    {
        [SerializeField] private Transform leftHand, rightHand;
        private Animator anim;
        private int weaponLayerIndex;

        void Start()
        {
            anim = GetComponent<Animator>();
            weaponLayerIndex = anim.GetLayerIndex("Weapon");
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != weaponLayerIndex)
                return;

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);

            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);

            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
        }
    }
}
