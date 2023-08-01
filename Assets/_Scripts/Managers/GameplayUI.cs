using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get; private set; }

    private const float DURATION = 0.4f;
    [SerializeField] private Image progressCircle;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private GameObject bestScorePanel;
    [SerializeField] private GameObject buyTimePanel;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI heighestScoreText;
    [SerializeField] private GameObject pointsTextPrefab;
    private float time = 120;


    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator ProgressCircle()
    {
        Destroy(Instantiate(pointsTextPrefab, gameplayPanel.transform), 0.5f);
        float startFillAmount = progressCircle.fillAmount;
        float scorePoints = (float)(GameController.Instance.score - GameController.Instance.previousScore)
            / GameController.Instance.holeScaleThreshold;
        float endFillAmount = scorePoints <= 0 ? 1f : scorePoints;
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            progressCircle.fillAmount = Mathf.Lerp(startFillAmount, endFillAmount, progress);
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / DURATION;
            yield return null;
        }
        progressCircle.fillAmount = endFillAmount;

        if (progressCircle.fillAmount >= 1) { progressCircle.fillAmount = 0; }// 0.01f; }
    }

    private IEnumerator StartTimer(float time)
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time -= 1;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timeText.text = "Time " + string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            yield return null;
        }
        ActivateBuyTimePanel();
    }

    #region Public Methods
    public void SetProgressCircle()
    {
        scoreText.text = "Score: " + GameController.Instance.score.ToString();
        _ = StartCoroutine(ProgressCircle());
    }

    public void StartGame()
    {
        time = 120;
        startGamePanel.SetActive(false);
        GameController.Instance.start = true;
        _ = StartCoroutine(StartTimer(time));
    }

    private void ActivateBuyTimePanel()
    {
        GameController.Instance.start = false;
        buyTimePanel.SetActive(true);
    }

    public void OnBuyTimeButtonClick()
    {
        BuyTimeDone();
    }

    private void BuyTimeDone()
    {
        buyTimePanel.SetActive(false);
        GameController.Instance.start = true;
        _ = StartCoroutine(StartTimer(30));
    }

    public void OnNoThanksBuyTimeButtonClick()
    {
        buyTimePanel.SetActive(false);
        EndGame();
    }

    public void EndGame()
    {
        GameController.Instance.start = false;
        finalScoreText.text = "Score: " + GameController.Instance.score.ToString();
        if (GameController.Instance.score > PersistentDataManager.Instance.gameData.highestScore)
        {
            PersistentDataManager.Instance.gameData.highestScore = GameController.Instance.score;
            PersistentDataManager.Instance.SaveData();
        }
        gameOverPanel.SetActive(true);
    }

    public void StartAgain()
    {
        GameController.Instance.score = 0;
        SceneManager.LoadScene(sceneName: "Gameplay");
    }

    public void SetHeighestScore()
    {
        heighestScoreText.text = PersistentDataManager.Instance.gameData.highestScore.ToString();
    }

    public void OpenBestScorePanel()
    {
        bestScorePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitBestScorePanel()
    {
        Time.timeScale = 1f;
        bestScorePanel.SetActive(false);
    }
    #endregion
}
