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
    [SerializeField] private float jumpForce;
    
    [Header("ダッシュ関連")]
    [SerializeField] private float dashSpeed; //dashの速度
    [SerializeField] private float dashDuration; //dashの長さ
    private float dashTime; //dashの残り時間
    [SerializeField] private float dashCooldown; //dashのCooldown時間
    private float dashCooldownTimer; //Cooldownまでの時間


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
        
        FlipController(); //反転チェック
        AnimatorControllers();//Animation
    }

    private void CollisionChecks(){
        //transform.positionから下方向にgroundCheckDistanceだけ光線を射出し、
        //特定のレイヤ(whatIsGround)とぶつかるか調べる(※将来的に if isGrounded jumpCount = 0)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput(){
        //Arrow - Move
        xInput = Input.GetAxisRaw("Horizontal");
        //Space　- Jump(Only Grounded)
        if (Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
        //LeftShift - Dash
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            DashAbility();
        }
    }

    private void DashAbility(){
        if(dashCooldownTimer < 0){
            dashCooldownTimer = dashCooldown;
            dashTime = dashDuration;
        }
    }

    private void Movement(){
        //dash(dash中は落下しない)
        if(dashTime > 0)
            rb.velocity = new Vector2(xInput * dashSpeed, 0);
        else
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void Jump(){
        if(isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void AnimatorControllers(){
        //Animator - Move
        bool isMoving = rb.velocity.x != 0; //速度が0でなければTrue
        anim.SetFloat("yVelocity", rb.velocity.y); // -1 fall 1 jump
        anim.SetBool("isMoving", isMoving); //isMovingの状態でAnimationを切り替える
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
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
