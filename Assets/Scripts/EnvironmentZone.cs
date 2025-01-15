using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentZone : MonoBehaviour
{
    public Color ambientColor;
    public float fogDensity;
    public AudioClip ambientSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnvironmentManager.Instance.SetEnvironment(this);
        }
    }
}
