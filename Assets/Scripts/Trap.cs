using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public Animator animator;
    private bool isTrapActive = false;

    private void Start()
    {
        // Bắt đầu với trạng thái bẫy không hoạt động
        isTrapActive = false;
        // Set parameter 'NoTrap' khi bắt đầu
        animator.SetBool("NoTrap", true);
        animator.SetBool("Trap", false);
    }

    private void Update()
    {
        // Kiểm tra xem animation đang ở trạng thái nào
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Trap"))
        {
            // Nếu đang ở trạng thái bẫy hoạt động
            isTrapActive = true;
            // Set parameter 'Trap' để kích hoạt animation bẫy
            animator.SetBool("Trap", true);
            animator.SetBool("NoTrap", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("NoTrap"))
        {
            // Nếu đang ở trạng thái bẫy không hoạt động
            isTrapActive = false;
            // Reset parameter 'Trap' và set 'NoTrap'
            animator.SetBool("Trap", false);
            animator.SetBool("NoTrap", true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
            PlayerMovements controller = other.GetComponent<PlayerMovements>();
        if (other.gameObject.tag == "Player" && isTrapActive)
        {
            // Trừ máu cho người chơi nếu bẫy đang hoạt động
            controller.ChangeHealth(-30);
        }
    }
}
