using System.Collections;
using UnityEngine;
using Fusion;
public class PlayerCamera : NetworkBehaviour
{
    private GameObject playerObject;
    private Transform player;
    // private GameObject camera;
    // private Camera playerCamera;

    public override void FixedUpdateNetwork()
    {
        playerObject = GameObject.Find("Player(clone)");
        
        if(playerObject==null){
            return;
        }
        player = playerObject.GetComponent<Transform>(); 

        transform.position = player.position + Vector3.back * 90;
    }
}
