using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Throwable : MonoBehaviour
{
    Rigidbody rb;

    AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
    }

    public void Throw(Transform thrower)
    {
        rb.AddForce(thrower.transform.forward * 120 * Time.deltaTime, ForceMode.VelocityChange);
        rb.AddForce(transform.up * 220 * Time.deltaTime, ForceMode.VelocityChange);
        audioSource.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
    }
}
