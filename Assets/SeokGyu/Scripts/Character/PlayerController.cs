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
