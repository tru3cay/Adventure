using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public float maxHealth = 150;
    public float timeInvincible = 2.0f;

    public float health { get { return currentHealth; } }
    float currentHealth;

    bool isInvincible;
    float invincibleTimer;

    public float moveSpeed = 5f;

    public static PlayerMovements Instance;

    public float rollBoost = 2f;
    private float rollTime;
    public float RollTime;
    bool rollOnce = false;

    private Vector2 direction;

    public Rigidbody2D rb;
    public Animator animator;

    public SpriteRenderer spriteRenderer; // Renderer của sprite nhân vật
    public float flashDuration = 0.3f; // Thời gian flash


    //dialog
    private bool canInteract = true;

    Vector2 movement;

    //audio
    AudioManager audioManager;
    private float soundTimer = 0f; // Biến thời gian để đếm khoảng thời gian giữa các lần phát âm thanh

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        Instance = this;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

        // Xử lý đầu vào từ bàn phím
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        // Xác định vị trí con chuột trên màn hình
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Tính toán hướng từ nhân vật đến con chuột
        direction = (mousePosition - rb.position).normalized;

        // Cập nhật hướng nhìn của nhân vật
        animator.SetFloat("Move X", direction.x);
        animator.SetFloat("Move Y", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1)) && rollTime <= 0)
        {
            animator.SetBool("Roll", true);
            moveSpeed += rollBoost;
            rollTime = RollTime;
            rollOnce = true;
        }

        if (rollTime <= 0 && rollOnce == true)
        {
            animator.SetBool("Roll", false);
            moveSpeed -= rollBoost;
            rollOnce = false;
        }
        else
        {
            rollTime -= Time.deltaTime;
        }

        // sử dụng rascasting
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Input.GetKeyDown(KeyCode.X) && canInteract)
            {
                StartCoroutine(InteractWithNPC());
            }

        }
    }

    //dialog
    private IEnumerator InteractWithNPC()
    {
        canInteract = false;

        RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, direction, 2.0f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
            if (character != null)
            {
                character.DisplayDialog();
            }
        }

        yield return new WaitForSeconds(2.5f);

        canInteract = true;
    }


    private void FixedUpdate()
    {
        //handle movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        // Play sound effect with a delay of 2 seconds between each play
        if (movement != Vector2.zero)
        {
            if (soundTimer <= 0f)
            {
                audioManager.PlaySFX(audioManager.move);
                soundTimer = 0.6f; // Đặt thời gian đếm lại 2 giây sau khi phát âm thanh
            }
        }

        // Giảm thiểu giá trị của biến thời gian
        if (soundTimer > 0f)
        {
            soundTimer -= Time.fixedDeltaTime;
        }
    }
    public void GameOver()
    {
        // Tìm đối tượng chứa script GameOver trong Scene
        GameObject gameOverObject = GameObject.FindGameObjectWithTag("GameOver");

        if (gameOverObject != null)
        {
            // Lấy component GameOver từ đối tượng chứa script
            GameOver gameOverScript = gameOverObject.GetComponent<GameOver>();

            if (gameOverScript != null)
            {
                // Gọi phương thức Over() từ script GameOver để hiển thị màn hình Game Over
                gameOverScript.Over();
            }
            else
            {
                Debug.LogError("Game Over script not found on the GameOver object!");
            }
        }
        else
        {
            Debug.LogError("Game Over object not found in the scene!");
        }
    }
    public void ChangeHealth(float amount)
    {
        if (amount < 0)
        {
            audioManager.PlaySFX(audioManager.damagePlayer);

            if (isInvincible)
                return;
            StartCoroutine(Flash());
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        if (amount > 0)
        {
                audioManager.PlaySFX(audioManager.collect);

        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        //Debug.Log(currentHealth + "/" + maxHealth);
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có tag là "Enemy" không
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Gọi phương thức ChangeHealth với số lượng máu muốn giảm
            ChangeHealth(-30); // Giả sử mỗi lần chạm vào kẻ địch, người chơi mất 1 máu
        }
    }
    //chuyển cảnh thì reset lại máu
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
}
