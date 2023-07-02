using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationChange : MonoBehaviour
{
    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                mAnimator.SetTrigger("TrShield");
                Debug.Log("shieldTime");
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                mAnimator.SetTrigger("TrNorm");
                Debug.Log("NormTime");
            }
        }
    }
}
