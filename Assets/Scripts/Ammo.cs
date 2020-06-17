using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class Ammo : MonoBehaviour
{
	public Rigidbody2D body;

	public LinkedListNode<Ammo> node;

    private Vector2 moveDir = Vector2.zero;
    public Transform MyTransform { get; set; }
    protected PoolObject myPoolObject;

    private float minTan;

    private float maxTan;

    private long preContactTicks;

    private long minAlphaTickCounter;


    bool withBounder = false;

    private void Awake()
	{

        myPoolObject = GetComponent<PoolObject>();
        body = base.GetComponent<Rigidbody2D>();
        MyTransform = transform;

        this.minAlphaTickCounter = 0L;


        this.minTan = Mathf.Tan(0.0610865243f);
        this.maxTan = Mathf.Tan(1.50970984f);
    }


    private void OnCollisionEnter2D(Collision2D coll)
    {

        OnContact();

        if (coll.gameObject.tag == "BrickKill")
        {
            GameManager.Singlton.OnContactBrickKill(coll.gameObject);
        }
        else if (coll.gameObject.tag == "BrickShild")
        {
            GameManager.Singlton.OnContactBrick(coll.gameObject);
        }

        if (coll.gameObject.tag == "BottomBounder")
        {
            DisabledObject();
            GameManager.Singlton.EndShooting();
        }
       
       
    }

    public void Push(Vector2 direct)
    {
        moveDir = direct;
        myPoolObject.ActivatePoolOject();
        body.isKinematic = false;

        body.AddForce(moveDir, ForceMode2D.Impulse);
    }

 
    private void OnContact()
	{
        SoundMenager.Singlton.PlaySound(SoundGame.ContactBrick);

        Vector2 velocity = this.body.velocity;
        float num = velocity.y / velocity.x;
        float num2 = Time.timeScale * 0.3f;

        if (Mathf.Abs(num) < this.minTan || Mathf.Abs(num) > this.maxTan)
        {
            if (this.minAlphaTickCounter > 0L)
            {
                this.minAlphaTickCounter += (DateTime.Now.Ticks - this.preContactTicks) * (long)Mathf.FloorToInt(Time.timeScale);
            }
            else
            {
                this.minAlphaTickCounter = 1L;
            }

            if (!withBounder && TimeSpan.FromTicks(this.minAlphaTickCounter).TotalSeconds > 3.0)
            {
                num2 *= (float)TimeSpan.FromTicks(this.minAlphaTickCounter).TotalSeconds / 3f;
                Vector3 v = Vector3.zero;
                if (Mathf.Abs(num) > this.maxTan)
                {
                    if (MyTransform.position.x < -4.017857f)
                    {
                        v = Vector3.right * num2;
                    }
                    else if (MyTransform.position.x > 4.017857f)
                    {
                        v = -Vector3.right * num2;
                    }
                    else
                    {
                        v = Vector3.right * ((velocity.x <= 0f) ? (-num2) : num2);
                    }
                }
                else
                {
                    v = Vector3.down * num2;
                }
                this.body.AddForce(v, ForceMode2D.Impulse);
            }
        }
        else
        {
            this.minAlphaTickCounter = 0L;
        }

        this.preContactTicks = DateTime.Now.Ticks;

    }

    public void DisabledObject()
    {
        myPoolObject.DisablePoolObject();
    }
}
