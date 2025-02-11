using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Photon/GameMode",fileName = "gameMode")]
    public class GameMode : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private byte maxPlayers;
        [SerializeField] private bool hasTeams;
        [SerializeField] private int teamSize;

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }
        public byte MaxPlayers
        {
            get { return maxPlayers; }
            private set { maxPlayers = value; }
        }
        public bool HasTeams
        {
            get { return hasTeams; }
            private set { hasTeams = value; }
        }
        public int TeamSize
        {
            get { return teamSize; }
            private set { teamSize = value; }
        }
    }
}
