using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    public bool active { get; set; } //is this pool asteroid active or not?

    /// <summary>
    /// Disables a pool asteroid.
    /// </summary>
    public void DisablePoolObject()
    {
        this.active = false;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Enables a pool asteroid.
    /// </summary>
    public void ActivatePoolOject()
    {
        this.active = true;
        this.gameObject.SetActive(true);
    }
}
