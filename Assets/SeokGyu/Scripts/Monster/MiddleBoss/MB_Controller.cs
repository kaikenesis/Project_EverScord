using System.Collections;
using UnityEngine;

namespace EverScord
{
    public class MB_Controller : MonoBehaviour
    {
        private float curHealth;
        [SerializeField] private float maxDistance = 50.0f;
        [SerializeField] private float lagerRotSpeed = 10.0f;
        [SerializeField] private Transform[] lager;
        private BoxCollider[] colliders;
        private RaycastHit[] hit;
        private float delay = 0.01f;

        public float CurHealth
        {
            get { return curHealth; }
            protected set { curHealth = value; }
        }

        private void Awake()
        {
            colliders = new BoxCollider[lager.Length];
            hit = new RaycastHit[lager.Length];

            for (int i = 0; i < lager.Length; i++)
            {
                colliders[i] = lager[i].gameObject.GetComponent<BoxCollider>();
                colliders[i].size = new Vector3(maxDistance, colliders[i].size.y, colliders[i].size.z);
                colliders[i].center = new Vector3(maxDistance / 2.0f, colliders[i].center.y, colliders[i].center.z);
            }
            StartCoroutine(Pattern2());

            Lager.OnEnter += HandleEnter;
        }

        private void OnDestroy()
        {
            Lager.OnEnter -= HandleEnter;
        }

        public void HandleEnter()
        {
            Debug.Log("OnTrigger");
        }

        private IEnumerator Pattern2()
        {
            while(true)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    Physics.Raycast(transform.position, lager[i].right * maxDistance, out hit[i], maxDistance);
                    Debug.DrawRay(transform.position, lager[i].right * maxDistance, Color.blue, delay);

                    lager[i].RotateAround(transform.position, new Vector3(0, 1, 0), lagerRotSpeed * Time.deltaTime);
                }

                yield return new WaitForSeconds(delay);
            }
        }
    }
}
