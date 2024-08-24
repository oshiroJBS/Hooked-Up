using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_coor : MonoBehaviour
{
    public Transform player;
    Vector3 targetPos;

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);
        this.transform.position = targetPos;
    }
}
