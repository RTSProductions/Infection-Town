using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enviroment : MonoBehaviour
{
    public int citzenCount = 250;

    public int throwAbleCount = 100;

    public LayerMask ground;

    public LayerMask ground2;

    public GameObject citzenPrefab;

    public GameObject UI;

    public static int maxThrowUp = 10;

    [Range(0f, 10f)]
    public int maxThrowUpChance = 10;

    public GameObject infectedPrefab;

    [Range(1, 100)]
    public float timeScale = 1;

    public float spawnRange = 100;

    public int infectedCount = 5;

    Waypoint[] waypoints;

    public GameObject[] throwAbles;

    bool UIOn = true;

    int fireCount = 5;

    public GameObject fire;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitSpawnFire());

        waypoints = FindObjectsOfType<Waypoint>();

        foreach (var throwAble in throwAbles)
        {
            for (int i = 0; i < throwAbleCount; i++)
            {
                float randomZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
                float randomX = UnityEngine.Random.Range(-spawnRange, spawnRange);

                RaycastHit hit;

                Vector3 spawnPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
                if (Physics.Raycast(spawnPoint, -transform.up, out hit, ground))
                {
                    var obj = Instantiate(throwAble, hit.point, Quaternion.identity);
                }
            }
        }

        for (int i = 0; i < citzenCount; i++)
        {
            var random = new System.Random();
            var index = random.Next(waypoints.Length);

            Waypoint spawnPoint = waypoints[index];

            var person = Instantiate(citzenPrefab, spawnPoint.transform.position, Quaternion.identity);

            Citzen citzen = person.GetComponent<Citzen>();

            citzen.target = spawnPoint.transform;
        }
        for (int i = 0; i < infectedCount; i++)
        {
            var random = new System.Random();
            var index = random.Next(waypoints.Length);

            Waypoint spawnPoint = waypoints[index];

            var infected = Instantiate(infectedPrefab, spawnPoint.transform.position, Quaternion.identity);

            Citzen citzen = infected.GetComponent<Citzen>();

            citzen.target = spawnPoint.transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        maxThrowUp = maxThrowUpChance;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIOn == false)
            {
                UI.SetActive(true);
                UIOn = true;
            }
            else
            {
                UI.SetActive(false);
                UIOn = false;
            }
        }

        Time.timeScale = timeScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
        
    }
    IEnumerator WaitSpawnFire()
    {

        yield return new WaitForSeconds(120);

        //SpawnFire();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SpawnFire()
    {
        for (int i = 0; i < fireCount; i++)
        {
            float randomZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
            float randomX = UnityEngine.Random.Range(-spawnRange, spawnRange);

            RaycastHit hit;

            Vector3 spawnPoint = new Vector3(transform.position.x + randomX, 20, transform.position.z + randomZ);
            if (Physics.Raycast(spawnPoint, -transform.up, out hit, ground2))
            {
                var obj = Instantiate(fire, hit.point, Quaternion.identity);
            }
        }
        StartCoroutine(WaitSpawnFire());
    }
}
