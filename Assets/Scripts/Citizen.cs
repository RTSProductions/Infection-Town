using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    public bool infected = false;

    public bool next = true;

    public GameObject mask;

    public bool masked = false;

    public bool quarantined = false;

    Animator animator;

    public Material sickSkin;

    public MeshRenderer head;

    NavMeshAgent agent;

    SphereCollider trigger;

    public Transform target;

    AudioSource mob;

    Transform leader;

    [Range(0, 100f)]
    public float infectionChance = 50f;

    bool waiting = false;

    [Range(0, 100f)]
    public float nonCovidiot = 50f;

    float infectionPersnetage;

    public Transform camSpot;

    float walkPointRange = 100;

    public Material[] skinTones;

    public Material[] shirts;

    public Material[] pants;

    public MeshRenderer[] skinMeshs;

    Material skin;

    public Transform vomitSpot;

    public GameObject vomit;

    bool triggerEntered = false;

    bool canMakeNoise = false;

    Vector3 walkPoint;

    bool walkPointSet = false;

    public MeshRenderer body;

    public int randomOfset = 0;

    public LayerMask ground;

    bool canThrow = false;

    public bool canThrowTwo = false;

    bool throwing = false;

    bool recovering = false;

    bool recoveringTwo = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WillMakeNoise());

        mob = GetComponent<AudioSource>();

        trigger = this.gameObject.AddComponent<SphereCollider>();

        trigger.isTrigger = true;

        trigger.radius = 6;

        next = (Random.value < 0.5);

        randomOfset = Random.Range(0, 5);

        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        var skinRandom = new System.Random();
        var skinIndex = skinRandom.Next(skinTones.Length);

        Material skinColor = skinTones[skinIndex + randomOfset];

        skin = skinColor;

        foreach (var skinMesh in skinMeshs)
        {
            skinMesh.sharedMaterial = skin;
        }

        head.sharedMaterial = skin;



        var pantsRandom = new System.Random();
        var pantsIndex = pantsRandom.Next(pants.Length);

        Material pantsColor = pants[pantsIndex + randomOfset];



        var shirtRandom = new System.Random();
        var shirtIndex = shirtRandom.Next(shirts.Length);

        Material shirtColor = shirts[shirtIndex + randomOfset];

        Material[] bodyMats = new Material[2];

        bodyMats[0] = shirtColor;
        bodyMats[1] = pantsColor;



        body.sharedMaterials = bodyMats;

        if (infected == true)
        {
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Sick", infected);

        if (recovering == true)
        {
            infected = false;
        }


        
        

        infectionPersnetage = infectionChance  / 100f;

        if (masked)
        {
            infectionPersnetage = infectionChance - 5 / 100f;
        }

        if (infected == false)
        {
            

            float distace = Vector3.Distance(target.position, transform.position);

            agent.SetDestination(target.position);

            if (distace <= 1)
            {
                Waypoint waypoint = target.GetComponent<Waypoint>();

                if (waypoint != null)
                {
                    if (next == true)
                    {
                        target = waypoint.nextWaypoint.transform;
                    }
                    else
                    {
                        target = waypoint.previousWaypoint.transform;
                    }
                }
            }

            agent.speed = 6;

            head.material = skin;
        }
        else
        {
            head.material = sickSkin;

            if (throwing == false)
            {
                agent.speed = 3;
            }

            trigger.radius = 6;


            float distace = Vector3.Distance(target.position, transform.position);

            agent.SetDestination(target.position);

            if (distace <= 1)
            {
                Waypoint waypoint = target.GetComponent<Waypoint>();

                if (waypoint != null)
                {
                    if (next == true)
                    {
                        target = waypoint.nextWaypoint.transform;
                    }
                    else
                    {
                        target = waypoint.previousWaypoint.transform;
                    }
                }
            }

        }

        
    }
    private void FixedUpdate()
    {
        int willThrow = Random.Range(0, 10);

        if (willThrow >= Enviroment.maxThrowUp && infected == true && throwing == false)
        {
            if (canThrowTwo == true)
            {
                ThrowUp();
            }
            else if (canThrowTwo == false && !waiting)
            {
                StartCoroutine(canThrowLOL());
            }
        }

        int willRecover = Random.Range(0, 10);

        if (willRecover >= Enviroment.maxThrowUp && infected == true && recoveringTwo == false)
        {
            StartCoroutine(Recover());
        }
    }

    void ThrowUp()
    {
        throwing = true;

        Debug.Log(this.name + " Will Throw Up");
        if (canThrow == false)
        {
            StartCoroutine(Throw());

            canThrowTwo = false;
        }

        
        if (canThrow == true)
        {
            var vom = Instantiate(vomit, vomitSpot.transform.position, vomitSpot.rotation);

            Debug.Log(this.name + " Has Thrown Up");

            canThrow = false;

            canThrowTwo = false;

            throwing = false;
        }

       
    }

    IEnumerator canThrowLOL()
    {
        waiting = true;

        canThrowTwo = false;

        yield return new WaitForSeconds(60);

        canThrowTwo = true;

        waiting = false;
    }
    IEnumerator Recovering()
    {
        recovering = true;

        yield return new WaitForSeconds(30f);

        Debug.Log(this.name + " Can Get Infected Again");

        recoveringTwo = false;
    }
    IEnumerator Recover()
    {
        recoveringTwo = true;

        yield return new WaitForSeconds(300f);

        infected = false;

        Debug.Log(this.name + " Has Recovered");

        StartCoroutine(Recovering());
    }
    IEnumerator findNewPoint()
    {

        yield return new WaitForSeconds(120f);


        FindNewPoint();
    }

    void FindNewPoint()
    {
        Waypoint[] waypoints = FindObjectsOfType<Waypoint>();

        var random = new System.Random();
        var index = random.Next(waypoints.Length);

        Waypoint newPoint = waypoints[index];

        target = newPoint.transform;

        Debug.Log(this.name + " Has Found A New Point");
    }


    void WalkRandom()
    {


        if (!walkPointSet)
        {
            SeachWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        float distace = Vector3.Distance(walkPoint, transform.position);
        if (distace <= 1)
        {
            walkPointSet = false;
        }
    }

    void SeachWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
        {
            walkPointSet = true;
            StartCoroutine(WalkNewPlace());
        }


    }
  
    IEnumerator WalkNewPlace()
    {
        //walkPointSet = true;

        yield return new WaitForSeconds(5f);

        walkPointSet = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (infected == false)
        {
            Citizen citzen = other.GetComponent<Citizen>();

            Vomit vom = other.GetComponent<Vomit>();

            if (citzen != null && other.isTrigger == false)
            {
                if (citzen.infected == true && infected == false)
                {
                    if (!citzen.masked)
                    {
                        infected = (Random.value < infectionPersnetage);
                    }
                    else
                    {
                        float newInfectionPersnetage = (infectionPersnetage * 100 - 30) / 100;
                        if (newInfectionPersnetage == infectionPersnetage)
                        {
                            newInfectionPersnetage -= 0.3f;
                        }
                        infected = (Random.value < newInfectionPersnetage);
                        Debug.Log("1 : " + infectionPersnetage);
                        Debug.Log("2 : " + newInfectionPersnetage);
                    }

                    if (infected == true)
                    {
                        Warn(WarnType.mask);

                        StartCoroutine(findNewPoint());
                    }

                }
            }
            if (vom != null)
            {
                if (infected == false)
                {
                    infected = (Random.value < infectionPersnetage);

                    if (infected == true)
                    {
                        StartCoroutine(findNewPoint());

                        vom.Destroy();
                    }
                }
            }
            
        }
        


    }
    IEnumerator Throw()
    {
        throwing = true;

        agent.speed = 0;

        canThrowTwo = false;

        animator.SetBool("Vomitting", true);

        yield return new WaitForSeconds(2.033f);

        animator.SetBool("Vomitting", false);

        canThrowTwo = false;

        canThrow = true;

        throwing = false;

        ThrowUp();
    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Focus();
        }
    }
    void Focus()
    {
        Camera cam = FindObjectOfType<Camera>();

        FocusOnObject focusOnObject = cam.GetComponent<FocusOnObject>();

        focusOnObject.Focus(camSpot, this.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
    IEnumerator WillMakeNoise()
    {
        canMakeNoise = false;

        yield return new WaitForSeconds(20);

        canMakeNoise = true;
    }

    public void Warn(WarnType type)
    {
        bool willDo = Random.value < nonCovidiot / 100;

        if (type == WarnType.mask)
        {
            if (willDo)
            {
                Mask();
            }
        }
        if (type == WarnType.quarentine)
        {
            Quarentine();
        }
    }

    void Mask()
    {
        masked = true;
        mask.SetActive(true);
    }

    void UnMask()
    {
        masked = true;
        mask.SetActive(false);
    }

    void Quarentine()
    {
        quarantined = true;
    }

    public void Mandate(WarnType type) { }
    
}

public enum WarnType
{
    mask = (1 << 0),
    quarentine = (1 << 1),
}
