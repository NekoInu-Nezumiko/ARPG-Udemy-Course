using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using UnityEngine;

public class Player : MonoBehaviour{

    //Inspectorに表示する理由はないのでコンポーネントを取得する
    private Rigidbody2D rb; //剛体(アタッチ)
    private Animator anim; //Animator
    [SerializeField] private float moveSpeed;
    [Header("Jump関連")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int jumpMaxCount;
    private int jumpCount;
    [Header("ダッシュ関連")]
    [SerializeField] private float dashSpeed; //dashの速度
    [SerializeField] private float dashDuration; //dashの長さ
    private float dashTime; //dashの残り時間
    [SerializeField] private float dashCooldown; //dashのCooldown時間
    private float dashCooldownTimer; //Cooldownまでの時間

    [Header("攻撃関連")]
    [SerializeField] private float comboTime = .3f;
    private float comboTimeWindow; //comboが発動できる時間枠（残り時間）
    private bool isAttacking;
    private int comboCounter;


    private float xInput; //Arrow-Key
    private int facingDir = 1; // right == 1 , left == -1
    private bool facingRight = true;  //right == true , left == false



    [Header("衝突関連")]
    [SerializeField] private LayerMask whatIsGround; //Layerを調べ、Groundなのか見分ける
    [SerializeField] private float groundCheckDistance; //PlayerからGroundまでの距離
    private bool isGrounded; //Ground or Not

    void Start(){
        rb = GetComponent<Rigidbody2D>(); //ComponentにRigidbody2Dがあれば割り当てる
        anim = GetComponentInChildren<Animator>(); //ChildにAnimatorがあれば割り当てる
    }

    void Update()
    {
        Movement(); //Move
        CheckInput(); //入力チェック
        CollisionChecks(); //衝突チェック
        //dash
        dashTime -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        //combo
        comboTimeWindow -= Time.deltaTime;
        
        FlipController(); //反転チェック
        AnimatorControllers();//Animation
    }

    public void AttackOver(){
        isAttacking = false;
        //combo
        comboCounter++;
        if(comboCounter > 2)
            comboCounter = 0;

    }

    private void CollisionChecks(){
        //transform.positionから下方向にgroundCheckDistanceだけ光線を射出し、
        //特定のレイヤ(whatIsGround)とぶつかるか調べる(※将来的に if isGrounded jumpCount = 0)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput(){
        //Arrow - Move
        xInput = Input.GetAxisRaw("Horizontal");
        //Z - Attack
        if(Input.GetKeyDown(KeyCode.Z)){
            StartAttackEvent();
        }
        //Space　- Jump(Only Grounded)
        if (Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
        //LeftShift - Dash
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            DashAbility();
        }
    }

    private void StartAttackEvent(){
        //air -> return
        if(!isGrounded)
            return;
        //combo_reset_conditions
        if (comboTimeWindow < 0)
            comboCounter = 0;
        isAttacking = true;
        comboTimeWindow = comboTime;
    }

    private void DashAbility(){
        //DashがCooldown中でなく、Attackもしていないとき
        if(dashCooldownTimer < 0 && !isAttacking){
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        }
    }

    private void Movement(){
        //攻撃中は移動しない
        if(isAttacking)
            rb.velocity = new Vector2(0,0);
        //dash(dash中は落下しない)
        else if(dashTime > 0)
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        else
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void Jump(){
        //複数回Jump
        if(isGrounded)
            jumpCount = jumpMaxCount;
        if(jumpCount <= 0)
            return;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpCount--;
    }

    private void AnimatorControllers(){
        //Animator - (UnityのAnimatorのParamと同一であることを確認)
        bool isMoving = rb.velocity.x != 0; //速度が0でなければTrue
        anim.SetFloat("yVelocity", rb.velocity.y); // -1 fall 1 jump
        anim.SetBool("isMoving", isMoving); //isMovingの状態でAnimationを切り替える
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
    }

    private void Flip(){
        facingDir *=  -1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);
    }

    private void FlipController(){
        if (rb.velocity.x > 0 && !facingRight)
            Flip();
        else if(rb.velocity.x < 0 && facingRight)
            Flip();
    }

    private void OnDrawGizmos() {
        //PlayerからgroundCheckDistance分の線をy方向に引く
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));

    }
}
