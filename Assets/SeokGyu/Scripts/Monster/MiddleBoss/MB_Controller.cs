using EverScord.Character;
using EverScord.Effects;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EverScord
{
    public class MB_Controller : MonoBehaviour, IPunObservable, IEnemy, IAction
    {
        [SerializeField] private MiddleBossData data;
        [SerializeField] private Transform Mesh;
        [SerializeField] private GameObject laserObject;
        [SerializeField] private Animator animator;
        private List<GameObject> lasers = new List<GameObject>();
        private List<BoxCollider> colliders = new List<BoxCollider>();
        private List<Transform> hitPoints = new List<Transform>();
        private RaycastHit[] hit;
        private float countTime = 0.0f;
        private bool bPlaylagerPattern = false;
        private MB_BTRunner btRunner;

        // Photon Sync
        private Vector3[] remoteHitPointPos;
        private Quaternion[] remoteLaserRot;
        private bool remoteDig;
        private bool remotePattern2;
        private bool remoteActiveRaser;
        private float remoteCurHealth;

        public BodyType EnemyBodyType => BodyType.FLESH;

        private void Awake()
        {
            for (int i = 0; i < data.LaserCount; i++)
            {
                GameObject obj = Instantiate(laserObject, Mesh);
                lasers.Add(obj);

                BoxCollider box = obj.GetComponent<BoxCollider>();
                colliders.Add(box);

                MouseTargetV3D mouseTarget = obj.GetComponent<MouseTargetV3D>();
                hitPoints.Add(mouseTarget.targetCursor);
            }

            hit = new RaycastHit[lasers.Count];
            remoteHitPointPos = new Vector3[lasers.Count];
            remoteLaserRot = new Quaternion[lasers.Count];

            InitLagerPattern();
            data.Init();

            Vector3 colliderSize = new Vector3(1, 1, data.LaserMaxDistance);
            Vector3 colliderCenter = new Vector3(0, 0, data.LaserMaxDistance / 2 + 1);

            for (int i = 0; i < lasers.Count; i++)
            {
                colliders[i].size = colliderSize;
                colliders[i].center = colliderCenter;
            }

            btRunner = GetComponent<MB_BTRunner>();
        }

        private void Update()
        {
            if(PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                PhotonSync();
            }
            else
            {
                CountSkillTime();
            }

            DebugInput();
        }

        private void InitLagerPattern()
        {
            Vector3 initPos = new Vector3(0, 0, 0);
            int angle = 360 / lasers.Count;
            for (int i = 0; i < lasers.Count; i++)
            {
                lasers[i].transform.SetLocalPositionAndRotation(initPos, Quaternion.Euler(0, i * angle, 0));
                lasers[i].gameObject.SetActive(false);
            }

            animator.SetBool("bPattern2", false);
        }

        private void Move()
        {
            // 근접한 대상 추격, MasterClinet만 계산하고 결과값을 클라이언트에 전달해서 업데이트
        }

        private void CountSkillTime()
        {
            if (bPlaylagerPattern)
            {
                countTime += Time.deltaTime;
                if (countTime >= data.LaserPatternPlayTime - data.LaserCastTime)
                {
                    countTime = 0.0f;
                    bPlaylagerPattern = false;
                }
                //Debug.Log(countTime);
            }
        }

        private IEnumerator LagerPattern()
        {
            if (bPlaylagerPattern == true) yield break;

            bPlaylagerPattern = true;
            for (int i = 0; i < lasers.Count; i++)
            {
                lasers[i].gameObject.SetActive(true);
            }

            while (bPlaylagerPattern)
            {
                RotateLager(true);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            InitLagerPattern();
        }

        private void RotateLager(bool bRotate)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                Vector3 offset = lasers[i].transform.forward * data.LaserDistOffset + Mesh.localPosition;
                Vector3 colliderOffset = new Vector3(0, 0, 1);

                if (Physics.Raycast(offset, lasers[i].transform.forward * data.LaserMaxDistance, out hit[i], data.LaserMaxDistance))
                {
                    hitPoints[i].position = hit[i].point;
                }
                else
                {
                    hitPoints[i].position = lasers[i].transform.forward * data.LaserMaxDistance;
                }

                Debug.DrawRay(offset, lasers[i].transform.forward * data.LaserMaxDistance, Color.blue, Time.deltaTime);
                if (i == 0)
                    Debug.DrawRay(offset, lasers[i].transform.forward * data.LaserMaxDistance, Color.red, Time.deltaTime);

                if(bRotate == true)
                    lasers[i].transform.RotateAround(Mesh.position, new Vector3(0, 1, 0), data.LaserRotSpeed * Time.deltaTime);
            }
        }

        private void PhotonSync()
        {
            for (int i = 0; i < lasers.Count; i++)
            {
                hitPoints[i].position = Vector3.Lerp(hitPoints[i].position, remoteHitPointPos[i], 10 * Time.deltaTime);
                lasers[i].transform.rotation = Quaternion.Lerp(lasers[i].transform.rotation, remoteLaserRot[i], 10 * Time.deltaTime);
                
                lasers[i].gameObject.SetActive(remoteActiveRaser);
            }

            animator.SetBool("bPattern2", remotePattern2);
            animator.SetBool("bDig", remoteDig);

            data.curHealth = remoteCurHealth;
            btRunner.UpdatePhase();
        }

        public void DoAction(IAction.EType type)
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

            switch(type)
            {
                case IAction.EType.Action1:
                    {
                        if (animator.GetBool("bPattern2") == false)
                        {
                            animator.SetBool("bPattern2", true);
                            animator.SetBool("bChase", false);
                        }
                        else
                            StartCoroutine(LagerPattern());
                    }
                    break;
                case IAction.EType.Action2:
                    {
                        if (animator.GetBool("bPattern3") == false)
                        {
                            animator.SetBool("bPattern3", true);
                            animator.SetBool("bChase", false);
                        }
                    }
                    break;
                case IAction.EType.Action3:
                    {
                        if (animator.GetBool("bPattern4") == false)
                        {
                            animator.SetBool("bPattern4", true);
                            animator.SetBool("bChase", false);
                        }
                    }
                    break;
                case IAction.EType.Action4:
                    {
                        if (animator.GetBool("bPattern5") == false)
                        {
                            animator.SetBool("bPattern5", true);
                            animator.SetBool("bChase", false);
                        }
                    }
                    break;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                for (int i = 0; i < lasers.Count; i++)
                {
                    stream.SendNext(hitPoints[i].position);
                    stream.SendNext(lasers[i].transform.rotation);
                }
                stream.SendNext(lasers[0].gameObject.activeSelf);

                stream.SendNext(animator.GetBool("bDig"));
                stream.SendNext(animator.GetBool("bPattern2"));

                stream.SendNext(data.curHealth);
            }
            else
            {
                for (int i = 0; i < lasers.Count; i++)
                {
                    remoteHitPointPos[i] = (Vector3)stream.ReceiveNext();
                    remoteLaserRot[i] = (Quaternion)stream.ReceiveNext();
                }
                remoteActiveRaser = (bool)stream.ReceiveNext();

                remoteDig = (bool)stream.ReceiveNext();
                remotePattern2 = (bool)stream.ReceiveNext();

                remoteCurHealth = (float)stream.ReceiveNext();
            }
        }

        public void TestDamage(GameObject sender, float value)
        {
            Debug.Log($"{name} || Sender : {sender.name}, Damage : {value}");

            data.curHealth -= value;
            btRunner.UpdatePhase();
        }

        public void DecreaseHP(float hp, CharacterControl attacker)
        {
            throw new System.NotImplementedException();
        }

        public void StunMonster(float stunTime)
        {
            throw new System.NotImplementedException();
        }

        private void DebugInput()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                TestDamage(this.gameObject, 30.0f);
            }
        }

        public BlinkEffect GetBlinkEffect()
        {
            throw new System.NotImplementedException();
        }

        public float GetDefense()
        {
            throw new System.NotImplementedException();
        }

        public void SetDebuff(CharacterControl attacker, EBossDebuff debuffState, float time, float value)
        {
            throw new System.NotImplementedException();
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            throw new System.NotImplementedException();
        }
    }
}
