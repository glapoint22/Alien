using UnityEngine;
using UnityEngine.Networking;

public struct HitInfo
{
    public NetworkInstanceId attackerNetId;
    public NetworkInstanceId targetNetId;
    public Vector3 hitPosition;
    public byte bodyPart;

    //Head
    //Shoulders
    //Chest
    //Wrist
    //Hands
    //waist
    //Legs
    //Feet

    public static HitInfo GetHitInfo(RaycastHit hit, Player attacker)
    {
        HitInfo hitInfo = new HitInfo();

        //Find out if a player was hit
        NetworkIdentity networkIdentity = hit.transform.root.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            hitInfo.targetNetId = networkIdentity.netId;
            hitInfo.attackerNetId = attacker.transform.GetComponent<NetworkIdentity>().netId;
            hitInfo.bodyPart = GetBodyPart(hit.collider);
        }
        
        //Hit Position
        hitInfo.hitPosition = hit.transform.InverseTransformPoint(hit.point);

        return hitInfo;
    }

    private static byte GetBodyPart(Collider collider)
    {
        byte part = 0;

        switch (collider.tag)
        {
            case "Head":
                part = 1;
                break;
            case "Neck":
                part = 2;
                break;
            case "Upper Torso":
                part = 3;
                break;
            case "Lower Torso":
                part = 4;
                break;
            case "Waist":
                part = 5;
                break;
            case "Pelvis":
                part = 6;
                break;
            case "Left Upper Leg":
                part = 7;
                break;
            case "Left Lower Leg":
                part = 8;
                break;
            case "Left Foot":
                part = 9;
                break;
            case "Right Upper Leg":
                part = 10;
                break;
            case "Right Lower Leg":
                part = 11;
                break;
            case "Right Foot":
                part = 12;
                break;
            case "Left Upper Arm":
                part = 13;
                break;
            case "Left Lower Arm":
                part = 14;
                break;
            case "Left Wrist":
                part = 15;
                break;
            case "Left Hand":
                part = 16;
                break;
            case "Right Upper Arm":
                part = 17;
                break;
            case "Right Lower Arm":
                part = 18;
                break;
            case "Right Wrist":
                part = 19;
                break;
            case "Right Hand":
                part = 20;
                break;
        }

        return part;
    }

    public static Player GetPlayer(NetworkInstanceId id)
    {
        Player player = null;
        GameObject playerGameObject;

        if (NetworkClient.active)
        {
            playerGameObject = ClientScene.FindLocalObject(id);
        }
        else
        {
            playerGameObject = NetworkServer.FindLocalObject(id);
        }
        


            PlayerManager playerManager = playerGameObject.GetComponent<PlayerManager>();

        if (playerManager != null) player = playerManager.player;

        return player;
    }

    public static Collider GetCollider(Player player, byte bodyPart)
    {
        Collider collider = null;
        string tag = string.Empty;

        switch (bodyPart)
        {
            case 1:
                tag = "Head";
                break;
            case 2:
                tag = "Neck";
                break;
            case 3:
                tag = "Upper Torso";
                break;
            case 4:
                tag = "Lower Torso";
                break;
            case 5:
                tag = "Waist";
                break;
            case 6:
                tag = "Pelvis";
                break;
            case 7:
                tag = "Left Upper Leg";
                break;
            case 8:
                tag = "Left Lower Leg";
                break;
            case 9:
                tag = "Left Foot";
                break;
            case 10:
                tag = "Right Upper Leg";
                break;
            case 11:
                tag = "Right Lower Leg";
                break;
            case 12:
                tag = "Right Foot";
                break;
            case 13:
                tag = "Left Upper Arm";
                break;
            case 14:
                tag = "Left Lower Arm";
                break;
            case 15:
                tag = "Left Wrist";
                break;
            case 16:
                tag = "Left Hand";
                break;
            case 17:
                tag = "Right Upper Arm";
                break;
            case 18:
                tag = "Right Lower Arm";
                break;
            case 19:
                tag = "Right Wrist";
                break;
            case 20:
                tag = "Right Hand";
                break;
        }

        Transform[] children = player.transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.tag == tag)
            {
                collider = child.GetComponent<Collider>();
                break;
            }
        }

        return collider;
    }
}
