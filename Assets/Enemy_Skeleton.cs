using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Entity{

    bool isAttacking;

    [Header("移動関連")]
    [SerializeField] private float moveSpeed;

    [Header("Player検出")]
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private LayerMask whatIsPlayer;

    private RaycastHit2D isPlayerDetected; //Playerを検出すればTrue

    protected override void Start() {
        base.Start();
    }

    protected override void Update(){
        base.Update();

        if (isPlayerDetected){
            if (isPlayerDetected.distance > 1){
                rb.velocity = new Vector2(moveSpeed * 1.5f * facingDir, rb.velocity.y);
                Debug.Log("I see the player");
                isAttacking = false;
            }
            else{
                Debug.Log("ATTACK" + isPlayerDetected.collider.gameObject.name);
                isAttacking = true;

            }
        }

        if (!isGrounded || isWallDetected)
            Flip();
        Movement();

    }

    private void Movement(){
        if(!isAttacking)
            rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
    }

    protected override void CollisionChecks(){
        base.CollisionChecks();
        //Plater検出(前方のみ)
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right, playerCheckDistance * facingDir, whatIsPlayer);
    }

    protected override void OnDrawGizmos(){
        base.OnDrawGizmos();
        //Skelton to Player
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + playerCheckDistance * facingDir, transform.position.y));
    }

}
