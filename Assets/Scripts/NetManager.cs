using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetManager : NetworkManager  {


    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        ClientScene.AddPlayer(conn, 0, new IntegerMessage(0));
    }


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader raceID)
    {
        GameObject player = (GameObject)Instantiate(spawnPrefabs[raceID.ReadMessage<IntegerMessage>().value], Vector3.zero, Quaternion.identity);

        

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }


    
}
