using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using System;

public enum InputKind
{
    KeyDown,
    KeyUp,
    Key
}


[Serializable]
public class KeyEventStruct
{
    public KeyCode Key;
    public InputKind Kind;
    public UnityEvent InputEvent;
    public KeyEventStruct(KeyCode new_key, InputKind new_kind, UnityEvent new_unityEvent)
    {
        Key = new_key;
        Kind = new_kind;
        InputEvent = new_unityEvent;
    }
}

public class KeyInputModule : MonoBehaviour
{
    private Subject<KeyEventStruct> EventRegister; //Subject for filtering InputKind
    [SerializeField]
    private List<KeyEventStruct> InputList = new List<KeyEventStruct>();
    public ReactiveCollection<KeyEventStruct> InputCollection = new ReactiveCollection<KeyEventStruct>();
    // Start is called before the first frame update
    void Start()
    {
        EventRegister = new Subject<KeyEventStruct>();
        EventRegister.Where(x => x.Kind.Equals(InputKind.KeyDown))
            .Subscribe(x => CreateKeyDownStream(x));
        EventRegister.Where(x => x.Kind.Equals(InputKind.Key))
            .Subscribe(x => CreateKeyStream(x));
        EventRegister.Where(x => x.Kind.Equals(InputKind.KeyUp))
                    .Subscribe(x => CreateKeyUpStream(x));
        InputCollection.ObserveAdd().Subscribe(x => EventRegister.OnNext(x.Value));
        foreach (var k in InputList)
            EventRegister.OnNext(k);
    }

    private void CreateKeyDownStream(KeyEventStruct structure)
    {
        this.UpdateAsObservable()
           .Where(_ => Input.GetKeyDown(structure.Key))
           .Subscribe(_ => structure.InputEvent.Invoke())
           .AddTo(gameObject);
    }

    private void CreateKeyStream(KeyEventStruct structure)
    {
        this.UpdateAsObservable()
              .Where(_ => Input.GetKeyUp(structure.Key))
              .Subscribe(_ => structure.InputEvent.Invoke())
              .AddTo(gameObject);
    }

    private void CreateKeyUpStream(KeyEventStruct structure)
    {
        this.UpdateAsObservable()
              .Where(_ => Input.GetKey(structure.Key))
              .Subscribe(_ => structure.InputEvent.Invoke())
              .AddTo(gameObject);
    }
}
