using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public bool roaming = true;
    public float moveSpeed;
    public float nextWPDistance;

    private KnockBack knockBack;

    //shoot
    public bool isShootable = false;
    public GameObject bullet;
    public float bulletSpeed;
    public float timeBtwFire;
    private float fireCoolDown;

    public Seeker seeker;
    public bool updateContinuesPath;
    bool reachDestination = false;
    Path path;
    Coroutine moveCoroutine;

    public Animator animator;
    private int currentWP = 0;

    public float attackRange; // Khoảng cách tấn công
    public bool isAttacking = false; // Biến kiểm tra tấn công
    private Transform playerTransform; // Vị trí của người chơi

    private void Start()
    {
        InvokeRepeating("CalculatePath", 0f, 0.5f);
        reachDestination = true;

        playerTransform = FindObjectOfType<PlayerMovements>().transform; // Tìm và lưu vị trí người chơi

        // Khởi tạo tham chiếu đến Animator
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        fireCoolDown -= Time.deltaTime;

        if (fireCoolDown < 0)
        {
            fireCoolDown = timeBtwFire;
            //shoot
            EnemyFireBullet();
        }

        // Kiểm tra xem path có phải là null và currentWP có nhỏ hơn path.vectorPath.Count
        if (path != null && currentWP < path.vectorPath.Count)
        {
            // Cập nhật giá trị x, y cho Animator
            Vector3 direction = ((Vector2)path.vectorPath[currentWP] - (Vector2)transform.position).normalized;
            animator.SetFloat("x", direction.x);
            animator.SetFloat("y", direction.y);
        }

        // Kiểm tra khoảng cách giữa kẻ địch và người chơi
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            // Nếu trong phạm vi tấn công, kích hoạt hoạt ảnh tấn công
            isAttacking = true;
            animator.SetBool("isAttack", isAttacking);
        }
        else
        {
            // Nếu không trong phạm vi, hủy hoạt ảnh tấn công
            isAttacking = false;
            animator.SetBool("isAttack", isAttacking);
        }
    }

    private void Awake()
    {
        knockBack = GetComponent<KnockBack>();
    }

    private void FixedUpdate()
    {
        if (knockBack.gettingKnockedBack) { return; }
    }
    void EnemyFireBullet()
    {
        var bulletTmp = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody2D rb = bulletTmp.GetComponent<Rigidbody2D>();
        Vector3 playerPos = FindObjectOfType<PlayerMovements>().transform.position;
        Vector3 direction = playerPos - transform.position;
        rb.AddForce(direction.normalized * bulletSpeed, ForceMode2D.Impulse);
    }
    void CalculatePath()
    {
        Vector2 target = FindTarget();
        if (seeker.IsDone() && (reachDestination || updateContinuesPath))
            seeker.StartPath(transform.position, target, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error) return;
        path = p;

        //Move to target
        MoveToTarget();
    }
    void MoveToTarget()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        int currentWP = 0;
        reachDestination = false;
        while (currentWP < path.vectorPath.Count)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWP] - (Vector2)transform.position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;
            transform.position += (Vector3)force;

            float distance = Vector2.Distance(transform.position, path.vectorPath[currentWP]);
            if(distance < nextWPDistance)
                currentWP++;

            yield return null;
        }
        reachDestination = true;
    }

    Vector2 FindTarget()
    {
        if (FindObjectOfType<PlayerMovements>())
        {
            Vector3 playerPos = FindObjectOfType<PlayerMovements>().transform.position;
            if (roaming == true)
            {
                return (Vector2)playerPos + (Random.Range(10f, 50f) * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized);
            }
            else
            {
                return playerPos;
            }
        }
        else
        {
            Debug.LogError("No object of type PlayerMovement found");
            return Vector2.zero;
        }

    }
}
