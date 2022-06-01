using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityTemplateProjects.MultiplayerScripts;

public class FPCharacterControllerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Animator tp_CharacterAnimator;
    private Vector3 movementDirection;
    private Transform characterTransform;
    private float velocity;


    private bool isCrouched = false;
    private bool isGlid = false;
    private float originHeight;

    public float SprintingSpeed;
    public float WalkSpeed;

    public float SprintingSpeedWhenCrouched;
    public float WalkSpeedWhenCrouched;

    public float Gravity = 9.8f;
    public float GlidLimit = 5;//滑铲速度下限，高于该速度即可滑铲 
    public float GlidSpeedMax = 15; //滑铲初最高速，会缓慢下降。下降到glidlimit转换为蹲着

    public float JumpHeight;
    public float CrouchHeight = 1f;

    public float CurrentSpeed { get; private set; }
    private IEnumerator crouchCoroutine;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
//        characterAnimator = GetComponentInChildren<Animator>();
        characterTransform = transform;
        originHeight = characterController.height;
    }


    private void Update()
    {
        //CurrentSpeed = WalkSpeed;
        if(isGlid)
        {
            DoGlid();
            //return;
        }
        else
        {
            if (isCrouched)
            {
                CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
            }
            else
            {
                CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;
            }
        }


        
        if (characterController.isGrounded)
        {
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");
            if (!isGlid)
            {
                //若不在滑铲状态才改变运动方向，滑铲中不可改变运动方向
                movementDirection =
                    characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical)).normalized;
            }
            
            //if (isCrouched)
            //{
            //    CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
            //}
            //else
            //{
            //    CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;
            //}

            if (Input.GetButtonDown("Jump"))
            {
                if(isGlid)
                {
                    UnGlid();
                    movementDirection.y = JumpHeight*1.2f;
                }
                else
                {
                    movementDirection.y = JumpHeight;

                }
            }

            if (Input.GetKeyDown(KeyCode.C) && isGlid == false)
            {
                if(CurrentSpeed > GlidLimit)
                {
                    CurrentSpeed = GlidSpeedMax;
                    characterController.height = CrouchHeight;
                    isGlid = true;
                    tp_CharacterAnimator.SetBool("isGlid", true);
                    characterAnimator.SetBool("isGlid", true);
                    //开滑！
                    //速度设为glidSpeedMax
                    //开一个速度不断下降的协程，下降到阈值关闭协程并转换为蹲着
                    //滑铲有3s cd
                    //播放动画
                }
                else
                {
                    var tmp_CurrentHeight = isCrouched ? originHeight : CrouchHeight;
                    if (crouchCoroutine != null)
                    {
                        StopCoroutine(crouchCoroutine);
                        crouchCoroutine = null;
                    }

                    crouchCoroutine = DoCrouch(tmp_CurrentHeight);
                    StartCoroutine(crouchCoroutine);
                    isCrouched = !isCrouched;
                }
                
            }

            if (characterAnimator != null)
            {
                characterAnimator.SetFloat("Velocity",
                    CurrentSpeed * movementDirection.normalized.magnitude,
                    0.25f,
                    Time.deltaTime);

                tp_CharacterAnimator.SetFloat("Velocity",
                    CurrentSpeed * movementDirection.normalized.magnitude,
                    0.25f,
                    Time.deltaTime);
                tp_CharacterAnimator.SetFloat("Movement_X", tmp_Horizontal, 0.25f, Time.deltaTime);
                tp_CharacterAnimator.SetFloat("Movement_Y", tmp_Vertical, 0.25f, Time.deltaTime);
                //暂时注释两行 attention!
            }
        }

        //if (isCrouched)
        //{
        //    CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouched : WalkSpeedWhenCrouched;
        //}
        //else
        //{
        //    CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;
        //}

        movementDirection.y -= Gravity * Time.deltaTime;
        var tmp_Movement = CurrentSpeed * Time.deltaTime * movementDirection;
        characterController.Move(tmp_Movement);
    }


    private IEnumerator DoCrouch(float _target)
    {
        float tmp_CurrentHeight = 0;
        while (Mathf.Abs(characterController.height - _target) > 0.1f)
        {
            yield return null;
            characterController.height =
                Mathf.SmoothDamp(characterController.height, _target,
                    ref tmp_CurrentHeight, Time.deltaTime * 5);
        }
    }

    internal void SetupAnimator(Animator _animator)
    {
        //Debug.Log($"Execute! the animator is empty??? {_animator == null}");
        characterAnimator = _animator;
    }

    private void DoGlid()
    {
        if (CurrentSpeed < GlidLimit)
        {
            UnGlid();
            return;
        }
        float tmp_What = 0;
        CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, GlidLimit - 0.2f,
                ref tmp_What, Time.deltaTime * 7);
        //慢慢减速 这个减速对应的秒还是没有搞得太清楚
        movementDirection.y -= Gravity * Time.deltaTime;
        var tmp_Movement2 = CurrentSpeed * Time.deltaTime * movementDirection;
        characterController.Move(tmp_Movement2);

        //射线检测 若撞到敌人将造成伤害
        Ray ray = new Ray(this.gameObject.transform.position, this.gameObject.transform.forward);
        bool isCollider = Physics.Raycast(ray, out RaycastHit hit);
        if (isCollider)
        {
            Debug.Log(hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "ai" && hit.distance < 3)//如果可以被击中 15m开枪
            {
                if (hit.collider.gameObject.TryGetComponent(out IDamager tmp_Damager))
                {
                    tmp_Damager.TakeDamage(50);
                }
            }
        } 
    }

    private void UnGlid()
    {
        //从滑铲中脱离。
        isGlid = false;
        CurrentSpeed = GlidLimit-2;
        tp_CharacterAnimator.SetBool("isGlid", false);
        characterAnimator.SetBool("isGlid", false);
        characterController.height = originHeight;
        return;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 销毁当前游戏物体
        //Destroy(this.gameObject);
        if(collision.gameObject.tag == "ai")
            Debug.Log(this.gameObject.name + "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        Debug.Log(collision.gameObject.name + "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
    }
}