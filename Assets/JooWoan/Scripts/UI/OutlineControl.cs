using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.UI
{
    public static class OutlineControl
    {
        private static IDictionary<IEnemy, Renderer[]> enemySkinDict = new Dictionary<IEnemy, Renderer[]>();
        private static LayerMask? originalEnemySkinLayer = null;
        private static IEnemy currentOutlinedEnemy = null;

        public static void ResetSkinDictionary()
        {
            enemySkinDict.Clear();
        }

        private static void SetTargetOutline(Renderer[] renderers, LayerMask disabledLayer, LayerMask enabledLayer, bool state)
        {
            LayerMask layerMask = state ? enabledLayer : disabledLayer;
            int layerNumber = Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].gameObject.layer = layerNumber;
            }
        }

        public static void SetCharacterOutline(CharacterControl character, bool state)
        {
            if (character == null)
                return;
            
            SetTargetOutline(character.SkinRenderers, character.OriginalSkinLayer, GameManager.OutlineLayer, state);
        }

        public static void EnableEnemyOutline(PhotonView photonView, IEnemy enemy)
        {
            if (!photonView.IsMine)
                return;
            
            if (enemy == null && enemySkinDict.ContainsKey(enemy))
            {
                enemySkinDict.Remove(enemy);
                return;
            }

            if (enemy == currentOutlinedEnemy)
                return;
            
            DisableEnemyOutline(photonView);

            if (!enemySkinDict.ContainsKey(enemy))
            {
                MonoBehaviour enemyMono = enemy as MonoBehaviour;

                if (!enemyMono)
                    return;

                Renderer[] renderers = enemyMono.transform.GetComponentsInChildren<Renderer>();
                enemySkinDict[enemy] = renderers;

                if (renderers.Length > 0 && originalEnemySkinLayer == null)
                    originalEnemySkinLayer = 1 << renderers[0].gameObject.layer;
            }

            if (enemySkinDict[enemy].Length == 0)
                return;

            if (originalEnemySkinLayer == null)
                return;

            SetTargetOutline(enemySkinDict[enemy], (LayerMask)originalEnemySkinLayer, GameManager.RedOutlineLayer, true);
            currentOutlinedEnemy = enemy;
        }

        public static void DisableEnemyOutline(PhotonView photonView)
        {
            if (!photonView.IsMine)
                return;
            
            if (currentOutlinedEnemy == null || !enemySkinDict.ContainsKey(currentOutlinedEnemy))
                return;

            if (originalEnemySkinLayer == null)
                return;
            
            SetTargetOutline(enemySkinDict[currentOutlinedEnemy], (LayerMask)originalEnemySkinLayer, GameManager.RedOutlineLayer, false);
            currentOutlinedEnemy = null;
        }
    }
}
