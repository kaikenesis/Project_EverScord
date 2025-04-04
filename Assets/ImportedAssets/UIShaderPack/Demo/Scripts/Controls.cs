using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    public GameObject[] pages;
    public GameObject previousButton;
    public GameObject nextButton;
    public Text shaderText;

    [Header("AutoPlay")]
    public bool autoPlay;
    public float timePerPage = 8;
    public RectTransform timer;

    [Header("Animation")]
    public float animateSpeed = 0.5f;
    public Material[] materialsToAnimate;
    public RectTransform[] grabRects;

    private int index;
    private int dir = 1;
    private float currentTime;
    private int pageIndex;

    void Start()
    {
        previousButton.SetActive(false);
        nextButton.SetActive(true);
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == 0);
    }

    private void Update()
    {
        currentTime += dir * Time.deltaTime * animateSpeed;
        if (dir == 1 && currentTime >= 1 || dir == -1 && currentTime <= 0)
            dir *= -1;

        for (int i = 0; i < materialsToAnimate.Length; i++)
            materialsToAnimate[i].SetFloat("_Cutoff", currentTime);

        for (int i = 0; i < grabRects.Length; i++)
            grabRects[i].anchorMax = new Vector2(currentTime, 1);

        if (autoPlay)
        {
            float t = Time.time / timePerPage;
            timer.anchorMax = new Vector2(1, t - pageIndex); 
            if (t > pageIndex + 1 && pageIndex < pages.Length - 1)
            {
                Next();
                pageIndex++;
            }
        }
    }

    public void Next()
    {
        if (index < pages.Length - 1)
        {
            pages[index].SetActive(false);
            pages[index + 1].SetActive(true);
            shaderText.text = pages[index + 1].name;
            if (index == 0)
                previousButton.SetActive(true);
            else if (index == pages.Length - 2)
                nextButton.SetActive(false);

            index++;
        }
    }

    public void Previous()
    {
        if (index > 0)
        {
            pages[index].SetActive(false);
            pages[index - 1].SetActive(true);
            shaderText.text = pages[index - 1].name;
            if (index == 1)
                previousButton.SetActive(false);
            else if (index == pages.Length - 1)
                nextButton.SetActive(true);

            index--;
        }
    }
}
