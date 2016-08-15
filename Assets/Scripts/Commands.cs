using System.Collections.Generic;
using UnityEngine;

public enum MoveCommand : byte { None = 0, Forward = 1, Backward = 2, Left = 4, Right = 8, Jump = 16 };

public class Commands
{
    public List<byte> commandsList = new List<byte>();
    public CommandHistory[] commandHistory = new CommandHistory[256];
    public byte commandsStartingIndex = 0;
    private int historyIndex = -1;

    public int currentCommandsIndex
    {
        get
        {
            return historyIndex;
        }
    }

    public void AddCommandsToList(byte moveCommand, byte mouseXCommand, byte mouseYCommand)
    {
        commandsList.Add(moveCommand);
        commandsList.Add(mouseXCommand);
        commandsList.Add(mouseYCommand);

        historyIndex++;
        historyIndex = (historyIndex % 256);

        commandHistory[historyIndex].moveCommand = moveCommand;
        commandHistory[historyIndex].mouseXCommand = mouseXCommand;
        commandHistory[historyIndex].mouseYCommand = mouseYCommand;
    }


    public void ResetCommands()
    {
        commandsList.Clear();

        commandsStartingIndex = (byte)(historyIndex + 1);
        commandsStartingIndex = (byte)(commandsStartingIndex % 256);
    }

    public static byte GetMouseCommand(float val)
    {
        val = Mathf.Clamp(val, -12f, 12f);
        if (val < 0) val = Mathf.Abs(val) + 12f;
        return (byte)Mathf.Round(val * 10.625f);
    }

    public static byte GetMoveCommand(ref bool isJump)
    {
        byte moveCommands = new byte();


        //Here we are storing each move command made by the player
        if (Input.GetKey(KeyCode.UpArrow)) moveCommands += (byte)MoveCommand.Forward;
        if (Input.GetKey(KeyCode.DownArrow)) moveCommands += (byte)MoveCommand.Backward;
        if (Input.GetKey(KeyCode.LeftArrow)) moveCommands += (byte)MoveCommand.Left;
        if (Input.GetKey(KeyCode.RightArrow)) moveCommands += (byte)MoveCommand.Right;
        if (isJump)
        {
            moveCommands += (byte)MoveCommand.Jump;
            isJump = false;
        }

        return moveCommands;
    }
}

