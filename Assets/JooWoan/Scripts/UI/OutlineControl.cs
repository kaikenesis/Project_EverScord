using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.UI
{
    public static class OutlineControl
    {
        private static IDictionary<IEnemy, EnemyOutlineInfo> enemySkinDict = new Dictionary<IEnemy, EnemyOutlineInfo>();
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
                enemySkinDict[enemy] = null;

                MonoBehaviour enemyMono = enemy as MonoBehaviour;

                if (!enemyMono)
                    return;

                Renderer[] renderers = enemyMono.transform.GetComponentsInChildren<Renderer>();

                if (renderers.Length > 0)
                {
                    LayerMask enemySkinLayer = 1 << renderers[0].gameObject.layer;
                    EnemyOutlineInfo info = new EnemyOutlineInfo(renderers, enemySkinLayer);
                    enemySkinDict[enemy] = info;
                }
            }

            if (enemySkinDict[enemy] == null)
                return;

            if (enemySkinDict[enemy].Renderers.Length == 0)
                return;

            SetTargetOutline(enemySkinDict[enemy].Renderers, enemySkinDict[enemy].SkinLayer, GameManager.RedOutlineLayer, true);
            currentOutlinedEnemy = enemy;
        }

        public static void DisableEnemyOutline(PhotonView photonView)
        {
            if (!photonView.IsMine)
                return;
            
            if (currentOutlinedEnemy == null)
                return;

            if (!enemySkinDict.ContainsKey(currentOutlinedEnemy))
                return;

            if (enemySkinDict[currentOutlinedEnemy] == null)
                return;

            if (enemySkinDict[currentOutlinedEnemy].Renderers.Length == 0)
                return;

            SetTargetOutline(enemySkinDict[currentOutlinedEnemy].Renderers, enemySkinDict[currentOutlinedEnemy].SkinLayer, GameManager.RedOutlineLayer, false);
            currentOutlinedEnemy = null;
        }
    }

    public class EnemyOutlineInfo
    {
        public Renderer[] Renderers;
        public LayerMask SkinLayer;

        public EnemyOutlineInfo(Renderer[] renderers, LayerMask skinLayer)
        {
            Renderers = renderers;
            SkinLayer = skinLayer;
        }
    }
}
