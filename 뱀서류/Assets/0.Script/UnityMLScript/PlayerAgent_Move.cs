using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent_Move : Agent
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform target;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 1, 0);
        target.localPosition = new Vector3(Random.Range(-6, 6), 1, Random.Range(-6, 6));

        rb.velocity = Vector3.zero;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 velocity = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        rb.AddForce(velocity, ForceMode2D.Impulse);

        SetReward(-1 / MaxStep);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            SetReward(+1);
            EndEpisode();
        }
    }
}