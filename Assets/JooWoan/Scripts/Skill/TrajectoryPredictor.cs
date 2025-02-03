using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] private Rigidbody projectile;
    [SerializeField] private Transform startPoint, hitMarker;
    [SerializeField] private float predictInterval, hitMarkerGroundOffset;
    [SerializeField] private int maxPoints;
    private LineRenderer trajectoryLine;
    private Camera cam;
    private const float RAY_OVERLAP = 1.2f;
    private float force;

    void Start()
    {
        cam = Camera.main;
        trajectoryLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        SetForce();
        Predict();
        Fire();
    }

    private void SetForce()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            return;
        
        Vector3 startPos = new Vector3(startPoint.position.x, hit.point.y, startPoint.position.z);
        force = Vector3.Distance(hit.point, startPos);
    }

    private void Fire()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        Rigidbody thrownObject = Instantiate(projectile, startPoint.position, Quaternion.identity);
        thrownObject.AddForce(startPoint.forward * force, ForceMode.Impulse);
    }

    private void Predict()
    {
        Vector3 currentVelocity = startPoint.forward * (force / projectile.mass);
        Vector3 currentPosition = startPoint.position;
        Vector3 nextPosition;

        UpdateLine(maxPoints, 0, currentPosition);

        for (int i = 1; i < maxPoints; i++)
        {
            currentVelocity = CalculateNewVelocity(currentVelocity, projectile.drag, predictInterval);
            nextPosition = currentPosition + currentVelocity * predictInterval;

            float overlap = Vector3.Distance(currentPosition, nextPosition) * RAY_OVERLAP;

            if (Physics.Raycast(currentPosition, currentVelocity.normalized, out RaycastHit hit, overlap))
            {
                UpdateLine(i, i - 1, hit.point);
                ShowHitMarker(hit);
                break;
            }

            HideHitMarker();
            currentPosition = nextPosition;
            UpdateLine(maxPoints, i, currentPosition);
        }
    }

    private void UpdateLine(int positionCount, int index, Vector3 position)
    {
        trajectoryLine.positionCount = positionCount;
        trajectoryLine.SetPosition(index, position);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float predictInterval)
    {
        velocity += Physics.gravity * predictInterval;
        velocity *= Mathf.Clamp01(1f - drag * predictInterval);
        return velocity;
    }

    private void ShowHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);
        hitMarker.position = hit.point + hit.normal * hitMarkerGroundOffset;
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }

    private void HideHitMarker()
    {
        hitMarker.gameObject.SetActive(false);
    }
}
