using UnityEngine;

public class DebugCam : MonoBehaviour
{
    [SerializeField] private Camera debugCam;
    private Transform player;
    private bool isEnabled = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        debugCam.enabled = false;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            ToggleCam();

        FollowPlayer();
    }

    private void ToggleCam()
    {
        isEnabled = !isEnabled;
        debugCam.enabled = isEnabled;
    }

    private void FollowPlayer()
    {
        if (!isEnabled)
            return;
        
        transform.position = new Vector3(player.position.x, player.position.y + 4f, player.position.z);
    }
}
