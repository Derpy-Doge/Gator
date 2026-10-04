using UnityEngine;

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

    [Space(3f)]

    public static bool isDead = false; //for player only

    void Start()
    {
        healthCurrent = healthMax;

        if (GetComponent<InputReader>() != null)
        {
            isPlayer = true;
        }
    }

    
    void Update()
    {
        if (healthCurrent <= 0 && isPlayer)
        {
            isDead = true;
        }
        else if (healthCurrent <= 0 && !isPlayer)
        {
            //award points
            Destroy(gameObject);
        }

        if(healthCurrent > healthMax)
        {
            healthCurrent = healthMax;
        }
    }
}
