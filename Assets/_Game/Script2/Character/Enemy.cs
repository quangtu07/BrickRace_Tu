using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Enemy : Character
{
    //[SerializeField] protected Transform tf;

    public Vector3 destination;
    public Vector3 winPos;

    private IState currentState;
    private Floor currentFloor;

    // Change character color, set speed, set idle state when start
    public override void OnInit()
    {
        ChangeColor(color);
        agent.enabled = true;
        agent.speed = 10f;
        ChangeState(new IdleState());
    }

    // if gamestate is gameplay - character can move
    // else gamestate is not gameplay - character stop
    private void Update()
    {
        if (GameManager.Instance.IsState(GameState.GamePlay))
        {
            if (currentState != null)
            {
                currentState.OnExecute(this);
            }

            isGround = CheckGround();

            //CheckIsUp
            if (Tf.forward.z > 0f)
            {
                isUp = true; // Di len cau
            }
            else if (Tf.forward.z < 0f)
            {
                isUp = false; // Di xuong cau
            }
        } else
        {
            ChangeState(new PauseState());
        }
    }

    // Check when character came target des
    public bool IsDestionation()
    {
        return Vector3.Distance(Tf.position, destination + (Tf.position.y - destination.y) * Vector3.up) < 0.1f;
    }

    // Set destination for bot AI
    public void SetDestination(Vector3 pos)
    {
        agent.enabled = true;
        destination = pos;
        agent.destination = pos;
        //agent.SetDestination(pos);
    }

    public override void OnStopMove()
    {
        agent.enabled = false;
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    public Vector3 SeekBrickSameColor()
    {
        return currentFloor.SeekBrick(color);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagName.STAIR_TAG))
        {
            if (other.GetComponent<ColorBrick>().color != color)
            {
                if (brickStack.Count > 0) // Con gach
                {
                    other.GetComponent<ColorBrick>().ChangeColor(color);
                    RemoveBrick();
                }
                else // Het gach
                {
                    ChangeState(new PatrolState());
                }
            }
        }
        if (isGround && other.CompareTag(TagName.BRICK_TAG))
        {
            // Cung mau thi an gach
            if (other.GetComponent<ColorBrick>().color == color)
            {
                other.GetComponent<ColorBrick>().ChangeColor(ColorType.None);
                AddBrick();
            }
        }

        if (other.CompareTag("Ground")) {
            currentFloor = other.gameObject.GetComponent<Floor>();
        }

        if (other.CompareTag(TagName.WIN_AREA))
        {
            Win();
            GameManager.Instance.ChangeState(GameState.Finish);
            LevelManager.Instance.OnFinish();
            UIManager.Instance.OpenUI<CanvasFail>();
        }

    }
}
