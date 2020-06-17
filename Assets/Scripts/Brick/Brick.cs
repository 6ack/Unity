using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int brickCount = 1;
    public int brickMaxCount = 1;

    private TextMesh text;

    public Transform MyTransform { get; set; }
    protected PoolObject myPoolObject;


    private bool move = false;
    private Vector3 moveToDown;

    [HideInInspector]
    public int line;
    [HideInInspector]
    public int NumberBick;

    public SpriteRenderer RendererS { get; set; }
    public GameObject brickParticle;

    private void Awake()
    {
        text = GetComponentInChildren<TextMesh>();
        myPoolObject = GetComponent<PoolObject>();
        RendererS = GetComponent<SpriteRenderer>();
        MyTransform = transform;
        UpdateCount();
    }


  

  


    private void Update()
    {
        if (!move)
            return;

        MyTransform.position += Vector3.down * 0.3f;

        if (moveToDown.y >= MyTransform.position.y)
        {
            MyTransform.position = moveToDown;
            move = false;
        }
    }

    public void MoveToY(float Y)
    {

        moveToDown = MyTransform.position;
        moveToDown.y = Y;
        move = true;
     }

    public bool IsActive()
    {
        return myPoolObject.active;
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
        if(brickParticle != null)
        Instantiate(brickParticle, transform.position, Quaternion.identity);
    }

    public void UpdateCount()
    {
        text.text = brickCount.ToString();
    }
}
