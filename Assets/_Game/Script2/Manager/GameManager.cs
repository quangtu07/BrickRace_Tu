using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Pause, }
public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;

    private void Start()
    {
        ChangeState(GameState.MainMenu);
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetState();
        LevelManager.Instance.player.joystick.gameObject.SetActive(false);
    }

    public void ChangeState(GameState state)
    {
        gameState = state;
    }

    public bool IsState(GameState state)
    {
        return gameState == state;
    }

}
