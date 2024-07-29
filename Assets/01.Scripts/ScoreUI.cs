using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private GameObject[] Stars;
    [SerializeField] private TextMeshProUGUI Score_Text;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button ExitButton;

    private void Start()
    {
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].SetActive(true);
        }
        
        RestartButton.onClick.AddListener(OnRestartButtonClicked);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnEnable()
    {
        Score_Text.text += UIManager.Instance.Score;
    }

    private void Update()
    {
        GiveStars();
    }

    void GiveStars()
    {
        if (UIManager.Instance.Score >= 400)
        {
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].SetActive(false);
            }
        }
        else if (UIManager.Instance.Score < 400  && UIManager.Instance.Score >= 150)
        {
            for (int i = 0; i < Stars.Length - 1; i++)
            {
                Stars[i].SetActive(false);
            }
        }
        else
        {
            Stars[0].SetActive(false);
        }
    }
    
    void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
