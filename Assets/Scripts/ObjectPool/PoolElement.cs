using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PedestrianController))]
public class PoolElement : MonoBehaviour
{
    [ReadOnly]
    public GameObject Spawner;
    [ReadOnly]
    public ObjectPool pool;

    public void release()
    {
        pool.Release(this.gameObject);
    }

    public void setUp(ObjectPool pool, GameObject Spawner, Quaternion lookRotation, Vector3 start, Vector3 destination)
    {
        this.pool = pool;
        this.Spawner = Spawner;
        this.transform.rotation= lookRotation;
        this.transform.position = start;
        this.GetComponent<PedestrianController>().destination = destination;
        gameObject.SetActive(true);
    }
}
