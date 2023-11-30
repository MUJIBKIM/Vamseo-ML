using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    [SerializeField] Transform target;

    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
        target.position = new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0);
        Debug.Log("응안돼ㅋㅋ");
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
        Debug.Log("응꺼져ㅋㅋ");
    }

    [SerializeField] float speed = 1f;
    Vector3 nextMove;
    public override void OnActionReceived(ActionBuffers actions)
    {
        nextMove.x = actions.ContinuousActions[0];
        nextMove.y = actions.ContinuousActions[1];
        float checktime = Time.deltaTime;
        transform.Translate(nextMove * Time.deltaTime * speed);
        checktime += Time.deltaTime;
        Debug.Log(checktime);
        if(checktime > 30.0f)
        {
            SetReward(+10.0f);
            EndEpisode();
        }
        Debug.Log("응싫어ㅋㅋ");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Debug.Log("impact monster");
            SetReward(-5.0f);
            EndEpisode();
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("impact wall");
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}