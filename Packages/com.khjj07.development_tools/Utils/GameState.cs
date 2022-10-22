using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class GameState<State> : MonoBehaviour where State: MonoBehaviour
{
    [SerializeField]
    private bool ActiveOnAndOff = true;
    public State previousState;
    public State nextState;
    public UnityEvent onStateEnableEvent;
    public UnityEvent onStateExitEvent;
    /*
     protected void OnEnable()
     {
         ChangeState(this);
     }
     */
    public virtual void OnStateEnter()
    {
        if(ActiveOnAndOff)
            gameObject.SetActive(true);
        onStateEnableEvent.Invoke();
    }

    public virtual void OnStateExit()
    {
        if(ActiveOnAndOff)
            gameObject.SetActive(false);
        onStateExitEvent.Invoke();
    }
}
