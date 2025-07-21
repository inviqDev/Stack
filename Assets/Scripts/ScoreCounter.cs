using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    public int CurrentScore { get; private set; }

    private void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.Log("GameManager Instance is not found");
            return;
        }

        GameManager.Instance.OnTimerCountdownFinished += OnTimerCountdownFinished;
    }

    private void OnEnable()
    {
        if (!GameManager.Instance)
        {
            Debug.Log("GameManager Instance is not found");
            return;
        }

        GameManager.Instance.OnTimerCountdownFinished += OnTimerCountdownFinished;
    }

    private void OnTimerCountdownFinished()
    {
        textField.gameObject.SetActive(true);
        
        CurrentScore = 0;
        textField.text = $"Score: {CurrentScore.ToString()}";
    }

    public void ChangeCurrentScore(int increment = 1)
    {
        CurrentScore += increment;
        textField.text = $"Score: {CurrentScore.ToString()}";
    }

    private void OnDisable()
    {
        if (!GameManager.Instance)
        {
            Debug.Log("GameManager Instance is not found");
            return;
        }
        
        GameManager.Instance.OnTimerCountdownFinished += OnTimerCountdownFinished;
    }

    private void OnDestroy()
    {
        if (!GameManager.Instance)
        {
            Debug.Log("GameManager Instance is not found");
            return;
        }
        
        GameManager.Instance.OnTimerCountdownFinished += OnTimerCountdownFinished;
    }
}