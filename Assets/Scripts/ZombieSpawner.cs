using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject ZombiePrefab;
    public DestructibleController mDestructibleController;

    public void SpawnZombie()
    {
        Vector3 positionToSpawn = transform.position;
        positionToSpawn.y = 1f;
        var spawnedZombie = Instantiate(ZombiePrefab, positionToSpawn, transform.rotation) as GameObject;
        spawnedZombie.GetComponent<Zombie>().mDestructibleController = mDestructibleController;
        spawnedZombie.name = string.Format("{0}:{1}", spawnedZombie.GetInstanceID(), spawnedZombie.name);
    }
}