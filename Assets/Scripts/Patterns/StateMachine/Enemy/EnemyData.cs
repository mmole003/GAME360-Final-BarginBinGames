using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Information")]
    public string enemyName;

    [Header("Combat Stats")]
    public int maxHealth = 100;
    public int damage = 10;

    [Header("Movement")]
    public float movementSpeed = 3f;

    [Header("Detection")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
}