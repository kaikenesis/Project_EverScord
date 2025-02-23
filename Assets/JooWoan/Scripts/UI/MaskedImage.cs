using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace EverScord.UI
{
    public class MaskedImage : Image
    {
        public override Material materialForRendering
        { 
            get
            {
                Material material = new Material(base.materialForRendering);

                // Render this image where the stencil buffer is not equal to the reference value
                material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return material;
            }
        }
        protected override void Start()
        {
            base.Start();
            StartCoroutine(Fix());
        }

        // Fix for async loading scenes
        // Disabling and enabling the mask to apply properly?
        private IEnumerator Fix()
        {
            yield return null;
            maskable = false;
            maskable = true;
        }
    }
}
