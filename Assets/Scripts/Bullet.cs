using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Khi có va chạm xảy ra
    void OnTriggerEnter2D(Collider2D collision)
    {

        // Kiểm tra tag của đối tượng va chạm
        if (!collision.CompareTag("Player"))
        {
            // Hủy đối tượng đạn
            Destroy(gameObject);
        }
    }
}
