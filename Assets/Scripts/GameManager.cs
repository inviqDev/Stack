using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [SerializeField] private TextMeshProUGUI textField;
    
    private ScoreCounter _scoreCounter;
    public static GameManager Instance { get; private set; }

    public event Action OnTimerCountdownFinished;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _scoreCounter = GetComponent<ScoreCounter>();
        
        var timerStartValue = 3;
        StartTimerCountdown(timerStartValue);
    }

    private IEnumerator StartTimerCountdownCoroutine(int countdownFrom)
    {
        inputController.gameObject.SetActive(false);
        textField.gameObject.SetActive(true);
        
        var counter = countdownFrom;
        while (counter > 0)
        {
            textField.text = counter.ToString();
            
            yield return new WaitForSeconds(1.0f);
            counter--;
        }
        
        inputController.gameObject.SetActive(true);
        textField.gameObject.SetActive(false);
        
        OnTimerCountdownFinished?.Invoke();
        yield return null;
    }

    public void StartTimerCountdown(int countdownFrom)
    {
        StartCoroutine(StartTimerCountdownCoroutine(countdownFrom));
    }

    public void ChangeCurrentScore()
    {
        _scoreCounter.ChangeCurrentScore();
    }

    public void ShowGameOverPrompt()
    {
        textField.text = "GAME OVER!";
        
        inputController.gameObject.SetActive(false);
        textField.gameObject.SetActive(true);
        
        Time.timeScale = 0.0f;
    }
}