using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

    public Rigidbody2D rb; //剛体(アタッチ)
    public float moveSpeed;
    public float jumpForce;
    private float xInput;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        xInput = Input.GetAxisRaw("Horizontal");
        //入力により速度変更
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        //Space　- Jump
        if(Input.GetKeyDown(KeyCode.Space)){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
