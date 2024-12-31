using UnityEngine;

namespace EverScord
{
    public class PlayerController : MonoBehaviour
    {
        // 마우스가 있는 방향으로 캐릭터 회전
        // 좌클릭,우클릭 공격
        // Q,R 스킬 공격 -> 직군별 스킬변화
        // 재장전 E or 장탄수 모두 소모시 자동으로 수행
        private float speed = 50.0f;

        #region Private Methods

        private void Start()
        {

        }

        private void Update()
        {
            InputKey();
        }

        private void InputKey()
        {
            Move();
        }

        private void Move()
        {
            float moveSpeed = speed * Time.deltaTime;
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
            dir *= moveSpeed;
            transform.Translate(dir);
        }

        #endregion

        #region Public Methods
        #endregion
    }
}
