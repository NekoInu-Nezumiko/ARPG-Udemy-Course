using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour{
    
    //共通要素
    protected Rigidbody2D rb;
    protected Animator anim;

    [Header("衝突関連")]
    [SerializeField] protected Transform groundCheck; //gizmos用
    [SerializeField] protected LayerMask whatIsGround; //Layerを調べ、Groundなのか見分ける
    [SerializeField] protected float groundCheckDistance; //PlayerからGroundまでの距離
    protected bool isGrounded; //Ground or Not

    
    protected int facingDir = 1; // right == 1 , left == -1
    protected bool facingRight = true;  //right == true , left == false

    protected virtual void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

    }

    protected virtual void Update(){
        CollisionChecks();
    }

    //----Collision-------
    protected virtual void CollisionChecks(){
        //transform.positionから下方向にgroundCheckDistanceだけ光線を射出し、
        //特定のレイヤ(whatIsGround)とぶつかるか調べる(※将来的に if isGrounded jumpCount = 0)
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    //-----Flip------
    protected virtual void Flip(){
        facingDir *=  -1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);
    }

    //----Gizmos--------
    protected virtual void OnDrawGizmos() {
        //PlayerからgroundCheckDistance分の線をy方向に引く
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));

    }
}
