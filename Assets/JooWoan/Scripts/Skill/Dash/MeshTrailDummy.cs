using EverScord.Pool;
using UnityEngine;

namespace EverScord.Skill
{
    public class MeshTrailDummy : MonoBehaviour, IPoolable
    {
        public MeshRenderer DummyMeshRenderer;
        public MeshFilter DummyMeshFilter;
        public Mesh DummyMesh;

        void Awake()
        {
            DummyMesh = new Mesh();
        }

        public void SetGameObject(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
