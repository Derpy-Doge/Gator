using UnityEngine;

public class Gurt : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TMPro.TMP_Text scoreText;


    void Start()
    {
        
    }

    void Update()
    {
        scoreText.SetText(score + "");
    }

    public void AddScore(int amount)
    {
        score += amount;
    }
}
