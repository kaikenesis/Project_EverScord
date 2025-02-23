using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.UI
{
    public static class OutlineControl
    {
        private static IDictionary<IEnemy, SkinnedMeshRenderer[]> enemySkinDict = new Dictionary<IEnemy, SkinnedMeshRenderer[]>();
        private static LayerMask? originalEnemySkinLayer = null;
        private static IEnemy currentOutlinedEnemy = null;
        public static IEnemy CurrentOutlinedEnemy => currentOutlinedEnemy;

        private static void SetTargetOutline(SkinnedMeshRenderer[] renderers, LayerMask disabledLayer, LayerMask enabledLayer, bool state)
        {
            LayerMask layerMask = state ? enabledLayer : disabledLayer;
            int layerNumber = Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].gameObject.layer = layerNumber;
        }

        public static void SetCharacterOutline(CharacterControl character, bool state)
        {
            SetTargetOutline(character.SkinRenderers, character.OriginalSkinLayer, GameManager.OutlineLayer, state);
        }

        public static void EnableEnemyOutline(PhotonView photonView, IEnemy enemy)
        {
            if (!photonView.IsMine)
                return;
            
            if (enemy == currentOutlinedEnemy)
                return;
            
            DisableEnemyOutline(photonView);

            if (!enemySkinDict.ContainsKey(enemy))
            {
                MonoBehaviour enemyMono = enemy as MonoBehaviour;

                if (!enemyMono)
                    return;

                SkinnedMeshRenderer[] renderers = enemyMono.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
                enemySkinDict[enemy] = renderers;

                if (renderers.Length > 0 && originalEnemySkinLayer == null)
                    originalEnemySkinLayer = 1 << renderers[0].gameObject.layer;
            }

            SetTargetOutline(enemySkinDict[enemy], (LayerMask)originalEnemySkinLayer, GameManager.RedOutlineLayer, true);
            currentOutlinedEnemy = enemy;
        }

        public static void DisableEnemyOutline(PhotonView photonView)
        {
            if (!photonView.IsMine)
                return;
            
            if (currentOutlinedEnemy == null || !enemySkinDict.ContainsKey(currentOutlinedEnemy))
                return;
            
            SetTargetOutline(enemySkinDict[currentOutlinedEnemy], (LayerMask)originalEnemySkinLayer, GameManager.RedOutlineLayer, false);
            currentOutlinedEnemy = null;
        }
    }
}
