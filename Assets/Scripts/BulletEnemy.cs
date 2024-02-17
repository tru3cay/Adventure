using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public int damage = 1; // Số máu sẽ bị trừ khi bị bắn trúng

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovements player = collision.GetComponent<PlayerMovements>();
            if (player != null)
            {
                player.ChangeHealth(-damage);
            }
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy") && !collision.CompareTag("Muzzle"))
        {
            Destroy(gameObject);
        }
    }
}
