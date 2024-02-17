using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlayerWithEne : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovements controller = other.GetComponent<PlayerMovements>();

        if (controller != null)
        {
            if (controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(5f);
            }
                Destroy(transform.parent.gameObject);
        }
    }
}
