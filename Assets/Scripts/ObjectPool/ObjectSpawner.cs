using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public float cooldown;
    public ObjectPool pool;
    public string tag;
    [Header("Scanning")]
 
    public float maxDistance = 30f;

    [Header("Infos")]
    [ReadOnly]
    public float remainingCooldown;
    List<Vector3> destinations;
    // Start is called before the first frame update
    void Start()
    {
        destinations = new List<Vector3>();
        remainingCooldown = 0;
        for(int i = 1; i < pool.transform.childCount; i++)
        {
            Transform child = pool.transform.GetChild(i).transform;
            if (child != this.transform)
                destinations.Add(child.position);
        }
            
    }

    void FixedUpdate()
    {
        remainingCooldown -= Time.fixedDeltaTime;
        if(remainingCooldown <= 0)
        {
            spawnElement();
        }
    }

    private void spawnElement()
    {
        Transform spawnPos = gameObject.transform.GetChild(0);
        if (Physics.OverlapSphere(spawnPos.position, maxDistance, LayerMask.GetMask(tag)).Length > 0) 
            return;
        int index = UnityEngine.Random.Range(0, destinations.Count);
        GameObject x = pool.spawnObject(tag);
        if( x != null)
        {
            
            x.GetComponent<PoolElement>().setUp(pool, this.gameObject, Quaternion.LookRotation(spawnPos.forward, x.transform.up),spawnPos.position, destinations[index]);
            remainingCooldown = cooldown;
            if (this.tag.CompareTo("Vehicle") == 0)
            {
                Debug.Log("Spawn" + this.name);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Transform spawnPos = gameObject.transform.GetChild(0);
        Gizmos.DrawSphere(spawnPos.position, maxDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        PoolElement e = other.GetComponent<PoolElement>();
        if(e.Spawner != this && other.tag.CompareTo(this.tag) ==0)
        {
            e.release();
        }
    }
}
