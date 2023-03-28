using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10.0f;

    Rigidbody2D rigid;
    Animator anim;
    float h;
    float v;

    public enum eKeyState : UInt16
    {
        KEY_STATE_UNPRESSED = 0,
        KEY_STATE_PRESSED = 1,
    }

    public eKeyState hKey;
    public eKeyState vKey;
    public bool isStateChanged = false;

    public bool hDown;
    public bool vDown;
    public bool hUp;
    public bool vUp;

    public bool isHorizonMove;
    bool isStopped = false;

    public float timer;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hKey = eKeyState.KEY_STATE_UNPRESSED;
        vKey = eKeyState.KEY_STATE_UNPRESSED;
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        hDown = Input.GetButtonDown("Horizontal");
        vDown = Input.GetButtonDown("Vertical");
        hUp = Input.GetButtonUp("Horizontal");
        vUp = Input.GetButtonUp("Vertical");

        isStateChanged = false;
        if (hDown)
        {
            isStateChanged = true;
            hKey = eKeyState.KEY_STATE_PRESSED;
        }
        else if(hUp)
        {
            isStateChanged = true;
            hKey = eKeyState.KEY_STATE_UNPRESSED;
        }
        if (vDown)
        {
            isStateChanged = true;
            vKey = eKeyState.KEY_STATE_PRESSED;
        }
        else if (vUp)
        {
            isStateChanged = true;
            vKey = eKeyState.KEY_STATE_UNPRESSED;
        }

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = h != 0;

        if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
            anim.SetBool("isChange", false);

        if (isStateChanged && !isStopped)
        {
            Client.instance.Move(transform.position.x, transform.position.y, h, v, (int)hKey, (int)vKey);
        }
    }

    void FixedUpdate()
    {
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);

        if (isStopped)
        {
            anim.SetBool("isChange", false);
        }
        else
        {
            rigid.velocity = moveVec * speed;
        }
    }

    public void ToggleStop()
    {
        isStopped = !isStopped;
    }
}
