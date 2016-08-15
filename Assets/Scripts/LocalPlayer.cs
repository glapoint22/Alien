using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LocalPlayer : MonoBehaviour {
    private bool isJump;
    private PlayerManager playerManager;
    private Player player;

    

    private Text text;
    
    void Start ()
    {
        playerManager = GetComponent<PlayerManager>();
        player = playerManager.player;
        StartCoroutine(SendCommandsToServer());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;



        text = GameObject.Find("Text").GetComponent<Text>();
    }
	
	
	void Update ()
    {
        //As long as the local player is on the ground and the jump key is pressed, let it be known that the player is jumping
        if (player.movement.controller.isGrounded) if (Input.GetKeyDown(KeyCode.RightControl)) isJump = true;

        //Fire
        if (Input.GetMouseButtonDown(0))
        {
            playerManager.CmdFire(NetworkTransport.GetNetworkTimestamp(), 0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            playerManager.CmdFire(NetworkTransport.GetNetworkTimestamp(), 1);
        }
    }


    void FixedUpdate()
    {
        //Move the player
        byte moveCommand = Commands.GetMoveCommand(ref isJump);
        player.movement.Move(moveCommand);

        
        //Rotate the player on the Y axis
        byte mouseXCommand = Commands.GetMouseCommand(Input.GetAxis("Mouse X"));
        player.movement.RotateYAxis(mouseXCommand);

        //Rotate player on the X axis
        byte mouseYCommand = Commands.GetMouseCommand(Input.GetAxis("Mouse Y"));
        player.movement.RotateXAxis(mouseYCommand);


        //Add the move commands and mouse commands to the list
        //This is a list of commands we will be sending off to the server
        player.commands.AddCommandsToList(moveCommand, mouseXCommand, mouseYCommand);


        text.text = transform.position.ToString();
    }

    
    IEnumerator SendCommandsToServer()
    {
        while (true)
        {
            if (player.commands.commandsList.Count > 0)
            {
                //Create an array for the commands
                //We need to create a built in array because we can't send a list over the network :(
                byte[] commandArray = new byte[player.commands.commandsList.Count + 1];


                //Assign the values to the array
                commandArray[0] = player.commands.commandsStartingIndex;
                for (int i = 0; i < player.commands.commandsList.Count; i++)
                {
                    commandArray[i + 1] = player.commands.commandsList[i];
                }

               
                //Send the commands to the server
                playerManager.CmdUpdateServer(commandArray);

                //Reset the the commands
                player.commands.ResetCommands();
            }
            
            yield return new WaitForSeconds(playerManager.sendInterval);
        }
        
    }
}
