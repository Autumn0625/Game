using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wolf"))
        {
            Destroy(other.gameObject);
        }
    }
}
