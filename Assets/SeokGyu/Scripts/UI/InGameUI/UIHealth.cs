using UnityEngine;

namespace EverScord
{
    public class UIHealth : UIProgress
    {
        [Range(0.0f,1.0f)]
        [SerializeField] private float value;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                UpdateFillAmount(value);
            }
        }
    }
}

