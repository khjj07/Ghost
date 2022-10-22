using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GameStateManager<State> : Singleton<GameStateManager<State>> where State : GameState<State>
{
    public State currentState;

    void Start()
    {
        State[] allChildren = GetComponentsInChildren<State>();
        foreach (State child in allChildren)
        {
            child.gameObject.SetActive(false);
        }
        currentState.gameObject.SetActive(true);
    }

    public void Change(State newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnStateEnter();
        }
    }
    public void Next()
    {
        if (currentState == null)
        {
            return;
        }

        if (currentState.nextState == null)
        {
            return;
        }

        Change((State)currentState.nextState);
    }
    public void Previous()
    {
        if (currentState == null)
        {
            return;
        }

        if (currentState.previousState == null)
        {
            return;
        }

        Change((State)currentState.previousState);
    }
}
