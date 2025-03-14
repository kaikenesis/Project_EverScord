using UnityEngine;

namespace EverScord
{
    public class UITutorial : ToggleObject
    {
        private int curPage = 0;
        [SerializeField] private GameObject[] pages;

        public void OnClickedNextPage()
        {
            if (curPage >= pages.Length - 1) return;

            pages[curPage++].SetActive(false);
            pages[curPage].SetActive(true);
        }

        public void OnClickedPrevPage()
        {
            if (curPage <= 0) return;

            pages[curPage--].SetActive(false);
            pages[curPage].SetActive(true);
        }

        public override void OnActivateObjects()
        {
            if(curPage != 0)
            {
                pages[curPage].SetActive(false);
                curPage = 0;
                pages[curPage].SetActive(true);
            }
            
            base.OnActivateObjects();
        }
    }
}
