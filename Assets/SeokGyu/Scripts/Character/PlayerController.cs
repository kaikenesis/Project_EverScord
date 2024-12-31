using UnityEngine;

namespace EverScord
{
    public class PlayerController : MonoBehaviour
    {
        // ���콺�� �ִ� �������� ĳ���� ȸ��
        // ��Ŭ��,��Ŭ�� ����
        // Q,R ��ų ���� -> ������ ��ų��ȭ
        // ������ E or ��ź�� ��� �Ҹ�� �ڵ����� ����
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
