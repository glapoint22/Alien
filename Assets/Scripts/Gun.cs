using UnityEngine;

public class Gun: Weapon
{
    public override void Fire1(Player player, int commandExecutionTime)
    {
        
        MovePlayers(player, commandExecutionTime);

        //Ray
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        player.SetCamera();
        Ray ray = player.camera.ScreenPointToRay(screenCenter);
        player.camera.enabled = false;
        


        RaycastHit hit;
        HitInfo hitInfo = new HitInfo();
        int layerMask = (1 << 0) | (1 << 9);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            hitInfo = HitInfo.GetHitInfo(hit, player);
        }

        player.transform.GetComponent<PlayerManager>().RpcHit(hitInfo);

        MoveBackPlayers(player);


        

    }

    public override void Fire2(Player player, int commandExecutionTime)
    {

    }
}


