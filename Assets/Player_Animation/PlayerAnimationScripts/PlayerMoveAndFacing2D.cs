using UnityEngine;

[RequireComponent(typeof(RigSwitcher2D))]
public class PlayerMoveAndFacing2D : MonoBehaviour
{
    public float speed = 3f;

    RigSwitcher2D switcher;

    void Awake()
    {
        switcher = GetComponent<RigSwitcher2D>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(x, y);

        bool moving = input.sqrMagnitude > 0.01f;

        switcher.SetMoving(moving);

        if (moving)
        {
            input = input.normalized;
            transform.position += (Vector3)(input * speed * Time.deltaTime);

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                switcher.SetFacing(x > 0 ?
                    RigSwitcher2D.Facing.Right :
                    RigSwitcher2D.Facing.Left);
            }
            else
            {
                switcher.SetFacing(y > 0 ?
                    RigSwitcher2D.Facing.Up :
                    RigSwitcher2D.Facing.Down);
            }
        }
    }
}