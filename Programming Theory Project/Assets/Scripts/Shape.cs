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
        Debug.DrawLine(transform.position, m_CurrentDestination);
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
        return Vector3.Distance(transform.position, m_CurrentDestination) <= 1.5f;
    }

    private void GenerateRandonDestination()
    {
        Vector3 randonDestination = new Vector3(
            Random.Range(-m_XBoundary, m_XBoundary),
            1,
            Random.Range(-m_ZBoundary, m_ZBoundary)
        );

        ChangeDestination(randonDestination);
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
            CanIKillIt(triggerObject))
        {
            Debug.Log("Kill");
            ChangeDestination(triggerObject.transform.position);
        }
        else if (IsAnEnemy(triggerObject) &&
            CouldIDie(triggerObject))
        { 
            Debug.Log("RUN");
            
            Vector3 directionAwayFromThreat = (triggerObject.transform.position - transform.position).normalized;
            Vector3 directionWithMagnitud = directionAwayFromThreat * 10 * -1;
            directionWithMagnitud.y = 1;
            ChangeDestination(directionWithMagnitud);
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

    private bool CouldIDie(GameObject gameObject)
    {
        var otherForce = gameObject.GetComponent<Shape>().force;
        return health - otherForce <= 0;
    }

    private void ChangeDestination(Vector3 destination)
    {
        m_CurrentDestination = destination;
    }
}
