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
    //RayPerceptionOutput.RayOutput rayOutput; 안댐 ㅅㅂ
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
            AddReward(+3.0f);
            hitcheck = 0;
            Debug.Log("총알맞음");
        }
        if(wallcheck == 1)
        {
            AddReward(-0.1f);
            wallcheck = 0;
            Debug.Log("벽맞음");
        }
        if (insideTimer >= 10)
        {
            AddReward(+3f);
            Debug.Log("10초생존3점추가");
            insideTimer = 0;
        }
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    [SerializeField] float speed = 1f;
    Vector3 nextMove;
    float nextShot;
    Rotate nextrot;
    public override void OnActionReceived(ActionBuffers actions)
    {
        
        nextMove.x = actions.ContinuousActions[0]*3;
        nextMove.y = actions.ContinuousActions[1]*3;
        /*
        nextShot.x = (actions.ContinuousActions[2]+this.transform.position.x);
        nextShot.y = (actions.ContinuousActions[3]+this.transform.position.y);*/
        //nextShot.z = actions.ContinuousActions[2];
        nextShot = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f) * 180f; // -1과 1 사이의 값을 -180과 180 사이의 각도로 변환
        transform.Translate(nextMove * Time.deltaTime * speed);

        if (bullettimer > 2f ) // && rayOutput.HitGameObject.CompareTag("Monster")) 얘도 안댐 ㅅㅂ 눌레퍼런스뜸 ㅜㅜ
        {
            GameObject detectedMonster = RayCastInfo(m_rayPerceptionSensorComponent2D);
            if (detectedMonster)
            {
                CreateBullet();
                bullettimer = 0;
            }
        }
        
        /*
        Vector3 velocity = new Vector3(actions.ContinuousActions[0]/100, actions.ContinuousActions[1]/100, 0);
        rb.AddForce(velocity, ForceMode2D.Impulse);
        */
    }

    public void Destroymonsters() //Endepisode용
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Destroy(monster);
        }
    }

    public void CreateBullet()
    {
        Bullet spawndbullet;
        spawndbullet = Instantiate(bullet, this.transform.position, (transform.rotation * Quaternion.Euler(0, 0, nextShot)));
        spawndbullet.SetPlayer(this);
        spawndbullet.transform.SetParent(parent);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster")) 
        {
            Debug.Log("impact monster");
            player.Hit(10);
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
            AddReward(-1.0f);
            EndEpisode();
            Debug.Log("초기화");
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
                    // Found some of this code to Denormalized length
                    // calculation by looking trough the source code:
                    // RayPerceptionSensor.cs in Unity Github. (version 2.2.1)
                    var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
                    var scaledRayLength = rayDirection.magnitude;

                    float rayHitDistance = rayOutputs[i].HitFraction * scaledRayLength;
                    if (rayHitDistance < minlength)
                    {
                        minlength = rayHitDistance;
                        minnum = i;
                    }
                    // Print info:
                    string dispStr;
                    dispStr = "__RayPerceptionSensor - HitInfo__:\r\n";
                    dispStr = dispStr + "GameObject name: " + goHit.name + "\r\n";
                    dispStr = dispStr + "GameObject tag: " + goHit.tag + "\r\n";
                    dispStr = dispStr + "Hit distance of Ray: " + rayHitDistance + "\r\n";
                    Debug.Log(dispStr);
                }
            }
            return rayOutputs[minnum].HitGameObject;
        }
        else { return null; }
    }
}