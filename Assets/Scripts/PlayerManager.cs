using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

[NetworkSettings(channel = 1, sendInterval = 0.05f)]//Unreliable
public class PlayerManager : NetworkBehaviour
{
    public float backTime = 0.1f;
    public Player player;
    public Weapon currentWeapon;
    public float sendInterval;
    private byte expectedIndex = 0;
    private bool isPacketLoss;
    private RemotePlayer remotePlayer;
    public PlayerData[] history = new PlayerData[256];

    private Gun gun = new Gun();





    [SyncVar]
    private float xPosition;

    [SyncVar]
    private float yPosition;

    [SyncVar]
    private float zPosition;

    [SyncVar]
    byte forward_BackwardAnimation;

    [SyncVar]
    byte left_RightAnimation;

    [SyncVar]
    byte isGroundedAnimation;


    [SyncVar]
    byte jumpAnimation;

    [SyncVar]
    private byte rotationAngle;

    [SyncVar(hook = "UpdateRemotePlayer")]
    private byte UpdateCounter = 0;




    public int index = 0;



    private Text text;

    const short resendCommandsID = MsgType.Highest + 1;

    public override void OnStartClient()
    {
        NetworkManager.singleton.client.RegisterHandler(resendCommandsID, ResendCommands);
    }

    void Start()
    {
        
        text = GameObject.Find("Text").GetComponent<Text>();

        sendInterval = GetNetworkSendInterval();
        player = new Player(transform);

        currentWeapon = gun;

        


        //Server and local player
        if (isServer || isLocalPlayer)
        {
            player.movement.controller.enabled = true;
        }


        //Local Player
        if (isLocalPlayer)
        {
            //Local player script
            gameObject.AddComponent<LocalPlayer>();


            //camera
            player.SetCamera();
        }


        //Remote Client
        if (isClient && !isLocalPlayer)
        {
            remotePlayer = gameObject.AddComponent<RemotePlayer>();
            player.movement.controller.enabled = false;
        }
    }

    [Command(channel = 1)]//Unreliable
    public void CmdUpdateServer(byte[] commands)
    {

        //Grab the starting index from this batch of commands
        byte startingIndex = commands[0];

        //If the starting index does not match the expected index, we have packet loss
        if (startingIndex != expectedIndex)
        {
            if (!isPacketLoss)
            {
                isPacketLoss = true;
                //Get the client to resend the missing packets
                connectionToClient.Send(resendCommandsID, new IntegerMessage(expectedIndex));
            }
            return;
        }


        //Use the current batch of commands to move and rotate the player
        MovePlayer(commands);

        //Temp
        text.text = transform.position.ToString();
    }


    void ResendCommands(NetworkMessage netMsg)
    {
        IntegerMessage myInt = netMsg.ReadMessage<IntegerMessage>();
        byte expectedIndex = (byte)myInt.value;

        //Calculate the size of the commands array(How many commands we will be resending)
        int size = player.commands.currentCommandsIndex - expectedIndex;
        if (size < 0) size += 256;
        byte[] commands = new byte[(size * 3) + 4];

        //Set the starting index
        int index = expectedIndex;
        commands[0] = (byte)index;

        //Grab the commands that were archived in history and give them to the commands array
        //These are the commands the sever never got, so we will be resending them
        for (int i = 1; i < commands.Length; i += 3)
        {
            commands[i] = player.commands.commandHistory[index].moveCommand;
            commands[i + 1] = player.commands.commandHistory[index].mouseXCommand;
            commands[i + 2] = player.commands.commandHistory[index].mouseYCommand;
            index++;
            index = index % 256;
        }

        //This method will clear the commands list and set the starting index
        player.commands.ResetCommands();


        //Send the commands to the server
        CmdUpdateMissingPackets(commands);
    }


    void MovePlayer(byte[] commands)
    {
        //Use the given commands to move and rotate the player
        for (int i = 1; i < commands.Length; i += 3)
        {
            player.movement.Move(commands[i]);
            player.movement.RotateYAxis(commands[i + 1]);
            player.movement.RotateXAxis(commands[i + 2]);

            //Update the animations
            player.animation.Update(player.movement);
        }


        history[index].serverTime = NetworkTransport.GetNetworkTimestamp();
        history[index].position = transform.position;
        history[index].rotation = transform.rotation;
        history[index].animation.forward_Backward = player.animation.forward_Backward;
        history[index].animation.left_Right = player.animation.left_Right;
        history[index].animation.isGrounded = player.animation.isGrounded;
        history[index].animation.jump = player.animation.jump;




        index++;
        index = (index % 256);

        //Calculate the index that should match the next batch of commands
        //If this expected index does not match the index provided with the next batch, we have packet loss
        expectedIndex = (byte)(commands[0] + ((commands.Length - 1) / 3) % 256);


        //---Update the sync vars---

        //Position
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        zPosition = transform.position.z;

        //Rotation
        rotationAngle = (byte)(transform.localEulerAngles.y * .71f);

        //Animation parameters
        forward_BackwardAnimation = PlayerAnimation.FloatToByte(player.animation.forward_Backward);
        left_RightAnimation = PlayerAnimation.FloatToByte(player.animation.left_Right);
        isGroundedAnimation = System.Convert.ToByte(player.animation.isGrounded);
        jumpAnimation = PlayerAnimation.FloatToByte(player.animation.jump);


        //Used to invoke UpdateRemotePlayer method when a new update has arrived
        UpdateCounter++;
        UpdateCounter = (byte)(UpdateCounter % 256);
    }

    [Command(channel = 0)]//Reliable sequenced
    void CmdUpdateMissingPackets(byte[] commands)
    {
        //Call this to move and rotate the player
        MovePlayer(commands);
        isPacketLoss = false;
    }


    void UpdateRemotePlayer(byte i)
    {
        //If not the remote client..return
        if (remotePlayer == null) return;

        PlayerData playerData = new PlayerData();

        playerData.position = new Vector3(xPosition, yPosition, zPosition);
        playerData.rotation = Quaternion.AngleAxis(rotationAngle * 1.406f, Vector3.up);
        playerData.animation.forward_Backward = PlayerAnimation.ByteToFloat(forward_BackwardAnimation);
        playerData.animation.left_Right = PlayerAnimation.ByteToFloat(left_RightAnimation);
        playerData.animation.isGrounded = System.Convert.ToBoolean(isGroundedAnimation);
        playerData.animation.jump = PlayerAnimation.ByteToFloat(jumpAnimation);


        //Update the remote client
        remotePlayer.SetPlayerData(playerData);
    }


    [Command(channel = 0)]//Reliable sequenced
    public void CmdFire(int time, byte fireType)
    {
        byte error;
        int delay = NetworkTransport.GetRemoteDelayTimeMS(connectionToClient.hostId, connectionToClient.connectionId, time, out error);
        
        int commandExecutionTime = NetworkTransport.GetNetworkTimestamp() - delay - (int)(backTime * 1000);

        if(fireType == 0)
        {
            currentWeapon.Fire1(player, commandExecutionTime);
        }
        else
        {
            currentWeapon.Fire2(player, commandExecutionTime);
        }
        

    }

    [ClientRpc(channel = 0)]//Reliable sequenced
    public void RpcHit(HitInfo hitInfo)
    {
        if(hitInfo.targetNetId.Value != 0)
        {
            Player player = HitInfo.GetPlayer(hitInfo.targetNetId);
            Debug.Log(HitInfo.GetCollider(player, hitInfo.bodyPart).tag);
        }
        else
        {
            Debug.Log("None");

        }

       
    }
}


