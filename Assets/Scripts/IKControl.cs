using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    protected Animator animator;

    [HideInInspector]
    public bool ikActive = true;
    [HideInInspector]
    public bool handIK = true;
    public Transform _rightFootTarget = null;
    public Transform _leftFootTarget = null;

    public Transform _rightHandTarget = null;
    public Transform _leftHandTarget = null;

    public Transform _headTarget = null;
    public float _footOffset = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {


                // Set the right hand target position and rotation, if one has been assigned
                if (_rightFootTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    //animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootTarget.position + new Vector3(0, _footOffset, 0));
                    //animator.SetIKRotation(AvatarIKGoal.RightFoot, _rightFootTarget.rotation);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (_leftFootTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    //animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootTarget.position + new Vector3(0, _footOffset, 0));
                    //animator.SetIKRotation(AvatarIKGoal.LeftFoot, _leftFootTarget.rotation);
                }

                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(_headTarget.position);

                if (handIK)
                {


                    // Set the right hand target position and rotation, if one has been assigned
                    if (_rightHandTarget != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandTarget.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandTarget.rotation);
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (_leftHandTarget != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandTarget.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandTarget.rotation);
                    }

                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

                animator.SetLookAtWeight(0);
            }
        }
    }
}
