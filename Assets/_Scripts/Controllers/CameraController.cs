using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private const float CameraHeightDURATION = 1.2f;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Collider boundsCollider;
    [SerializeField]
    private Transform cameraGO;


    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        float camXPos = Mathf.Clamp(playerTransform.position.x, boundsCollider.bounds.min.x, boundsCollider.bounds.max.x);
        float camZPos = Mathf.Clamp(playerTransform.position.z, boundsCollider.bounds.min.z, boundsCollider.bounds.max.z);

        transform.position = new Vector3(camXPos, transform.position.y, camZPos);
    }

    private IEnumerator CameraHeight()
    {
        yield return new WaitForSeconds(0.9f);
        Vector3 startPosition = cameraGO.localPosition;
        Vector3 endPosition = -cameraGO.forward + startPosition + (startPosition * 0.08f);

        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1 && cameraGO.transform.position.y < 50)
        {
            cameraGO.localPosition = Vector3.Lerp(startPosition, endPosition, progress);
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / CameraHeightDURATION;
            yield return null;
        }
        if (cameraGO.transform.position.y < 50) { cameraGO.localPosition = endPosition; }
    }

    public void SetCameraHeight()
    {
        _ = StartCoroutine(CameraHeight());
    }
}
