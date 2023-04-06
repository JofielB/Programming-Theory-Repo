using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 3;
    [SerializeField]
    private float m_Force = 2;
    [SerializeField]
    private float m_Health = 4;
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
        transform.position += GetNextMovementDirection() * m_Speed * Time.deltaTime;
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

            collisionShape.PushBack(m_Force);
            collisionShape.Hit(m_Force);
            if (collisionShape.m_Health <= 0)
                m_Health++;
        }
        else if (IsAnAlly(collisionObject) &&
            CanWeFuse(collisionObject))
        {
            Shape collisionShape = collisionObject.GetComponent<Shape>();

            if (collisionShape.m_Health > m_Health)
            {
                collisionShape.IncreaseStats(m_Speed/5, m_Force/4, m_Health/2, transform.localScale.x);
                DestroyShape();
            }
        }
    }

    private void PushBack(float pushBackForce)
    {
        m_RigidBoidy.AddForce(GetNextMovementDirection() * -1 * pushBackForce, ForceMode.Impulse);
    }

    private void Hit(float hitForce)
    {
        m_Health -= hitForce;
    }

    private void CheckHealth()
    {
        if (m_Health <= 0)
            DestroyShape();
    }

    private void DestroyShape()
    {
        CreateExplosion();
        Destroy(gameObject);
    }

    private bool IsAnEnemy(GameObject objectiveGameObject)
    {
        return enemies.Contains(objectiveGameObject.tag);
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
            Debug.Log("KILL");
            ChangeDestination(triggerObject.transform.position);
        }
        else if (IsAnAlly(triggerObject) &&
            CanWeFuse(triggerObject))
        {
            Debug.Log("FUSE");
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

    private bool CanIKillIt(GameObject objectiveGameObject)
    {
        var otherHealth = objectiveGameObject.GetComponent<Shape>().m_Health;
        return otherHealth - m_Force <= 0;
    }

    private bool IsCloserThanCurrentDestination(GameObject objectiveGameObject)
    {
        return Vector3.Distance(transform.position, objectiveGameObject.transform.position) <
            Vector3.Distance(transform.position, m_CurrentDestination);
    }

    private bool CouldIDie(GameObject objectiveGameObject)
    {
        var otherForce = objectiveGameObject.GetComponent<Shape>().m_Force;
        return m_Health - otherForce <= 0;
    }

    private void ChangeDestination(Vector3 destination)
    {
        m_CurrentDestination = destination;
    }

    private bool IsAnAlly(GameObject objectiveGameObject)
    {
        return objectiveGameObject.tag.Equals(gameObject.tag);
    }

    private bool CanWeFuse(GameObject objectiveGameObject)
    {
        var otherHealth = objectiveGameObject.GetComponent<Shape>().m_Health;
        return otherHealth != m_Health;
    }

    private void IncreaseStats(float speed, float force, float health, float xScale)
    {
        m_Speed += speed;
        m_Force += force;
        m_Health += health;
        if (xScale > transform.localScale.x)
        {
            var scale = xScale + .2f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        }
        
    }
}
