using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UISheetOptionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject containor;
        [SerializeField] private TMP_FontAsset optionTextFont;
        [SerializeField] private Color optionTextColor;

        public void SetTypeText(string typeName)
        {
            text.text = typeName;
        }

        public void SetOptionText(TextMeshProUGUI newOption)
        {
            newOption.font = optionTextFont;
            newOption.fontSize = 29;
            newOption.fontStyle = FontStyles.Bold;
            newOption.alignment = TextAlignmentOptions.Left;
            newOption.color = optionTextColor;
        }

        public void AddTextToContainor(GameObject textObj, string text)
        {
            TextMeshProUGUI newOption = textObj.AddComponent<TextMeshProUGUI>();
            SetOptionText(newOption);
            newOption.text = text;

            textObj.transform.SetParent(containor.transform);
        }

        public void SetHeight(int textCount)
        {
            GridLayoutGroup gridLayoutGrop = containor.GetComponent<GridLayoutGroup>();

            float textHeight = gridLayoutGrop.cellSize.y;
            int col = gridLayoutGrop.constraintCount;
            int row = textCount / col;
            if (textCount % col != 0)
                row += 2;

            float h = text.GetComponent<RectTransform>().sizeDelta.y;

            float height = textHeight * row + h;
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        }
    }
}
