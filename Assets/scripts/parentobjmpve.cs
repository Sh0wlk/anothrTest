using UnityEngine;

public class parentobjmpve : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        Vector2 scale=player.transform.localScale;
        scale.y = 1;
        if (scale.x > 0)
        {
            scale.x = 1;
        }
        else
        {
            scale.x = -1;
        }
            transform.localScale = scale;
        transform.position = player.transform.position;
    }
}
