using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class networkingStart : NetworkManager
{
    public Vector3[] spawnablePosition;
    public bool[] stateSpawn;
    private Vector3 SpawnPos = Vector3.zero;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if(this.SpawnMethode())
        {
            Debug.Log(this.spawnablePosition);
            Debug.Log(this.SpawnPos);
            var player = (GameObject)GameObject.Instantiate(playerPrefab, this.SpawnPos, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        // else do nothing
    }

    private bool SpawnMethode()
    {
        for (int i = 0; i < this.spawnablePosition.Length; i++)
        {
            if (!this.stateSpawn[i])
            {
                SpawnPos = this.spawnablePosition[i];
                this.stateSpawn[i] = true;
                return true;
            }
        }
        return false;
    }
}
