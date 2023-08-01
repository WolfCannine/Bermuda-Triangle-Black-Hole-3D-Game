using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public bool start;
    public int score;
    public int previousScore;
    public int holeScaleThreshold;
    public List<AIController> aiController = new();
    public List<Collider> obstaclesColliders;

    [SerializeField] private GameEvent setHoleScaleEvent;

    private void Awake()
    {
        Instance = this;
        obstaclesColliders = (FindObjectsOfType(typeof(Collider)) as Collider[]).ToList();
        holeScaleThreshold = 5;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Eatables"))
        {
            if (collision.gameObject.GetComponent<Eatables>().iD == -2)
            {
                PlayerProgress();
            }
            else
            {
                AIProgress(collision.gameObject.GetComponent<Eatables>().iD);
            }
            collision.gameObject.SetActive(false);
        }
    }

    private void PlayerProgress()
    {
        score++;
        if (score >= previousScore + holeScaleThreshold)
        {
            previousScore = score;
            holeScaleThreshold += 5;
            setHoleScaleEvent.TriggerEvent();
            CameraController.Instance.SetCameraHeight();
        }
        GameplayUI.Instance.SetProgressCircle();
    }

    private void AIProgress(int ai_id)
    {
        if (ai_id == -1) { return; }
        aiController[ai_id].score++;
        if (aiController[ai_id].score >= aiController[ai_id].previousScore + aiController[ai_id].holeScaleThreshold)
        {
            aiController[ai_id].previousScore = aiController[ai_id].score;
            aiController[ai_id].SetHoleScale();
        }
    }
}
