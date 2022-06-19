using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityTemplateProjects.MultiplayerScripts;
using Random = UnityEngine.Random;


public class AiController : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Animator tp_CharacterAnimator;
    private Vector3 movementDirection;
    private Transform characterTransform;
    private float velocity;

    private AiWeaponManager weapon;
    private float originHeight;

    public float SprintingSpeed;
    public float WalkSpeed;
    //[SerializeField] internal Animator GunAnimator;
    public float SprintingSpeedWhenCrouched;
    public float WalkSpeedWhenCrouched;
    public float FiringRange = 30;
    public int FindPlayerMaxDis = 40;

    public Transform RayPoint;
    public Transform HeadPoint;

    public float Gravity = 9.8f;

    private GameObject[] PlayerList;
    private int WaitTime = 5; // 每五秒更新一次玩家列表
    private IEnumerator FindPlayerIE;
    public GameObject target;
    private Nav thisNav;
    private int AiShootingDistance = 10;

    private Vector3 offset = new Vector3(0, 0.3f, 0);
    public GameObject ai;
    Dictionary<byte, object> tmp_HitData = new Dictionary<byte, object>();


    private void Start()
    {
        //characterController = GetComponent<CharacterController>();
        ////        characterAnimator = GetComponentInChildren<Animator>();
        //characterTransform = transform;
        //originHeight = characterController.height;
        thisNav = this.GetComponent<Nav>();
        if (FindPlayerIE != null)
        {
            StopCoroutine(FindPlayerIE);
            FindPlayerIE = null;
        }

        FindPlayerIE = FindPlayer();
        StartCoroutine(FindPlayerIE);


        weapon = this.GetComponent<AiWeaponManager>();
    }


    private void Update()
    {
        if(this.target != null)
        {
            Ray ray = new Ray(RayPoint.position, target.transform.position- RayPoint.position + offset);
            //ai.transform.forward.Set(this.target.transform.position.x - HeadPoint.position.x, 0, this.target.transform.position.z - HeadPoint.position.z);
            ai.transform.LookAt(this.target.transform);

            bool isCollider = Physics.Raycast(ray, out RaycastHit hit);
            //Debug.Log(target.transform.position);

            //weapon.Shooting();
            if (isCollider)
            {
                if (hit.collider.gameObject == this.target && hit.distance < FiringRange)//如果可以被击中 15m开枪
                {
                    StopFollow();
                    //面向玩家
                    //this.transform.forward.Set(this.target.transform.position.x - RayPoint.position.x, 0, this.target.transform.position.z - RayPoint.position.z);
                    //储存命中位置信息
                    tmp_HitData.Clear();
                    tmp_HitData.Add(0, hit.point);
                    tmp_HitData.Add(1, hit.normal);
                    tmp_HitData.Add(2, hit.collider.tag);
                    
                    //播放开枪动画
                    weapon.Shooting();
                    //掉血
                    

                }
                else
                {
                    //追逐
                    Follow();

                    //播放动画
                }
            }
            
        }
        else
        {

        }




    }


    private IEnumerator FindPlayer()
    {
        
        while(true)
        {
            PlayerList = GameObject.FindGameObjectsWithTag("player");
            this.target = null;
            foreach (GameObject player in PlayerList)
            {
                if ((player.transform.position - this.transform.position).sqrMagnitude < FindPlayerMaxDis * FindPlayerMaxDis)//距离平方比较
                {
                    this.target = player;//以该玩家位目标
                    Follow();
                    break;
                }
            }
            if (this.target == null)
            {
                StopFollow();
            }
            Debug.Log("find player ing...");
            yield return new WaitForSeconds(WaitTime);

        }
        
    }

    public void Follow()
    {
        thisNav.SetDestination(target.transform);
        tp_CharacterAnimator.SetFloat("Velocity", 8);
        tp_CharacterAnimator.SetFloat("Movement_Y",1);

    }

    public void StopFollow()
    {
        thisNav.Stop();
        tp_CharacterAnimator.SetFloat("Velocity", 0);
    }

    //internal void SetupAnimator(Animator _animator)
    //{
    //    Debug.Log($"Execute! the animator is empty??? {_animator == null}");
    //    characterAnimator = _animator;
    //}



    public void Damage(float DamageRate = 0.15f,int DamagePerShoot = 10)
    {
        if (Random.value < DamageRate)
        {
            if (this.target.TryGetComponent(out IDamager tmp_Damager))
            {
                tmp_Damager.TakeDamage(DamagePerShoot);
            }
            
            RaiseEventOptions tmp_RaiseEventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
            SendOptions tmp_SendOptions = SendOptions.SendReliable;
            PhotonNetwork.RaiseEvent((byte)Scripts.Weapon.EventCode.HitObject, tmp_HitData, tmp_RaiseEventOptions, tmp_SendOptions);
        }
    }



}