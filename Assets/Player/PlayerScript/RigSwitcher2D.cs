using UnityEngine;

public class RigSwitcher2D : MonoBehaviour
{
    public GameObject rigLeft;
    public GameObject rigRight;
    public GameObject rigUp;
    public GameObject rigDown;

    Animator animLeft;
    Animator animRight;
    Animator animUp;
    Animator animDown;

    public enum Facing { Left, Right, Up, Down }
    public Facing startFacing = Facing.Down;

    Facing current;

    void Awake()
    {
        animLeft = rigLeft ? rigLeft.GetComponent<Animator>() : null;
        animRight = rigRight ? rigRight.GetComponent<Animator>() : null;
        animUp = rigUp ? rigUp.GetComponent<Animator>() : null;
        animDown = rigDown ? rigDown.GetComponent<Animator>() : null;

        SetFacing(startFacing, true);
        SetMoving(false);
    }

    public void SetFacing(Facing facing, bool force = false)
    {
        if (!force && facing == current) return;
        current = facing;

        if (rigLeft) rigLeft.SetActive(facing == Facing.Left);
        if (rigRight) rigRight.SetActive(facing == Facing.Right);
        if (rigUp) rigUp.SetActive(facing == Facing.Up);
        if (rigDown) rigDown.SetActive(facing == Facing.Down);
    }

    public void SetMoving(bool moving)
    {
        Animator a = GetCurrentAnimator();
        if (a != null) a.enabled = moving;
    }

    Animator GetCurrentAnimator()
    {
        switch (current)
        {
            case Facing.Left: return animLeft;
            case Facing.Right: return animRight;
            case Facing.Up: return animUp;
            case Facing.Down: return animDown;
        }
        return null;
    }
}