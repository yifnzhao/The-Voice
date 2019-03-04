using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

    public Animator animator;

    public enum Anim
    {
        Idle = 0,
        Walk,
        No,
        Yes,

        Total,
    }

    Anim anim = Anim.Idle;

    bool enter = false;

    // Use this for initialization
    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Need attach a animator");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(anim);
        // update states
        switch (anim)
        {
            case Anim.Idle:
                {
                    if (!enter)
                    {
                        Enter_Idle();
                    }
                    Update_Idle();
                }
                break;
            case Anim.Walk:
                {
                    if (!enter)
                    {
                        Enter_Walk();
                    }
                    Update_Walk();
                }
                break;
            case Anim.No:
                {
                    if (!enter)
                    {
                        Enter_No();
                    }
                    Update_No();
                }
                break;
            case Anim.Yes:
                {
                    if (!enter)
                    {
                        Enter_Yes();
                    }
                    Update_Yes();
                }
                break;
            default:
                Debug.LogError("Unknown state:" + anim);
                break;
        }

        // test
        //if (Input.GetKeyUp(KeyCode.W))
        //    ChangeState(Anim.Idle);
        //if (Input.GetKeyUp(KeyCode.E))
        //    ChangeState(Anim.Walk);
    }

    void Enter_Yes()
    {
        animator.SetTrigger("yes");
        enter = true;
    }
    void Update_Yes()
    { }

    void Enter_No()
    {
        Debug.Log("Enter_No");
        animator.SetTrigger("no");
        animator.SetTrigger("walkback");
        enter = true;
    }
    void Update_No()
    { }

    void Enter_Idle()
    {
        Debug.Log("Enter_Idle");
        animator.SetBool("walk", false);
        enter = true;
    }

    void Update_Idle()
    { }

    void Enter_Walk()
    {
        Debug.Log("Enter_Walk");
        animator.SetBool("walk", true);
        enter = true;
    }

    void Update_Walk()
    { }

    

    public void ChangeState(Anim _anim)
    {
        anim = _anim;
        enter = false;
    }

}
