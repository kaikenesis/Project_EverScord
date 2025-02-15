using System.Collections;
using UnityEngine;

namespace EverScord
{
    public class MB_Controller : MonoBehaviour, IStatus
    {
        private float curHealth;
        [SerializeField] private MiddleBossData data;
        [SerializeField] private Transform Mesh;
        [SerializeField] private Transform[] lager;
        [SerializeField] private Transform lagerPatternPos;
        private RaycastHit[] hit;
        private float countTime = 0.0f;
        private bool bPlaylagerPattern = false;

        public float CurHealth
        {
            get { return curHealth; }
            protected set { curHealth = value; }
        }

        private void Awake()
        {
            hit = new RaycastHit[lager.Length];

            for (int i = 0; i < lager.Length; i++)
            {
                lager[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            CountSkillTime();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(LagerPattern());
            }
        }

        private void CountSkillTime()
        {
            if (bPlaylagerPattern)
            {
                countTime += Time.deltaTime;
                if (countTime >= data.LagerPatternPlayTime)
                {
                    for (int i = 0; i < lager.Length; i++)
                    {
                        lager[i].gameObject.SetActive(false);
                    }
                    countTime = 0.0f;
                    bPlaylagerPattern = false;
                }
                Debug.Log(countTime);
            }
        }

        private IEnumerator LagerPattern()
        {
            if (bPlaylagerPattern == true) yield break;

            bPlaylagerPattern = true;
            for (int i = 0; i < lager.Length; i++)
            {
                lager[i].gameObject.SetActive(true);
            }

            float activeTime = data.LagerCastTime + data.LagerActivateTime;

            while (bPlaylagerPattern)
            {
                if (countTime <= data.LagerCastTime)
                {
                    // 땅속으로 숨고 다니 나타나는 애니메이션 재생 및 캐릭터 이동
                    transform.SetPositionAndRotation(lagerPatternPos.position, lagerPatternPos.rotation);
                    RotateLager(false);
                }
                else if (countTime <= activeTime)
                {
                    RotateLager(true);
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }

            InitLagerPattern();
        }

        private void RotateLager(bool bRotate)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                Vector3 offset = lager[i].right * data.LagerDistOffset + Mesh.localPosition;

                if (Physics.Raycast(offset, lager[i].right * data.LagerMaxDistance, out hit[i], data.LagerMaxDistance))
                {
                    Vector3 newPos = lager[i].right * hit[i].distance / 2.0f + Mesh.localPosition;

                    lager[i].localScale = new Vector3(hit[i].distance, lager[i].localScale.y, lager[i].localScale.z);
                    lager[i].localPosition = newPos + offset;
                }
                else
                {
                    Vector3 newPos = lager[i].right * data.LagerMaxDistance / 2.0f + Mesh.localPosition;

                    lager[i].localScale = new Vector3(data.LagerMaxDistance, lager[i].localScale.y, lager[i].localScale.z);
                    lager[i].localPosition = newPos + offset;
                }

                Debug.DrawRay(offset, lager[i].right * data.LagerMaxDistance, Color.blue, Time.deltaTime);
                if (i == 0)
                    Debug.DrawRay(offset, lager[i].right * data.LagerMaxDistance, Color.red, Time.deltaTime);

                if(bRotate == true)
                    lager[i].RotateAround(Mesh.position, new Vector3(0, 1, 0), data.LagerRotSpeed * Time.deltaTime);
            }
        }

        private void InitLagerPattern()
        {
            Vector3 initPos = new Vector3(0, 0, 0);
            for (int i = 0; i < lager.Length; i++)
            {
                lager[i].SetLocalPositionAndRotation(initPos, Quaternion.Euler(0, i * 90, 0));
            }
        }

        public void TakeDamage(GameObject sender, float damage)
        {
            Debug.Log($"{name} || Sender : {sender.name}, Damage : {damage}");
        }
    }
}
