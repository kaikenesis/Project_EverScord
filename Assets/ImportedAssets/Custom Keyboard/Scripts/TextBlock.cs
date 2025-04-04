using UnityEngine;
using System.Collections;

public class TextBlock : MonoBehaviour
{
    #region Singleton
    public static TextBlock instance;
    TextBlock() { instance = this; }
    #endregion

    public bool isCapslockOn = false;

    public TMPro.TMP_Text textBlock;

    [Range(0.1f, 1f)]
    [SerializeField]
    private float cursorBlinkRate = .25f;

    private void Start()
    {
        StartCoroutine(blinkingCursor());
    }

    IEnumerator blinkingCursor()
    {
        textBlock.text = textBlock.text.Replace("_", "");
        yield return new WaitForSeconds(cursorBlinkRate);
        textBlock.text += "_";
        yield return new WaitForSeconds(cursorBlinkRate);

        StartCoroutine(blinkingCursor());
    }
}

