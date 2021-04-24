using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolElement))]
public class HardStuckFix : MonoBehaviour
{
    public float stuckfixPeriod;
    public float stuckThreshold;
    //fix hard stuck
    [ReadOnly]
    public Vector3 previousPos;
    [HideInInspector]
    public float checkStuckCooldown;
    // Start is called before the first frame update
    public void OnEnable()
    {
        previousPos = transform.position;
        checkStuckCooldown = stuckfixPeriod;
    }

    void FixedUpdate()
    {
        checkStuckCooldown -= Time.fixedDeltaTime;

        if(checkStuckCooldown <= 0)
        {
            Vector3 currentPos = transform.position;
            float distance = Vector3.Distance(currentPos, previousPos);
            if (distance <= stuckThreshold || Mathf.Approximately(distance, stuckThreshold)){
                GetComponent<PoolElement>().release();
            }
            checkStuckCooldown = stuckfixPeriod;
            previousPos = currentPos;
        }
    }
}
