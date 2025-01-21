using UnityEngine;

public class DebugCam : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera debugCam;
    private bool isEnabled = false;

    void Start()
    {
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
        
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
    }
}
