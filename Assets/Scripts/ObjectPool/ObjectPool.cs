using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

public class ObjectPool : MonoBehaviour
{
    [Serializable]
    public class ObjectPrefab
    {
        public String tag;
        public GameObject prefab;
        public int initialSize;
    }

    //public 

    public float frequency;

    Transform _target;
    private float cdTime;
    public List<ObjectPrefab> ObjectList;

    public Dictionary<String, Queue<GameObject>> pools;

    void Awake()
    {
        pools = new Dictionary<String, Queue<GameObject>>();
        foreach (ObjectPrefab ene in ObjectList)
        {
            Queue<GameObject> Pool = new Queue<GameObject>();
            for (int i = 0; i < ene.initialSize; i++)
            {
                GameObject x = Instantiate(ene.prefab) as GameObject;
                x.transform.SetParent(this.transform.GetChild(0));
                x.tag = ene.tag;
                x.SetActive(false);
               Pool.Enqueue(x);
            }
            pools.Add(ene.tag, Pool);
        }

        //cdTime = 1f / frequency;

    }

    private void Update()
    {
        if (frequency == 0)
            return;
        this.SpawnObjectByFreq();
    }

    public void Release(GameObject myObject)
    {
        myObject.SetActive(false);
        pools[myObject.tag].Enqueue(myObject.gameObject);
    }
    public GameObject spawnObject(String tag)
    {
        GameObject x = null;
        if (!pools.ContainsKey(tag))
        {
            Debug.Log("Tag " + tag + " does not exist");
            return x;
        }

        if (pools[tag].Count == 0)
        {
            return null;
            /*foreach (ObjectPrefab prefab in ObjectList)
            {
                if (prefab.tag.CompareTo(tag) == 0)
                {
                    x = Instantiate(prefab.prefab) as GameObject;
                    x.tag = tag;
                    x.transform.SetParent(this.transform.GetChild(0));
                    break;
                }
            }*/
        }
        else
        {
            x = pools[tag].Dequeue();
        }

        return x;
    }
    public void SpawnObjectByFreq()
    {
        //cdTime += Time.deltaTime;
        //if (cdTime < (1f / this.frequency))
          //  return;
        //int index = Random.Range(0, EnemyList.Count);
        //String tag = EnemyList[index].tag;
        //spawnEnemy(tag, 10 + 10 * playerLevel.level);
        //cdTime = 0;
    }


}
