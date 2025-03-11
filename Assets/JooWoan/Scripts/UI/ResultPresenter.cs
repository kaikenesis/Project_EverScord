using System.Collections.Generic;
using UnityEngine;

namespace EverScord.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField] private List<ResultUI> resultUIList;
        [SerializeField] private GameObject nedPrefab, uniPrefab, usPrefab;
        [SerializeField] private List<Transform> positionList;        
    }
}
