using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class AlterationData : MonoBehaviour
    {
        private List<PanelData> panelDatas = new List<PanelData>();
        public List<PanelData> PanelDatas 
        {
            get { return panelDatas; }
        }

        public AlterationData(int panelNum)
        {
            for (int i = 0; i < panelNum; i++)
            {
                panelDatas.Add(new PanelData());
            }
        }

        public class PanelData
        {
            public int lastUnlockedNum = 0;

            private List<int> optionNum = new List<int>();
            public List<int> OptionNum
            {
                get { return optionNum; }
            }

            private List<float> valueNum = new List<float>();
            public List<float> ValueNum
            {
                get { return valueNum; }
            }
        }
    }
}
