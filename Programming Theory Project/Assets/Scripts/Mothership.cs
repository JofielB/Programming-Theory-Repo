using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothership : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;

    ParticleSystem particles;
    ParticleSystem.EmissionModule emission;
    Behaviour halo;
    

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        emission = particles.emission;
        emission.enabled = false;
        halo = (Behaviour)GetComponent("Halo");
        halo.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        SetHiglightEffects(true);
    }

    private void OnMouseExit()
    {
        SetHiglightEffects(false);
    }

    private void OnMouseDown()
    {
        SpawnObject();
    }

    private void SetHiglightEffects(bool enableEffects)
    {
        if (particles != null)
        {
            emission.enabled = enableEffects;
            halo.enabled = enableEffects;
        }
    }

    private void SpawnObject()
    {
        if (objectToSpawn != null)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = 10;

            Instantiate(objectToSpawn, spawnPosition, transform.rotation);
        }
    }
}
