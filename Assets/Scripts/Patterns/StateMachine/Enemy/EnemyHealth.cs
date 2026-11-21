
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Knockback")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.15f;

    [Header("Hit Flash")]
    public float flashDuration = 0.1f;

    private int currentHealth;

    private EnemyData enemyData;
    private NavMeshAgent agent;

    private Renderer[] renderers;

    private Material[] originalMaterials;
    private Material flashMaterial;

    void Start()
    {
        // Get enemy data from the Enemy component
        Enemy enemy = GetComponent<Enemy>();

        if (enemy == null || enemy.enemyData == null)
        {
            Debug.LogError("EnemyHealth: Enemy or EnemyData is missing!", this);
            enabled = false;
            return;
        }

        enemyData = enemy.enemyData;

        // Set health using the ScriptableObject
        currentHealth = enemyData.maxHealth;

        // Find NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        // Find all renderers on the enemy and its children
        renderers = GetComponentsInChildren<Renderer>();

        // Store original materials
        originalMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }

        // Create white flash material
        flashMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        flashMaterial.color = Color.white;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;

        Debug.Log(enemyData.enemyName + " took " + damage + " damage! Remaining health: " + currentHealth);

        // Flash white
        StartCoroutine(FlashWhite());

        // Knock enemy backward
        StartCoroutine(Knockback());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashWhite()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = flashMaterial;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }

    IEnumerator Knockback()
    {
        if (agent == null)
            yield break;

        agent.isStopped = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            agent.isStopped = false;
            yield break;
        }

        Vector3 direction = transform.position - player.transform.position;
        direction.y = 0f;
        direction.Normalize();

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;

            transform.position += direction * knockbackForce * Time.deltaTime;

            yield return null;
        }

        agent.isStopped = false;
    }

    void Die()
    {
        Debug.Log(enemyData.enemyName + " died!");

        Destroy(gameObject);
    }
}
