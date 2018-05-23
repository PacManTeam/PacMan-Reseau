using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class networkingStart : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log(conn);
        Debug.Log(playerControllerId);
        base.OnServerAddPlayer(conn, playerControllerId);
    }
}
