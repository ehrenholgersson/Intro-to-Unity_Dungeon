using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class GlobalState 
{

    public static bool LookEnable;
    public static GameStates GameState = GameStates.MovementEnabled;
    public static void UpdateState(GameStates state)
    {
        GameState = state;
        ChangeGameState();
        Debug.Log("set state to " + state);
    }
    public static void ChangeGameState(GameStates state)
    {
        GameState = state;
        ChangeGameState();
    }
    public static void ChangeGameState()
    {
        switch(GameState)
        {
            case GameStates.MovementEnabled:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameStates.UIOnScreen:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
            case GameStates.MovementLocked:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }
}

public enum GameStates {UIOnScreen, MovementEnabled, MovementLocked }
