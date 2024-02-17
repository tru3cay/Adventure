using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class BossController : MonoBehaviour
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

    private int currentWP = 0;

    public float attackRange; // Khoảng cách tấn công
    public bool isAttacking = false; // Biến kiểm tra tấn công
    private Transform playerTransform; // Vị trí của người chơi

    private void Start()
    {
        InvokeRepeating("CalculatePath", 0f, 0.5f);
        reachDestination = true;

        playerTransform = FindObjectOfType<PlayerMovements>().transform; // Tìm và lưu vị trí người chơi

    }

        float angle = 0f;
    private void Update()
    {
        fireCoolDown -= Time.deltaTime;

        if (fireCoolDown < 0)
        {
            fireCoolDown = timeBtwFire;
            //shoot
            StartCoroutine(EnemyFireBullet());
        }

        // Kiểm tra xem path có phải là null và currentWP có nhỏ hơn path.vectorPath.Count
        if (path != null && currentWP < path.vectorPath.Count)
        {
            // Cập nhật giá trị x, y cho Animator
            Vector3 direction = ((Vector2)path.vectorPath[currentWP] - (Vector2)transform.position).normalized;
        }
        // Rotate the firing direction every second
        angle += 30 * Time.deltaTime;
    }

    private void Awake()
    {
        knockBack = GetComponent<KnockBack>();
    }

    private void FixedUpdate()
    {
        if (knockBack.gettingKnockedBack) { return; }
    }
    IEnumerator EnemyFireBullet()
    {

        float angleStep = 360f / 4;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                float projectileDirXposition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * 0.5f;
                float projectileDirYposition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * 0.5f;

                Vector3 projectileVector = new Vector3(projectileDirXposition, projectileDirYposition);
                Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized * bulletSpeed;

                var bulletTmp = Instantiate(bullet, transform.position, Quaternion.identity);
                bulletTmp.transform.rotation = Quaternion.Euler(0, 0, angle);
                Rigidbody2D rb = bulletTmp.GetComponent<Rigidbody2D>();
                rb.AddForce(projectileMoveDirection, ForceMode2D.Impulse);

                yield return new WaitForSeconds(0.2f);
            }

            angle += angleStep;
        }

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
            if (distance < nextWPDistance)
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
