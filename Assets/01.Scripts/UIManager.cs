using System;
using System.Collections;
using TMPro;
using UnityEngine;

public sealed class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject GameOverUI;
    
    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            if (_score == value) return;
            _score = value;
            if (scoreText != null)
            {
                scoreText.SetText($"{_score}");
            }
        }
    }
    
    private float timeLimit = 60f;
    private float timeRemaining;
    private bool gameEnded = false;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = timeLimit;
        UpdateTimerText();
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            UpdateTimerText();
        }
        
        if (!gameEnded)
        {
            EndGame();
        }
    }
    
    void UpdateTimerText()
    {
        if (timerText == null) return;
        
        timerText.text = Mathf.CeilToInt(timeRemaining) + "s";

        if (timeRemaining <= 10)
        {
            timerText.color = Color.red;
        }
        else if (timeRemaining <= 29)
        {
            timerText.color = Color.yellow;
        }
        else
        {
            timerText.color = Color.green;
        }
    }
    
    void EndGame()
    {
        gameEnded = true;
        GameOverUI.SetActive(true);
    }
}