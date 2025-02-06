using UnityEngine;

public class UIFactor : MonoBehaviour
{
    [SerializeField] private UIFactorSlot.EType factorType;
    [SerializeField] private GameObject factor;
    [SerializeField] private Transform containor;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private int slotCount;
    [SerializeField] private int confirmedCount;

    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        for (int i = 0; i < slotCount; i++)
        {
            bool bConfirmed = false;
            if (i < confirmedCount)
                bConfirmed = true;

            GameObject obj = Instantiate(factor, containor);
            obj.GetComponent<UIFactorSlot>().Initialize(factorType, bConfirmed, i);
            obj.SetActive(true);
        }
    }

    public void OnClickedSlot(UIFactorSlot factorSlot)
    {
        if(factorSlot.slotNum < confirmedCount)
        {
            optionPanel.SetActive(true);
        }
        else
        {
            if(factorSlot.bLock == true)
            {
                Debug.Log("Lock");
            }
            else
            {
                Debug.Log("UnLock");
            }
        }
    }
}
