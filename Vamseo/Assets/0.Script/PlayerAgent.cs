using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using System.Threading;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Transform parent;
    [SerializeField] RayPerceptionSensorComponent2D m_rayPerceptionSensorComponent2D;
    public Bullet bullet;
    public int hitcheck = 0;
    public int wallcheck = 0;
    private float bullettimer = 0;
    private float insideTimer = 0; //생존시간마다 보상점수 주기용 카운트
    public Player player;
    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
        hitcheck = 0;
        wallcheck = 0;
        player.HP = 100;
        //target.position = new Vector2(UnityEngine.Random.Range(-15, 15), UnityEngine.Random.Range(-15, 15));
    }
    private void Update()
    {
        bullettimer += Time.deltaTime;
        insideTimer += Time.deltaTime;

        if(hitcheck == 1)
        {
            AddReward(+10.0f);
            hitcheck = 0;
            Debug.Log("몬스터맞춤");
        }
        if(wallcheck == 1)
        {
            AddReward(-1f);
            wallcheck = 0;
            Debug.Log("벽맞춤");
        }
        /*if (insideTimer >= 5)
        {
            AddReward(+3f);
            Debug.Log("5초생존3점추가");
            insideTimer = 0;
        }*/
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    [SerializeField] float speed = 1.2f;
    Vector3 nextMove;
    float nextShot;
    Rotate nextrot;
    public override void OnActionReceived(ActionBuffers actions)
    {
        
        nextMove.x = actions.ContinuousActions[0]*3;
        nextMove.y = actions.ContinuousActions[1]*3;
        transform.Translate(nextMove * Time.deltaTime * speed);

        if (bullettimer > 2.0f)
        {
            GameObject detectedMonster = RayCastInfo(m_rayPerceptionSensorComponent2D);
            if (detectedMonster != null && detectedMonster.CompareTag("Monster"))
            {
                Vector2 targetDirection = detectedMonster.transform.position - transform.position;
                CreateBullet(targetDirection.normalized);
                bullettimer = 0;
            }
        }
    }

    public void Destroymonsters() //Endepisode용
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Destroy(monster);
        }
    }

    public void CreateBullet(Vector2 direction)
    {
        Bullet spawnedBullet = Instantiate(bullet, this.transform.position, Quaternion.identity);
        spawnedBullet.Initialize(direction); 
        spawnedBullet.SetPlayer(this); 
        spawnedBullet.transform.SetParent(parent);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster")) 
        {
            Debug.Log("impact monster");
            player.Hit(110);
            AddReward(-1.0f);
            Destroy(other.gameObject); //여기까지는 에이전트가 죽지않았을 때, 피 일부와 보상점수 일부 차감
            if(player.HP <= 0f)
            {
                EndEpisode();
                Debug.Log("초기화");
                Destroymonsters(); //에이전트가 사망하므로 에피소드 초기화 및 모든 몬스터 제거
            }
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("impact wall");
            AddReward(-5.0f);
            EndEpisode();
            Debug.Log("벽에부딫혀초기화");
            Destroymonsters(); //벽의 경우, 바로 초기화 안할 시 에이전트가 뚫고 지나갈 가능성이 있으므로 닿을 시 무조건 초기화 유지
        }
    }
    private GameObject RayCastInfo(RayPerceptionSensorComponent2D rayComponent)
    {
        var rayOutputs = RayPerceptionSensor.Perceive(rayComponent.GetRayPerceptionInput(), false)
                .RayOutputs;
        float minlength = 100000f;
        int minnum = 0;
        if (rayOutputs != null)
        {
            var lengthOfRayOutputs = RayPerceptionSensor
                    .Perceive(rayComponent.GetRayPerceptionInput(), false)
                    .RayOutputs
                    .Length;

            for (int i = 0; i < lengthOfRayOutputs; i++)
            {
                GameObject goHit = rayOutputs[i].HitGameObject;
                if (goHit != null)
                {
                    var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
                    var scaledRayLength = rayDirection.magnitude;

                    float rayHitDistance = rayOutputs[i].HitFraction * scaledRayLength;
                    if (rayHitDistance < minlength)
                    {
                        minlength = rayHitDistance;
                        minnum = i;
                    }
                    /*잘되나 확인
                    string dispStr;
                    dispStr = "__RayPerceptionSensor - HitInfo__:\r\n";
                    dispStr = dispStr + "GameObject name: " + goHit.name + "\r\n";
                    dispStr = dispStr + "GameObject tag: " + goHit.tag + "\r\n";
                    dispStr = dispStr + "Hit distance of Ray: " + rayHitDistance + "\r\n";
                    Debug.Log(dispStr);*/
                }
            }
            return rayOutputs[minnum].HitGameObject;
        }
        else { return null; }
    }
}