using UnityEngine;

namespace EverScord
{
    public class PreventDestruction : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
