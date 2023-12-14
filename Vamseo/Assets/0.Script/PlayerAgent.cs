using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using System.Threading;

public class PlayerAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Transform parent;
    public Bullet bullet;
    private float timer = 0;
    public int hitcheck = 0;
    public int wallcheck = 0;
    private float bullettimer = 0;
    System.TimeSpan ts;
    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
        hitcheck = 0;
        wallcheck = 0;
        //target.position = new Vector2(UnityEngine.Random.Range(-15, 15), UnityEngine.Random.Range(-15, 15));
    }
    private void Update()
    {
        timer += Time.deltaTime;
        bullettimer += Time.deltaTime;
        ts = System.TimeSpan.FromSeconds(timer);

        if(hitcheck == 1)
        {
            SetReward(+1.0f);
            hitcheck = 0;
            Debug.Log("총알맞음");
        }
        if(wallcheck == 1)
        {
            SetReward(-1.0f);
            wallcheck = 0;
            Debug.Log("벽맞음");
        }
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    [SerializeField] float speed = 1f;
    Vector3 nextMove;
    Vector3 nextShot;
    public override void OnActionReceived(ActionBuffers actions)
    {
        
        nextMove.x = actions.ContinuousActions[0]*3;
        nextMove.y = actions.ContinuousActions[1]*3;
        nextShot.x = (actions.ContinuousActions[2]+this.transform.position.x);
        nextShot.y = (actions.ContinuousActions[3]+this.transform.position.y);
        transform.Translate(nextMove * Time.deltaTime * speed);
        
        if(bullettimer > 2f)
        {
            CreateBullet(nextShot);
            bullettimer = 0;
        }
        
        /*
        Vector3 velocity = new Vector3(actions.ContinuousActions[0]/100, actions.ContinuousActions[1]/100, 0);
        rb.AddForce(velocity, ForceMode2D.Impulse);
        */
        SetReward(1 / MaxStep);
        if ( ts.Seconds >= 100.0f)
        {
            SetReward(+10.0f);
            EndEpisode();
            Destroymonster();
            Debug.Log("응싫어ㅋㅋ");
        }
    }

    public void Destroymonster()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void CreateBullet(Vector3 vec)
    {
        
        Bullet spawndbullet;
        spawndbullet = Instantiate(bullet, vec, transform.rotation * Quaternion.Euler(0, 0, -90f));
        spawndbullet.SetPlayer(this);
        spawndbullet.transform.SetParent(parent);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Debug.Log("impact monster");
            SetReward(-1.0f);
            EndEpisode();
            Destroymonster();
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("impact wall");
            SetReward(-1.0f);
            EndEpisode();
            Destroymonster();
        }
    }
}