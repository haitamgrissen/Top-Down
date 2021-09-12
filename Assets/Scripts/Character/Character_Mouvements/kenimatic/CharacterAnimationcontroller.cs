using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationcontroller : MonoBehaviour
{
    public Animator CharAnimator;
    public CustomCharacterController CCC;
    bool isrunning;
    bool iswalking;
    bool isstanding;
    // Start is called before the first frame update
    void Start()
    {
        isstanding = true;
        iswalking = false;
        isrunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (CCC.) { }
    }

    public void walk() {
        CharAnimator.SetBool("iswalking", true);

        CharAnimator.SetBool("isstanding", false);

        CharAnimator.SetBool("isrunning", false);
    } 
    public void run() {
        CharAnimator.SetBool("iswalking", false);

        CharAnimator.SetBool("isstanding", false);

        CharAnimator.SetBool("isrunning", true);
    }
    public void stand() {
        CharAnimator.SetBool("iswalking", false);

        CharAnimator.SetBool("isstanding", true);

        CharAnimator.SetBool("isrunning", false);
    }
}
