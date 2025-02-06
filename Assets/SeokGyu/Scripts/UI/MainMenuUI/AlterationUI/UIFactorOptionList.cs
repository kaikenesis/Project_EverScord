using UnityEngine;

public class UIFactorOptionList : MonoBehaviour
{
    [SerializeField] private GameObject optionObject;
    [SerializeField] private Transform containor;

    public void Initialize(int count)
    {
        for(int i = 0; i<count;i++)
        {
            Instantiate(optionObject, containor);
        }
    }
}
