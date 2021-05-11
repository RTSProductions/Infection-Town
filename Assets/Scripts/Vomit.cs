using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vomit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CleanUp());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CleanUp()
    {
        yield return new WaitForSeconds(20f);

        Destroy();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
