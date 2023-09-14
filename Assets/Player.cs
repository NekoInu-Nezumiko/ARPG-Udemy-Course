using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

    //Inspectorに表示する理由はないのでコンポーネントを取得する
    private Rigidbody2D rb; //剛体(アタッチ)
    private Animator anim; //Animator
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private float xInput; //Arrow-Key
    private int facingDir = 1; // right == 1 , left == -1
    private bool facingRight = true;  //right == true , left == false

    void Start(){
        rb = GetComponent<Rigidbody2D>(); //ComponentにRigidbody2Dがあれば割り当てる
        anim = GetComponentInChildren<Animator>(); //ChildにAnimatorがあれば割り当てる
    }

    void Update(){
        Movement(); //Move
        CheckInput(); //入力チェック
        FlipController(); //反転チェック
        AnimatorControllers();//Animation
    }

    private void CheckInput(){
        //Arrow - Move
        xInput = Input.GetAxisRaw("Horizontal");
        //Space　- Jump
        if (Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
    }

    private void Movement(){
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void AnimatorControllers(){
        //Animator - Move
        bool isMoving = rb.velocity.x != 0; //速度が0でなければTrue
        anim.SetBool("isMoving", isMoving);
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
}
