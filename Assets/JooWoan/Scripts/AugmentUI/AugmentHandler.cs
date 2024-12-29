using EverScord.Armor;
using UnityEngine;

public class AugmentHandler : MonoBehaviour
{
    [SerializeField] private AugmentUI augmentUI;
    private AugmentData augmentData;

    void Start()
    {
        augmentData.Init();
        
    }

    void OnClick()
    {
        
    }
}
