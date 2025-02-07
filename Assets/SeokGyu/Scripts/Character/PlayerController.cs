using Photon.Pun;
using UnityEngine;

namespace EverScord
{
    public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
    {
        // 마우스가 있는 방향으로 캐릭터 회전
        // 좌클릭,우클릭 공격
        // Q,R 스킬 공격 -> 직군별 스킬변화
        // 재장전 E or 장탄수 모두 소모시 자동으로 수행
        [SerializeField] private GameObject character;
        //private Collider collider;
        //private CharacterController controller;
        private Rigidbody rigidBody;
        private Animator animator;
        //private Camera camera;

        private PhotonView photonView;

        private float moveSpeed = 30.0f;
        //private float rotSpeed = 1.0f;
        protected Vector3 remotePos;
        protected Quaternion remoteRot;

        #region Private Methods

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            //controller = character.GetComponent<CharacterController>();
            //collider = character.GetComponent<Collider>();
            rigidBody = character.GetComponentInChildren<Rigidbody>();
            animator = character.GetComponentInChildren<Animator>();
            //camera = Camera.main;

            photonView = character.GetComponentInChildren<PhotonView>();
        }

        private void Start()
        {
            SetInfo();
        }

        private void SetInfo()
        {

        }

        private void Update()
        {
            InputKey();
        }

        private void InputKey()
        {
            if (photonView.IsMine)
            {
                Move();
            }
        }

        private void Move()
        {
            float speed = moveSpeed * Time.deltaTime;
            Vector3 dir;
            float forward = 0;
            float right = 0;

            if (Input.GetKey(KeyCode.W))
            {
                forward += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                right -= 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                forward -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                right += 1;
            }
            dir = new Vector3(right, 0.0f, forward);
            dir = dir.normalized;
            dir *= speed;

            //controller.SimpleMove(dir * 50);
            rigidBody.position = Vector3.MoveTowards(rigidBody.position, rigidBody.position + dir, 1.0f);
            //rigidBody.MovePosition(dir);
            //transform.Translate(dir);
            if (false == photonView.IsMine)
            {
                transform.position = Vector3.Lerp(transform.position, remotePos, 10 * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
                return;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 내가 데이터를 보내는 중이라면
            if (stream.IsWriting) // 내꺼 보내는 거
            {
                // 이 방안에 있는 모든 사용자에게 브로드캐스트 
                // - 내 포지션 값을 보내보자
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            // 내가 데이터를 받는 중이라면 
            else // 원격에 있는 나 
            {
                // 순서대로 보내면 순서대로 들어옴. 근데 타입캐스팅 해주어야 함
                remotePos = (Vector3)stream.ReceiveNext();
                remoteRot = (Quaternion)stream.ReceiveNext();
            }
        }
        #endregion

        #region Public Methods
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameManager.Instance.AddPlayerPhotonView(info.photonView);
        }
        #endregion
    }
}
