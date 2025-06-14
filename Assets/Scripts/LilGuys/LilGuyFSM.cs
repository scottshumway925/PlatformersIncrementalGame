using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class LilGuyFSM
{
    public enum STATE { RUNNING, CLIMBING, SWIMMING, JUMPING}
    public enum EVENT { ENTER, UPDATE, EXIT }

    public STATE name;
    protected EVENT stage;

    protected GameObject lilGuy;
    protected Rigidbody2D rb;
    protected UnityEngine.Transform groundLevel;

    protected LilGuyFSM nextState;

    public LayerMask ground = LayerMask.GetMask("Ground");
    public LayerMask climableWall = LayerMask.GetMask("Climable");
    public LayerMask jumpZone = LayerMask.GetMask("JumpZone");

    public float speed = 1f;
    public float climbSpeed = 1f;
    public float jumpForce = 5f;
    protected float rayDist = .3f;

    public LilGuyFSM(GameObject _lilGuy, Rigidbody2D _rb, UnityEngine.Transform _groundLevel)
    {
        lilGuy = _lilGuy;
        rb = _rb;
        groundLevel = _groundLevel;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public LilGuyFSM Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }

        return this;
    }

    public bool canClimb()
    {
        RaycastHit2D forwardRay = Physics2D.Raycast(groundLevel.position, Vector2.right, rayDist, climableWall);
        Debug.DrawRay(groundLevel.position, Vector2.right * rayDist, forwardRay.collider ? Color.red : Color.green);
        return forwardRay.collider != null;
    }

    public bool canJump()
    {
        RaycastHit2D downRay = Physics2D.Raycast(groundLevel.position, Vector2.down, rayDist, jumpZone);
        Debug.DrawRay(groundLevel.position, Vector2.down * rayDist, downRay.collider ? Color.red : Color.green);
        return downRay.collider != null;
    }

    void OnTriggerEnterCollider2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            nextState = new Swim(lilGuy, rb, groundLevel);
            stage = EVENT.EXIT;
        }
    }

}


public class Running : LilGuyFSM
{
    public Running(GameObject _lilGuy, Rigidbody2D _rb, UnityEngine.Transform _groundLevel)
        : base(_lilGuy, _rb, _groundLevel)
    {
        name = STATE.RUNNING;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (canClimb())
        {
            nextState = new Climb(lilGuy, rb, groundLevel);
            stage = EVENT.EXIT;
            return;
        }
        if (canJump())
        {
            nextState = new Jump(lilGuy, rb, groundLevel);
            stage = EVENT.EXIT;
            return;
        }
        
        rb.linearVelocity = new Vector2(1, rb.linearVelocity.y) * speed;
    }

    public override void Exit()
    {
        base.Exit();
    }
}


public class Climb : LilGuyFSM
{
    public Climb(GameObject _lilGuy, Rigidbody2D _rb, UnityEngine.Transform _groundLevel)
        : base(_lilGuy, _rb, _groundLevel)
    {
        name = STATE.CLIMBING;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        rb.linearVelocity = new Vector2(0, climbSpeed);

        if (!canClimb())
        {
            nextState = new Running(lilGuy, rb, groundLevel);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Jump : LilGuyFSM
{
    public Jump(GameObject _lilGuy, Rigidbody2D _rb, UnityEngine.Transform _groundLevel)
        : base(_lilGuy, _rb, _groundLevel)
    {
        name = STATE.JUMPING;
    }

    private bool isJumping = false;

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (!isJumping)
        {
            rb.linearVelocity = new Vector2(speed, jumpForce);
            isJumping = true;
        }
        rb.linearVelocity = new Vector2(1, rb.linearVelocity.y) * speed;

        if (!canJump())
        {
            nextState = new Running(lilGuy, rb, groundLevel);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        isJumping = false;
        base.Exit();
    }

}


public class Swim : LilGuyFSM
{
    public Swim(GameObject _lilGuy, Rigidbody2D _rb, UnityEngine.Transform _groundLevel) 
        : base(_lilGuy, _rb, _groundLevel)
    {
        name = STATE.SWIMMING;
    }

    private float swimTimer;
    private float timerReset = .5f;



    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (swimTimer !> timerReset)
        {
            swimTimer += Time.deltaTime;
        } else
        {
            rb.linearVelocity = new Vector2(speed, jumpForce);
            swimTimer = 0;
        }

        if (canClimb())
        {
            nextState = new Climb(lilGuy, rb, groundLevel);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}