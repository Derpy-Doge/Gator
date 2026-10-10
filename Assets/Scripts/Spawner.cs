using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject prefab;
    public Transform spawnpoint;
    public float spawnRate = 7f;

    private int spawnCount = 0;


    private void OnEnable()
    {
        prefab.GetComponent<Stats>().attackPower = 0f;
        prefab.GetComponent<Enemy>().attackCooldown = prefab.GetComponent<Enemy>().baseCooldown;
    }

    void Start()
    {
        StartCoroutine(Spawn());
    }

    
    void Update()
    {
        
    }

    IEnumerator Spawn() 
    { 
        while (true)
        {
            Instantiate(prefab, spawnpoint.position, spawnpoint.rotation);
            Stats stats = prefab.GetComponent<Stats>();
            Enemy enemy = prefab.GetComponent<Enemy>();

            spawnCount++;

            #region difficulty scaling
            if (spawnCount == 3)
            {             
                if (stats != null)
                {
                    stats.attackPower += 5f;
                }

                if (enemy != null)
                {
                    enemy.attackCooldown *= .8f;
                }
            }
            else if (spawnCount == 6)
            {
                if (stats != null)
                {
                    stats.attackPower += 5f;
                }


                if (enemy != null)
                {
                    enemy.attackCooldown *= .8f;
                }
            }
            else if (spawnCount == 9)
            {
                if (stats != null)
                {
                    stats.attackPower += 5f;
                }


                if (enemy != null)
                {
                    enemy.attackCooldown *= .8f;
                }
            }
            #endregion

            yield return new WaitForSeconds(spawnRate);
        }
    }
}
