using UnityEngine;

public class Smooth_follow : MonoBehaviour
{
    // Start is called before the first frame update
    public BoxCollider2D boundsCollider;
    public Transform player;
    Vector3 targetPos;

    void Start()
    {
        targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);
    }
    // Update is called once per frame

    void FixedUpdate()
    {
        targetPos = new Vector3(player.position.x, player.position.y, this.transform.position.z);

        if (!boundsCollider.bounds.Contains(targetPos))
        {
            targetPos = boundsCollider.bounds.ClosestPoint(targetPos);
            targetPos.z = this.transform.position.z;
        }
        this.transform.position = targetPos;
    }
}
