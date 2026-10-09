using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats : MonoBehaviour
{
    public float healthMax;
    public float healthCurrent;
    [Space(5f)]

    public float attackPower;
    [Space(3f)]
    public float defense;

    [Space(5f)]
    private InputReader player;
    bool isPlayer;

    [Space(5f)]
    public Gurt score;

    public static bool isDead = false; //for player only

    void Start()
    {
        healthCurrent = healthMax;

        if (GetComponent<InputReader>() != null)
        {
            isPlayer = true;
        }

        score = FindAnyObjectByType<Gurt>();
    }

    
    void Update()
    {
        if (healthCurrent <= 0 && isPlayer)
        {
            isDead = true;
            SceneManager.LoadScene("Lose");
        }
        else if (healthCurrent <= 0 && !isPlayer)
        {
            score.AddScore(gameObject.GetComponent<Enemy>().pointsOnDeath);
            Destroy(gameObject);
        }

        if(healthCurrent > healthMax)
        {
            healthCurrent = healthMax;
        }
    }
}
