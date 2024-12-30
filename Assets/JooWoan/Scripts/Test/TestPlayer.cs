using EverScord.Armor;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public IHelmet helmet { get; private set; }

    void Start()
    {
        helmet = new Helmet(10, 10, 10, 10, 10);
    }

    public void SetHelmet(IHelmet newHelmet)
    {
        helmet = newHelmet;
    }
}
