using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private int attempts = 0;
    private float timer = 0f;
    private bool running = false;

    private float bestScore = 0f;

    void Awake()
    {
        instance = this;
        bestScore = PlayerPrefs.GetFloat("BestScore", float.MaxValue);
        bestScoreText.text = bestScore != float.MaxValue ? "Best: " + bestScore.ToString("F2") + "s" : "";
    }

    void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;
            timeText.text = "Time: " + timer.ToString("F2") + "s";
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        running = true;
    }

    public void StopTimer()
    {
        running = false;
    }

    public float GetTime()
    {
        return timer;
    }

    public void AddAttempt()
    {
        attempts++;
        attemptsText.text = "Attempts: " + attempts;
    }

    public int GetAttempts()
    {
        return attempts;
    }

    public float GetBestScore()
    {
        return bestScore;
    }

    public void SaveBestScore(float currentTime)
    {
        if (currentTime < bestScore)
        {
            bestScore = currentTime;
            PlayerPrefs.SetFloat("BestScore", bestScore);
            PlayerPrefs.Save();
        }
    }
}