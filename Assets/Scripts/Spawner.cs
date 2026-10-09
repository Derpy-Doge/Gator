using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject prefab;
    public Transform spawnpoint;
    public float spawnRate = 7f;


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
            yield return new WaitForSeconds(spawnRate);
        }
    }
}
