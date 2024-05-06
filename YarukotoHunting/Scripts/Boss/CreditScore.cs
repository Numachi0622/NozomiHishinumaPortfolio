using UnityEngine;
using UnityEngine.UI;

public class CreditScore : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    private int maxScore = 100;
    public int score{ get; private set; }
    private bool isDefeatable => score >= 60;
    public bool IsDefeatable { get => isDefeatable; }
  
    private void Start()
    {
        score = 0;
    }

    public void AddScore()
    {
        score += 2;
        if (score >= maxScore) score = maxScore;
        scoreText.text = score.ToString();
    }
}
