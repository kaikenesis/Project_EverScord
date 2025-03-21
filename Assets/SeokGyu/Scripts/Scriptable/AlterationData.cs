using System.Collections.Generic;
using UnityEngine;
using static EverScord.AlterationData.PanelData;

namespace EverScord
{
    public class AlterationData : MonoBehaviour
    {
        private List<PanelData> panelDatas = new List<PanelData>();
        public List<PanelData> PanelDatas 
        {
            get { return panelDatas; }
        }

        public AlterationData Init(FactorData[] factorData)
        {
            int count = factorData.Length;
            for (int i = 0; i < count; i++)
            {
                panelDatas.Add(new PanelData(factorData[i]));
            }

            return this;
        }

        public AlterationStatus alterationStatus = new();

        public class PanelData
        {
            public int lastUnlockedNum = 0;

            private int[] optionNum;
            public int[] OptionNum
            {
                get { return optionNum; }
            }

            private float[] value;
            public float[] Value
            {
                get { return value; }
            }

            public PanelData(FactorData factorData)
            {
                int length = factorData.OptionDatas.Length;
                optionNum = new int[length];
                value = new float[length];

                for (int i = 0; i < length; i++)
                {
                    optionNum[i] = -1;
                }
            }

            public class AlterationStatus
            {
                public float[] statusValues { get; private set; }

                public void SetStatus(float[] values)
                {
                    statusValues = values;
                }
            }
        }
    }
}
