using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public int aiId;
    public int score;
    public int previousScore;
    public int holeScaleThreshold;
    public float speed = 0.05f;
    public float turnSpeed = 1f;
    public float initialScale = 0.5f;
    public float maxDragDistance = 40f;
    public MeshCollider groundCollider;
    public PolygonCollider2D hole2DCollider;
    public PolygonCollider2D ground2DCollider;
    public MeshCollider generatedMeshCollider;

    private Vector3 motion;
    private Mesh generatedMesh;
    public Vector3 randomPosition;
    public Vector3 distanceDifference;
    public Vector3 startDistance;
    private List<Collider> obstaclesColliders;
    private const float ScaleHoleDURATION = 0.4f;

    public float minSpeed = 2f;
    public float maxSpeed = 4f;
    public float directionChangeInterval = 2f;

    private Rigidbody rb;
    public Vector3 movementDirection;
    private float currentSpeed;
    private float timer;


    #region Unity Methods
    private void Start()
    {
        GameController.Instance.aiController.Add(this);
        aiId = GameController.Instance.aiController.IndexOf(this);
        obstaclesColliders = GameController.Instance.obstaclesColliders;

        foreach (Collider collider in obstaclesColliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                Physics.IgnoreCollision(collider, generatedMeshCollider, true);
            }
        }
        rb = GetComponent<Rigidbody>();
        SetRandomMovement();
        holeScaleThreshold = 5;
    }

    private void Update()
    {
        if (!GameController.Instance.start) { return; }
        timer += Time.deltaTime;

        if (timer >= directionChangeInterval)
        {
            SetRandomMovement();
            timer = 0f;
        }
        if (motion != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(motion, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!GameController.Instance.start)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            hole2DCollider.transform.position = new Vector2(transform.position.x, transform.position.z);
            hole2DCollider.transform.localScale = transform.localScale * initialScale;
            MakeHole2D();
            Make3DMeshCollider();
        }
        rb.velocity = motion * currentSpeed;
        transform.position = ClampPositionWithinBoundary(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Eatables"))
        {
            other.gameObject.GetComponent<Eatables>().iD = aiId;
            other.attachedRigidbody.isKinematic = false;
        }
        Physics.IgnoreCollision(other, groundCollider, true);
        Physics.IgnoreCollision(other, generatedMeshCollider, false);
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(other, groundCollider, false);
        Physics.IgnoreCollision(other, generatedMeshCollider, true);
    }
    #endregion

    #region Game Mechanics
    private void MakeHole2D()
    {
        Vector2[] pointPositions = hole2DCollider.GetPath(0);
        for (int i = 0; i < pointPositions.Length; i++)
        {
            pointPositions[i] = hole2DCollider.transform.TransformPoint(pointPositions[i]);
        }

        ground2DCollider.pathCount = 2;
        ground2DCollider.SetPath(1, pointPositions);
    }

    private void Make3DMeshCollider()
    {
        if (generatedMesh != null) { Destroy(generatedMesh); }
        generatedMesh = ground2DCollider.CreateMesh(true, true);
        generatedMeshCollider.sharedMesh = generatedMesh;
    }

    private IEnumerator HoleScale()
    {
        score++;
        holeScaleThreshold += 5;
        speed = speed < 0.4f ? speed + 0.013f : speed;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale + startScale * 0.1f;

        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1 && transform.localScale.x < 10f)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / ScaleHoleDURATION;
            yield return null;
        }
        if (transform.localScale.x < 10f) { transform.localScale = endScale; }
    }
    
    private void SetRandomMovement()
    {
        motion = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    private Vector3 ClampPositionWithinBoundary(Vector3 position)
    {
        float xPosClamp = Mathf.Clamp(position.x, groundCollider.bounds.min.x + 10, groundCollider.bounds.max.x - 10);
        float zPosClamp = Mathf.Clamp(position.z, groundCollider.bounds.min.z + 10, groundCollider.bounds.max.z - 10);
        return new Vector3(xPosClamp, transform.position.y, zPosClamp);
    }
    #endregion

    #region Public Methods
    public void SetHoleScale()
    {
        _ = StartCoroutine(HoleScale());
    }
    #endregion
}
