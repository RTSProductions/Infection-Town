using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityTemplateProjects;

public class FocusOnObject : MonoBehaviour
{
    public bool focused = false;

    SimpleCameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GetComponent<SimpleCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !Input.GetKey(KeyCode.F) && focused == true)
        {
            UnFocus();
        }
    }

    public void Focus(Transform camSpot, Transform citzen)
    {
        cameraController.enabled = false;

        this.transform.parent = citzen;
        this.transform.position = camSpot.position;
        this.transform.rotation = camSpot.rotation;
        focused = true;
    }

    public void UnFocus()
    {
        this.transform.parent = null;

        cameraController.enabled = true;

        focused = false;
    }
    
}
