using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    [SerializeField] private RayPerceptionSensorComponent3D raySensor;
    public override void OnEpisodeBegin()
    {
        // 초기 에피소드 설정
        transform.localPosition = new Vector3(0, 1, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 에이전트의 움직임 제어
        Vector3 movement = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        // 시간에 따른 리워드 감소
        SetReward(+1 / MaxStep);
    }

    private void OnCollisionEnter(Collision other)
    {
        // 몬스터와의 충돌 감지
        if (other.gameObject.CompareTag("Monster"))
        {
            SetReward(-1f); // 몬스터와 충돌 시 페널티
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 수동 테스트용 휴리스틱 함수
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
