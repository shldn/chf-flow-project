using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public bool debugSpawnRandom = false;
    public GameObject pf_enemy;
    public GameObject target;
    public float spawnRadius;
    public Vector2 spawnRateRange;
    public Vector2 spawnSizeRange;

    private float timeNextSpawn;

    private Transform rotationTransform;
	
	// Update is called once per frame
	void Update () {

        // spawn periodically
        if (Time.time > timeNextSpawn)
        {
            float spawnRate = spawnRateRange.x;

            // toggle this to switch between debug spawn rate and muse induced rate
            if (debugSpawnRandom)
                spawnRate = Random.Range(spawnRateRange.x, spawnRateRange.y);
            else if (MuseManager.Inst.MuseDetected)
                spawnRate = MuseManager.Inst.LastConcentrationMeasure * (spawnRateRange.y - spawnRateRange.x) + spawnRateRange.x;

            timeNextSpawn = (1f / spawnRate) + Time.time;
            
            // spawn the enemy
            SpawnEnemy();
        }
	}

    private void SpawnEnemy()
    {
        // calculate a random point on a hemisphere to spawn at
        float rotYRand = Random.Range(0f, 360f);
        float rotXRand = Random.Range(0f, 180f);

        // rotate a position according to the random angles above
        Vector3 posSpawn = Vector3.forward;
        posSpawn = Quaternion.Euler(-rotXRand, 0, 0) * posSpawn;
        posSpawn = Quaternion.Euler(0, rotYRand, 0) * posSpawn;
        posSpawn = posSpawn * spawnRadius + target.transform.position;

        // instantiate the enemy
        GameObject enemy = Instantiate(pf_enemy, posSpawn, Quaternion.identity);
        EnemyController enemyController = enemy.GetComponentInChildren<EnemyController>();

        // control the size of the enemy with debug values or biometric data
        float spawnSize = spawnRateRange.x;
        if (debugSpawnRandom)
            spawnSize = Random.Range(spawnSizeRange.x, spawnSizeRange.y);
        else
            spawnSize = MuseManager.Inst.LastMellowMeasure * (spawnSizeRange.y - spawnSizeRange.x) + spawnSizeRange.x;

        // initialize the enemy
        enemyController.Initialize(target, posSpawn, spawnSize);
    }
}
