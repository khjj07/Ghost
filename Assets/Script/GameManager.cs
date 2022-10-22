using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class GameManager : Singleton<GameManager>
{

    public Subject<bool> StepSubject;

    // Start is called before the first frame update
    void Start()
    {
        StepSubject = new Subject<bool>();
    }

    public void Step()
    {
        StepSubject.OnNext(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
