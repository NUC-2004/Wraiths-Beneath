using UnityEngine;

public class RigSwitcher2D : MonoBehaviour
{
    public GameObject rigLeft;
    public GameObject rigRight;
    public GameObject rigUp;
    public GameObject rigDown;
    public GameObject rigDead;

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
    public void SetDead()
    {
        // 关闭所有移动方向的 Rig
        if (rigLeft) rigLeft.SetActive(false);
        if (rigRight) rigRight.SetActive(false);
        if (rigUp) rigUp.SetActive(false);
        if (rigDown) rigDown.SetActive(false);

        // 激活死亡 Rig
        if (rigDead) rigDead.SetActive(true);
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