using UnityEngine;

public struct PlayerAnimation
{
    private Animator animator;
    private bool animatorInitialized;

    //Parameters
    private int left_RightParameter;
    private int forward_BackwardParameter;
    private int isGroundedParameter;
    private int jumpParameter;

    //Animation Values
    private float forwardBackwardValue;
    private float leftRightValue;
    private bool groundedValue;
    private float jumpValue;

    //Forward_Backward
    public float forward_Backward
    {
        get
        {
            if(animatorInitialized)
            {
                return animator.GetFloat(forward_BackwardParameter);
            }
            else
            {
                return forwardBackwardValue;
            }
            
        }

        set
        {
            if(animatorInitialized)
            {
                animator.SetFloat(forward_BackwardParameter, value);
            }
            else
            {
                forwardBackwardValue = value;
            }
            
        }
    }

    //Left_Right
    public float left_Right
    {
        get
        {
            if (animatorInitialized)
            {
                return animator.GetFloat(left_RightParameter);
            }
            else
            {
                return leftRightValue;
            }

        }

        set
        {
            if (animatorInitialized)
            {
                animator.SetFloat(left_RightParameter, value);
            }
            else
            {
                leftRightValue = value;
            }

        }
    }


    //isGrounded
    public bool isGrounded
    {
        get
        {
            if (animatorInitialized)
            {
                return animator.GetBool(isGroundedParameter);
            }
            else
            {
                return groundedValue;
            }

        }

        set
        {
            if (animatorInitialized)
            {
                animator.SetBool(isGroundedParameter, value);
            }
            else
            {
                groundedValue = value;
            }

        }
    }


    //jump
    public float jump
    {
        get
        {
            if (animatorInitialized)
            {
                return animator.GetFloat(jumpParameter);
            }
            else
            {
                return jumpValue;
            }

        }

        set
        {
            if (animatorInitialized)
            {
                animator.SetFloat(jumpParameter, value);
            }
            else
            {
                jumpValue = value;
            }

        }
    }


    public PlayerAnimation(Animator anim)
    {
        animator = anim;
        animatorInitialized = true;
        left_RightParameter = Animator.StringToHash("Left_Right");
        forward_BackwardParameter = Animator.StringToHash("Forward_Backward");
        isGroundedParameter = Animator.StringToHash("IsGrounded");
        jumpParameter = Animator.StringToHash("Jump");


        forwardBackwardValue = 0;
        leftRightValue = 0;
        groundedValue = false;
        jumpValue = 0;
}

    public static float ByteToFloat(byte byteValue)
    {
        float floatValue = (float)byteValue / 100;

        if (floatValue > 1)
        {
            floatValue = -(floatValue - 1);
        }

        return floatValue;
    }

    public static byte FloatToByte(float floatVal)
    {
        floatVal = (float)System.Math.Round(floatVal, 2) * 100;

        if (floatVal < 0)
        {
            floatVal = Mathf.Abs(floatVal - 100);
        }


        return (byte)floatVal;
    }

    

    public void Update(Movement movement)
    {
        //left right
        left_Right = movement.xDirection;

        //forward backward
        forward_Backward = movement.zDirection;

        //isGrounded
        isGrounded = movement.controller.isGrounded;

        //jump
        float jumpValue = ((movement.velocity.y + movement.gravity.y) / Time.fixedDeltaTime) / movement.jumpHeight;
        if (movement.controller.isGrounded) jumpValue = -1;
        jump = jumpValue;

    }

   
}

