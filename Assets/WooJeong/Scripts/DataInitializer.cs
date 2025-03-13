using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EverScord;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private List<ScriptableObject> datas = new();
    private AlterationParsedData alterationParsedData;
    private MonsterData monsterData;
    private StatData statData;
    private SkillData skillData;

    private void Awake()
    {
        alterationParsedData = new AlterationParsedData();
        monsterData = new MonsterData();
        statData = new StatData();
        skillData = new SkillData();

        alterationParsedData.Init();
        monsterData.Init();
        statData.Init();
        skillData.Init();

        Initialize();
    }

    private void Initialize()
    {
        foreach(var data in datas)
        {
            var idata = data as IData;
            idata.Init();
        }
    }
}
