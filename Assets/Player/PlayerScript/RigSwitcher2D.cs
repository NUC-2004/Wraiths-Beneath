using UnityEngine;

public class RigSwitcher2D : MonoBehaviour
{
    public GameObject rigLeft;
    public GameObject rigRight;
    public GameObject rigUp;
    public GameObject rigDown;
    public GameObject rigDead;

    public enum Facing { Left, Right, Up, Down }
    public Facing startFacing = Facing.Down;

    private Animator animLeft;
    private Animator animRight;
    private Animator animUp;
    private Animator animDown;
    private Facing current;
    private bool isMoving;

    void Awake()
    {
        animLeft = GetAnimator(rigLeft);
        animRight = GetAnimator(rigRight);
        animUp = GetAnimator(rigUp);
        animDown = GetAnimator(rigDown);

        if (rigDead != null)
        {
            rigDead.SetActive(false);
        }

        SetFacing(startFacing, true);
        SetMoving(false);
    }

    public void SetFacing(Facing facing, bool force = false)
    {
        if (!force && facing == current)
        {
            return;
        }

        current = facing;
        SetRigActive(rigLeft, facing == Facing.Left);
        SetRigActive(rigRight, facing == Facing.Right);
        SetRigActive(rigUp, facing == Facing.Up);
        SetRigActive(rigDown, facing == Facing.Down);
        ApplyMovingState();
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
        ApplyMovingState();
    }

    public void SetDead()
    {
        isMoving = false;
        SetRigActive(rigLeft, false);
        SetRigActive(rigRight, false);
        SetRigActive(rigUp, false);
        SetRigActive(rigDown, false);

        if (rigDead != null)
        {
            rigDead.SetActive(true);
        }
    }

    private void ApplyMovingState()
    {
        Animator animator = GetCurrentAnimator();
        if (animator != null)
        {
            animator.enabled = isMoving;
        }
    }

    private Animator GetCurrentAnimator()
    {
        switch (current)
        {
            case Facing.Left:
                return animLeft;
            case Facing.Right:
                return animRight;
            case Facing.Up:
                return animUp;
            case Facing.Down:
                return animDown;
            default:
                return null;
        }
    }

    private static Animator GetAnimator(GameObject rig)
    {
        return rig != null ? rig.GetComponent<Animator>() : null;
    }

    private static void SetRigActive(GameObject rig, bool active)
    {
        if (rig != null)
        {
            rig.SetActive(active);
        }
    }
}
