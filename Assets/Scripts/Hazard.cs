using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public AudioSource deathSound;
    // Hazard Script
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Die();
                deathSound.Play();
            }
        }
    }
}
