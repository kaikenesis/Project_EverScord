using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Keys : MonoBehaviour
{
    [SerializeField] private string keyValue = "";

    private TMPro.TMP_Text text;

    private Image image;

    private AudioSource keyAudioSFX;

    private void Start()
    {
        image = GetComponent<Image>();
        text = TextBlock.instance.textBlock;

        keyAudioSFX = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        OnKeyPressed();
    }

    public void OnKeyPressed()
    {
        keyAudioSFX.Play();

        if (keyValue == "back")
        {
            text.text = text.text.Replace("_", "");
            text.text = text.text.Remove(text.text.Length - 1);
        }
        else if (keyValue == "enter") //change line + ()
        {
            text.text += "\n";

            /// Add your code here, if you want to perform specific actions.
        }
        else if (keyValue == "capslock")
        {
            if (TextBlock.instance.isCapslockOn)
                TextBlock.instance.isCapslockOn = false;
            else
                TextBlock.instance.isCapslockOn = true;
        }
        else
        {
            text.text = text.text.Replace("_", "");
            if (TextBlock.instance.isCapslockOn)
            {
                text.text += keyValue.ToUpper();
            }
            else
            {
                text.text += keyValue;
            }
        }

        StartCoroutine(on_key_pressed());
    }

    IEnumerator on_key_pressed()
    {
        image.color = Color.grey;

        yield return new WaitForSeconds(.15f);

        image.color = Color.white;
    }
}
