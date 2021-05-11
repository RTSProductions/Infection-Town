using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enviroment : MonoBehaviour
{
    public int InfectedCitizens = 0;

    public int UnInfectedCitizens = 0;

    public bool IsPandemic = false;

    public int startCitzenCount = 300;

    public int startInfectedCount = 1;

    public int throwAbleCount = 100;

    public LayerMask ground;

    public LayerMask ground2;

    public GameObject citzenPrefab;

    public GameObject UI;

    public static int maxThrowUp = 10;

    [Range(0f, 10f)]
    public int maxThrowUpChance = 10;

    [Range(1, 100)]
    public float timeScale = 1;

    public float spawnRange = 100;

    Waypoint[] waypoints;

    public GameObject[] throwAbles;

    bool UIOn = true;

    int fireCount = 5;

    public GameObject fire;

    Citizen[] Citzens;

    List<Citizen> infectedCitizens = new List<Citizen>();

    List<Citizen> unInfectedCitizens = new List<Citizen>();

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

        for (int i = 0; i < startCitzenCount; i++)
        {
            var random = new System.Random();
            var index = random.Next(waypoints.Length);

            Waypoint spawnPoint = waypoints[index];

            var person = Instantiate(citzenPrefab, spawnPoint.transform.position, Quaternion.identity);

            Citizen citzen = person.GetComponent<Citizen>();

            citzen.target = spawnPoint.transform;
        }

        Citzens = FindObjectsOfType<Citizen>();

        for (int i = 0; i < startInfectedCount; i++)
        {
            Citzens[i].infected = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Citzens = FindObjectsOfType<Citizen>();

        foreach(var cit in Citzens)
        {
            if (cit.infected == true)
            {
                if (!infectedCitizens.Contains(cit))
                {
                    infectedCitizens.Add(cit);
                }
                if (unInfectedCitizens.Contains(cit))
                {
                    unInfectedCitizens.Remove(cit);
                }
            }
            else
            {
                if (!unInfectedCitizens.Contains(cit))
                {
                    unInfectedCitizens.Add(cit);
                }
                if (infectedCitizens.Contains(cit))
                {
                    infectedCitizens.Remove(cit);
                }
            }
        }

        InfectedCitizens = infectedCitizens.Count;
        UnInfectedCitizens = unInfectedCitizens.Count;

        if (InfectedCitizens >= UnInfectedCitizens)
        {
            Pandemic();
        }

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

    public void Pandemic()
    {
        IsPandemic = true;

        foreach(var cit in Citzens)
        {
            if (InfectedCitizens >= UnInfectedCitizens && InfectedCitizens < UnInfectedCitizens * 2)
            {
                cit.Warn(WarnType.mask);
            }
            if (InfectedCitizens >= UnInfectedCitizens * 2)
            {
                cit.Warn(WarnType.mask);

                cit.Warn(WarnType.quarentine);
            }
        }
    }
}
