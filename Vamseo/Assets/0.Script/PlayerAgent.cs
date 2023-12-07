using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class PlayerAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;
    private float timer = 0;
    System.TimeSpan ts;
    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
        //target.position = new Vector2(UnityEngine.Random.Range(-15, 15), UnityEngine.Random.Range(-15, 15));
    }
    private void Update()
    {
        timer += Time.deltaTime;
        ts = System.TimeSpan.FromSeconds(timer);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    [SerializeField] float speed = 1f;
    Vector3 nextMove;
    public override void OnActionReceived(ActionBuffers actions)
    {
        
        nextMove.x = actions.ContinuousActions[0]*3;
        nextMove.y = actions.ContinuousActions[1]*3;
        transform.Translate(nextMove * Time.deltaTime * speed);
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