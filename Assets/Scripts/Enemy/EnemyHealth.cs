using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private GameObject deathVFXprefab;

    private float currentHealth;
    private KnockBack knockBack;
    private Flash flash;

    public GameObject DropLootPrefab;
    GameObject _dropLoopTarget;
    AudioManager audioManager;


    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockBack = GetComponent<KnockBack>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }
    private void Start()
    {
        currentHealth = startingHealth;
        _dropLoopTarget = GameObject.FindGameObjectWithTag("DropLootTracker");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        knockBack.GetKnockBack(PlayerMovements.Instance.transform, 15f);
        //Debug.Log(currentHealth);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }
    private void DetectDeath()
    {
        if(currentHealth <= 0)
        {
            Instantiate(deathVFXprefab, transform.position, Quaternion.identity);
            // Thêm âm thanh khi quái vật hết máu
            audioManager.PlaySFX(audioManager.death);
            Death();
        }
    }

    void Death()
    {
        // Instantiate the death VFX
        if (deathVFXprefab != null)
        {
            Instantiate(deathVFXprefab, transform.position, Quaternion.identity);
        }

        // Instantiate the drop item
        for (int i = 0; i < Random.Range(1,4); i++)
        {
            if (DropLootPrefab != null)
            {
                var go = Instantiate(DropLootPrefab, transform.position + new Vector3(0, Random.Range(0, 3)), Quaternion.identity);
                if (go != null)
                {
                    Follow follow = go.GetComponent<Follow>();
                    if (follow != null && _dropLoopTarget != null)
                    {
                        follow.Target = _dropLoopTarget.transform;
                    }
                }
            }
        }
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
