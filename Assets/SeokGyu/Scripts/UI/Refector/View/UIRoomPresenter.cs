using UnityEngine;

namespace EverScord
{
    public class UIRoomPresenter : ToggleObject
    {
        //[SerializeField] private UIRoomModel model;
        [SerializeField] private UIRoomView view;

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();


        }
    }
}
