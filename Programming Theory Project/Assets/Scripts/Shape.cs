using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 3;
    [SerializeField]
    private float force = 1;
    [SerializeField]
    private float health = 2;
    [SerializeField]
    private List<string> enemies = new List<string>();
    [SerializeField]
    GameObject explosionEffect;

    private Rigidbody m_RigidBoidy;
    private Vector3 m_CurrentDestination;
    private float m_XBoundary = 20;
    private float m_ZBoundary = 15;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRandonDestination();
        m_RigidBoidy = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentDestination != null)
            Move();
        if (HasReachDestination())
            GenerateRandonDestination();

        CheckHealth();
    }

    private void Move()
    {
        transform.position += GetNextMovementDirection() * movementSpeed * Time.deltaTime;
    }

    private Vector3 GetNextMovementDirection()
    {
        return (m_CurrentDestination - transform.position).normalized;
    }

    private bool HasReachDestination()
    {
        return Vector3.Distance(transform.position, m_CurrentDestination) <= 1.0f;
    }

    private void GenerateRandonDestination()
    {
        m_CurrentDestination = new Vector3(
            Random.Range(-m_XBoundary, m_XBoundary),
            0,
            Random.Range(-m_ZBoundary, m_ZBoundary)
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;

        if (IsAnEnemy(collisionObject))
        {
            Shape collisionShape = collisionObject.GetComponent<Shape>();

            collisionShape.PushBack(force);
            collisionShape.Hit(force);
            if (collisionShape.health <= 0)
                health++;
        }
    }

    private void PushBack(float pushBackForce)
    {
        m_RigidBoidy.AddForce(GetNextMovementDirection() * -1 * pushBackForce, ForceMode.Impulse);
    }

    private void Hit(float hitForce)
    {
        health -= hitForce;
    }

    private void CheckHealth()
    {
        if (health <= 0)
            DestroyShape();
    }

    private void DestroyShape()
    {
        CreateExplosion();
        Destroy(gameObject);
    }

    private bool IsAnEnemy(GameObject gameObject)
    {
        return enemies.Contains(gameObject.tag);
    }

    private void CreateExplosion()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject triggerObject = other.gameObject;

        if (IsAnEnemy(triggerObject) &&
            CanIKillIt(triggerObject) &&
            IsCloserThanCurrentDestination(triggerObject))
        {
            m_CurrentDestination = triggerObject.transform.position;
        }
    }

    private bool CanIKillIt(GameObject gameObject)
    {
        var otherHealth = gameObject.GetComponent<Shape>().health;
        return otherHealth - force <= 0;
    }

    private bool IsCloserThanCurrentDestination(GameObject gameObject)
    {
        return Vector3.Distance(transform.position, gameObject.transform.position) <
            Vector3.Distance(transform.position, m_CurrentDestination);
    }
}
