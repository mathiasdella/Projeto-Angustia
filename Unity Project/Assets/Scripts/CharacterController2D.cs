using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    private Transform transf;

    void Start()
    {
        transf = gameObject.transform;
    }

    public void Move(Vector2 velocity)
    {
        if (velocity.x * transf.localScale.x < 0)
        {
            transf.localScale = new Vector3(-transf.localScale.x, transf.localScale.y, transf.localScale.z);
        }
        transf.position = transf.position + new Vector3(velocity.x, velocity.y, 0);
    }
}