using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<EnemyHealth>())
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
