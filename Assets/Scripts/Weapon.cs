using UnityEngine;

public class Weapon : Item
{
    public int criticalStrike;
    protected GameObject[] players;

    public void SetProperties(string[] properties, Sprite[] images, GameObject[] models, string[] modifications)
    {
        //Set the base properties
        SetProperties(properties, images, modifications);

        //Set the other properties
        model = models[int.Parse(properties[10])];
        criticalStrike = int.Parse(properties[11]);
    }

    public virtual void Fire1(Player player, int commandExecutionTime)
    {

    }

    public virtual void Fire2(Player player, int commandExecutionTime)
    {

    }


    public void MovePlayers(Player player, int commandExecutionTime)
    {
        players = GameObject.FindGameObjectsWithTag("Player");


        foreach (GameObject playerGameObject in players)
        {
            if (!playerGameObject.Equals(player.transform.gameObject))
            {
                PlayerManager playerManager = playerGameObject.GetComponent<PlayerManager>();
                Player currentPlayer = playerManager.player;

                for (int i = playerManager.history.Length - 1; i > -1; i--)
                {
                    int time2;
                    if (i == playerManager.history.Length - 1)
                    {
                        time2 = playerManager.history[0].serverTime;
                    }
                    else
                    {
                        time2 = playerManager.history[i + 1].serverTime;
                    }

                    if (playerManager.history[i].serverTime != 0 && playerManager.history[i].serverTime < commandExecutionTime && time2 > commandExecutionTime)
                    {
                        currentPlayer.transform.position = playerManager.history[i].position;
                        currentPlayer.transform.rotation = playerManager.history[i].rotation;
                        currentPlayer.animation.forward_Backward = playerManager.history[i].animation.forward_Backward;
                        currentPlayer.animation.left_Right = playerManager.history[i].animation.left_Right;
                        currentPlayer.animation.isGrounded = playerManager.history[i].animation.isGrounded;
                        currentPlayer.animation.jump = playerManager.history[i].animation.jump;
                        break;
                    }
                }
            }
        }
    }

    public void MoveBackPlayers(Player player)
    {
        foreach (GameObject playerGameObject in players)
        {
            if (!playerGameObject.Equals(player.transform.gameObject))
            {
                PlayerManager playerManager = playerGameObject.GetComponent<PlayerManager>();
                Player currentPlayer = playerManager.player;

                int index;
                if (playerManager.index == 0)
                {
                    index = 255;
                }
                else
                {
                    index = playerManager.index - 1;
                }
                currentPlayer.transform.position = playerManager.history[index].position;
                currentPlayer.transform.rotation = playerManager.history[index].rotation;
                currentPlayer.animation.forward_Backward = playerManager.history[index].animation.forward_Backward;
                currentPlayer.animation.left_Right = playerManager.history[index].animation.left_Right;
                currentPlayer.animation.isGrounded = playerManager.history[index].animation.isGrounded;
                currentPlayer.animation.jump = playerManager.history[index].animation.jump;
            }
        }
    }
}
