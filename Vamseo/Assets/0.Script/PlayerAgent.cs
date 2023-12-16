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

public class PlayerAgent : Agent
{
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Transform parent;
    //RayPerceptionOutput.RayOutput rayOutput; 안댐 ㅅㅂ
    public Bullet bullet;
    private float timer = 0;
    public int hitcheck = 0;
    public int wallcheck = 0;
    private float bullettimer = 0;
    private float insideTimer;
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
        insideTimer += Time.deltaTime;

        if(hitcheck == 1)
        {
            SetReward(+3.0f);
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


        if (bullettimer > 2f) // && rayOutput.HitGameObject.CompareTag("Monster")) 얘도 안댐 ㅅㅂ 눌레퍼런스뜸 ㅜㅜ
        {
            CreateBullet();
            bullettimer = 0;
        }
        
        /*
        Vector3 velocity = new Vector3(actions.ContinuousActions[0]/100, actions.ContinuousActions[1]/100, 0);
        rb.AddForce(velocity, ForceMode2D.Impulse);
        */
        if (insideTimer >= 10) //그냥 10초 생존할 때마다 1점 주고, endepisode는 없애는게 낫지안나? 굳이 특정시간까지 생존 시 초기화를 해야되나?
        {
            SetReward(+1f); //근데 그러면 endepsiode가 안먹으니까 10초마다 10초를 카운트하는 전용 타이머를 새로 만들어야할듯
            Debug.Log("응싫어ㅋㅋ");
            insideTimer = 0;
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

    public void CreateBullet()
    {
        
        Bullet spawndbullet;
        spawndbullet = Instantiate(bullet, this.transform.position, (transform.rotation * Quaternion.Euler(0, 0, nextShot)));
        spawndbullet.SetPlayer(this);
        spawndbullet.transform.SetParent(parent);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster")) //생각해보니까 충돌하면 초기화되니까 -1점 
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