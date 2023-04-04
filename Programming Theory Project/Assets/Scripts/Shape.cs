using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 3;

    private Vector3 m_CurrentDestination;
    private float m_xBoundary = 20;
    private float m_zBoundary = 15;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRandonDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentDestination != null)
            Move();
        if (HasReachDestination())
            GenerateRandonDestination();
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
        return Vector3.Distance(transform.position, m_CurrentDestination) <=  1.0f;
    }

    private void GenerateRandonDestination()
    {
        m_CurrentDestination = new Vector3(
            Random.Range(-m_xBoundary, m_xBoundary),
            0,
            Random.Range(-m_zBoundary, m_zBoundary)
        );
    }
}
