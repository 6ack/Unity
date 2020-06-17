using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickKill : MonoBehaviour {

    protected PoolObject myPoolObject;
    public GameObject Effect;

    private void Awake()
    {
        myPoolObject = GetComponent<PoolObject>();
    }

    public void ActivateOject()
    {
        myPoolObject.ActivatePoolOject();
    }

    public void DisabledObject()
    {
        ActiveEffect();
        myPoolObject.DisablePoolObject();
    }

    public void ActiveEffect()
    {
        if (Effect != null)
            Instantiate(Effect, transform.position, Quaternion.identity);
    }

}
