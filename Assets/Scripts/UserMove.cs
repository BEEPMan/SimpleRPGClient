using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserMove : MonoBehaviour
{
    public float speed = 10.0f;

    public UInt64 playerId = 0;

    public Rigidbody2D rigid;
    Animator anim;

    public float h;
    public float v;
    public bool hDown;
    public bool vDown;
    public bool hUp;
    public bool vUp;

    public enum eKeyState : UInt16
    {
        KEY_STATE_UNPRESSED = 0,
        KEY_STATE_PRESSED = 1,
    }

    public eKeyState hKey;
    public eKeyState vKey;

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
        if (hDown)
        {
            isHorizonMove = true;
            hDown = false;
        }
        else if (vDown)
        {
            isHorizonMove = false;
            vDown = false;
        }
        else if (hUp || vUp)
        {
            isHorizonMove = h != 0;
            hUp = false;
            vUp = false;
        }

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

    public void SetMoveInfo(float mH, float mV, int mHKey, int mVKey)
    {
        h = mH;
        v = mV;
        if(hKey != (eKeyState)mHKey)
        {
            if (hKey == eKeyState.KEY_STATE_UNPRESSED)
                hDown = true;
            else
                hUp = true;
        }
        if (vKey != (eKeyState)mVKey)
        {
            if (vKey == eKeyState.KEY_STATE_UNPRESSED)
                vDown = true;
            else
                vUp = true;
        }
        hKey = (eKeyState)mHKey;
        vKey = (eKeyState)mVKey;
    }
}
