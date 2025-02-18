using UnityEngine;

namespace EverScord
{
    public interface IAction
    {
        public enum EType
        {
            Action1,
            Action2,
            Action3,
            Action4
        }

        public void DoAction(EType type);
    }
}
