using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour{
    
    //共通要素
    protected Rigidbody2D rb;
    protected Animator anim;

    [Header("衝突関連")]
    [SerializeField] protected Transform groundCheck; //gizmos(ground)用
    [SerializeField] protected float groundCheckDistance; //EntityからGroundまでの距離
    [Space]
    [SerializeField] protected Transform wallCheck; // gizmos(wall)用
    [SerializeField] protected float wallCheckDistance; //Entityからwallまでの距離
    [SerializeField] protected LayerMask whatIsGround; //Layerを調べ、Groundなのか見分ける
    protected bool isGrounded; //Ground or Not
    protected bool isWallDetected; //Wall or Not

    
    protected int facingDir = 1; // right == 1 , left == -1
    protected bool facingRight = true;  //right == true , left == false

    protected virtual void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        //もし値を代入しなければ、オブジェクト自体のTransformを使用する
        if(wallCheck == null)
            wallCheck = transform;

    }

    protected virtual void Update(){
        CollisionChecks();
    }

    //----Collision-------
    protected virtual void CollisionChecks(){
        //始点, 方向, 長さ, 検知レイヤ
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDir, whatIsGround);
    }   

    //-----Flip------
    protected virtual void Flip(){
        facingDir *=  -1;
        facingRight = !facingRight;
        transform.Rotate(0,180,0);
    }

    //----Gizmos--------
    protected virtual void OnDrawGizmos() {
        //下方向へ線(groundCheck)
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        //進行方向へ線(WallCheck)
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir , wallCheck.position.y));
    }
}
