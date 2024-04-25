using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : Character
{
    [SerializeField] public FixedJoystick joystick;
    [SerializeField] private float moveSpeed;
    public int score = 0;

    public override void OnInit()
    {
        ChangeColor(color);
        agent.enabled = true;
    }

    private void Update()
    {
        if (GameManager.Instance.IsState(GameState.GamePlay))
        {
            isGround = CheckGround();

            Vector3 inputJoyStick = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

            if (inputJoyStick.magnitude > 0f)
            {
                ChangeAnim(AnimName.RUN);
                Move(inputJoyStick);
            }
            else
            {
                ChangeAnim(AnimName.IDLE);
            }

            // Di len cau
            if (Tf.forward.z > 0)
            {
                isUp = true; // di len cau

            }
            else if (Tf.forward.z < 0)
            {
                isUp = false; //Di xuong cau
                moveSpeed = 10;
            }
        }

    }

    private void Move(Vector3 direct)
    {
        agent.enabled = true;
        Vector3 targetPos = Camera.main.transform.TransformDirection(direct);
        targetPos.y = 0f;

        agent.Move(moveSpeed * Time.deltaTime * targetPos);

        if (targetPos != Vector3.zero)
        {
            Quaternion targetRos = Quaternion.LookRotation(targetPos);
            Tf.rotation = Quaternion.Slerp(Tf.rotation, targetRos, 10f * Time.deltaTime);
        }
    }

    public override void OnStopMove()
    {
        base.OnStopMove();
        agent.enabled = false;
    }

    public override void Win()
    {
        base.Win();
        joystick.gameObject.SetActive(false);

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
                    // Bi chan
                    moveSpeed = 0;
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
                UIManager.Instance.GetUI<CanvasGamePlay>().UpdateScore(++score);
            }
        }

        if (other.CompareTag(TagName.WIN_AREA))
        {
            Win();
            GameManager.Instance.ChangeState(GameState.Finish);
            LevelManager.Instance.OnFinish();
            UIManager.Instance.OpenUI<CanvasVictory>().SetBestScore(score);
        }

    }
}
