using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep")|| other.CompareTag("Lamb"))
        {
            Destroy(other.gameObject);
        }
    }
}
