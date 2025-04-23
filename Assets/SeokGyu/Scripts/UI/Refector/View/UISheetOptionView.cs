using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UISheetOptionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject containor;

        public void SetTypeText(string typeName)
        {
            text.text = typeName;
        }

        public void AddTextToContainor(GameObject textObj)
        {
            textObj.transform.SetParent(containor.transform);
        }

        public void SetHeight(int textCount, float textHeight)
        {
            int col = containor.GetComponent<GridLayoutGroup>().constraintCount;
            int row = textCount / col;
            if (textCount % col != 0)
                row += 2;

            float h = text.GetComponent<RectTransform>().sizeDelta.y;

            //float height = h * 3 + count / 2 * h;
            float height = textHeight * row + h;
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        }
    }
}
