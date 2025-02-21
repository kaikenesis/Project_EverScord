using UnityEngine;

namespace EverScord
{
    [CreateAssetMenu(menuName = "EverScord/Datas/PhotonData", fileName = "newPhotonData")]
    public class PhotonData : MonoBehaviour
    {
        public enum EState
        {
            NONE,
            MATCH,
            STOPMATCH,
            FOLLOW,
            MAX
        }

        public EState state = EState.NONE;
    }
}
    
