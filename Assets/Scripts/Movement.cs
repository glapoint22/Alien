using UnityEngine;

public class Movement
{
    public CharacterController controller;
    public float moveSpeed = 5.662f;
    public float airResistance = .03f;
    public float rotationSpeed = 10;
    public float jumpHeight = 4;
    public float xDirection;
    public float zDirection;
    public Vector3 gravity = Vector3.zero;
    public Vector3 velocity = Vector3.zero;


    private Transform transform;
    private float xAxisRotation = 0;
    private float yAxisRotation = 0;
    private float acceleration = .06f;
    private Transform playerEyes;
    

    
    public Movement(Transform trans, Transform eyes)
    {
        transform = trans;
        controller = transform.GetComponent<CharacterController>();
        playerEyes = eyes;
    }


    public void Move(byte commandInput)
    {
        //Get the direction the player will be moving
        Vector3 moveDirection = transform.TransformDirection(GetMoveDirection(commandInput));

        //If player is on the ground
        if (controller.isGrounded)
        {
            velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;

            //Reset gravity
            gravity = Vector3.down * .05f;
        }
        else
        {
            gravity += Physics.gravity * Mathf.Pow(Time.fixedDeltaTime, 2);
            velocity += Vector3.ClampMagnitude(moveDirection, 1) * moveSpeed * airResistance * Time.fixedDeltaTime;
            Vector3 tempVelocity = Vector3.ClampMagnitude(new Vector3(velocity.x, 0, velocity.z), moveSpeed * Time.fixedDeltaTime);
            velocity = new Vector3(tempVelocity.x, velocity.y, tempVelocity.z);
        }

        //If player is jumping
        if ((commandInput & (byte)MoveCommand.Jump) >> 4 == 1)
        {
            gravity = Vector3.zero;
            velocity += Vector3.up * jumpHeight * Time.fixedDeltaTime;
        }
        controller.Move(velocity + gravity);
    }

    public void RotateXAxis(byte mouseY)
    {
        xAxisRotation -= ByteToFloat(mouseY) * rotationSpeed;
        xAxisRotation = Mathf.Clamp(xAxisRotation, -90f, 90f);
        playerEyes.localEulerAngles = new Vector3(xAxisRotation, 0, 0);
    }

    public void RotateYAxis(byte mouseX)
    {
        yAxisRotation += ByteToFloat(mouseX) * rotationSpeed;
        transform.eulerAngles = new Vector3(0.0f, yAxisRotation, 0.0f);
    }

    float ByteToFloat(byte mouse)
    {
        float newVal = mouse * 0.09411764705882352941176470588235f;
        if (newVal > 12f) newVal = -(newVal - 12f);

        return newVal;
    }

    Vector3 GetMoveDirection(byte commandInput)
    {
        //Figure out which move commands are being used
        float left = -((commandInput & (byte)MoveCommand.Left) >> 2);
        float right = (commandInput & (byte)MoveCommand.Right) >> 3;
        float forward = commandInput & (byte)MoveCommand.Forward;
        float backward = -((commandInput & (byte)MoveCommand.Backward) >> 1);


        float horizontal = left + right;
        xDirection = Mathf.MoveTowards(xDirection, horizontal, acceleration);

        float vertical = forward + backward;
        zDirection = Mathf.MoveTowards(zDirection, vertical, acceleration);



        return Vector3.ClampMagnitude(new Vector3(xDirection, 0, zDirection), 1);

    }
}
