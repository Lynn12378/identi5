using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private GameObject playerObject;
    private Transform player;    
    // private GameObject camera;
    // private Camera playerCamera;

    void Update()
    {
        playerObject = GameObject.Find("localPlayer");
        
        if(playerObject==null){
            return;
        }
        player = playerObject.GetComponent<Transform>(); 

        transform.position = player.position + Vector3.back * 90;
    }
}
