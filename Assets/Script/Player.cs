using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using DG.Tweening;
using UniRx;
using System.Threading.Tasks;
using UniRx.Triggers;
using System;
using System.Threading;
using System.Data;
using Unity.Burst.CompilerServices;
using static UnityEngine.Rendering.DebugUI;

public class Player : Singleton<Player>
{
    public int ShortInputTime = 100;
    public float MoveDuration = 0.1f;
    public float ShootDuration = 0.3f;
    private bool Actable = true;
    private bool Rotatable = true;

    public Vector2 Direction;

    public void Start()
    {
        CreateKeyStream(KeyCode.W, Vector2.up);
        CreateKeyStream(KeyCode.S, Vector2.down);
        CreateKeyStream(KeyCode.A, Vector2.left);
        CreateKeyStream(KeyCode.D, Vector2.right);
        this.UpdateAsObservable()
            .Subscribe(_ => Sight());
        this.UpdateAsObservable()
           .Where(_ => Actable && Input.GetKeyDown(KeyCode.Space))
           .Subscribe(_ => Shoot());
    }
    public async Task Pass(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction / 2, direction);
        if (hit.collider && hit.collider.CompareTag("Mirror"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Mirror>().Reflect(direction);
        }
        else if (hit.collider && hit.collider.CompareTag("Ghost"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Ghost>().Reveal(direction);
        }
        else
        {
            Debug.DrawLine(transform.position, direction * 10000f, Color.red);
        }
    }

    public async void Sight()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+ Direction / 2, Direction);
        if (hit.collider && hit.collider.CompareTag("Mirror"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Mirror>().Reflect(Direction);
        }
        else if (hit.collider && hit.collider.CompareTag("Ghost"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Ghost>().Pass(Direction);
        }
        else
        {
            Debug.DrawLine(transform.position, Direction * 10000f, Color.red);

        }
    }
    public bool CheckFront()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+ Direction/2, Direction,Define.TileOffset/2);
        if (hit.collider && (hit.collider.CompareTag("Mirror") || hit.collider.CompareTag("Wall")))
            return false;
        else
            return true;
    }

    public void CreateKeyStream(KeyCode key, Vector2 direction)
    {
        var keyStream = this.UpdateAsObservable()
            .Where(_=>Input.GetKey(key));

        keyStream.Delay(TimeSpan.FromMilliseconds(ShortInputTime))
            .Repeat()
            .Where(_ => Actable && Input.GetKey(key))
            .Subscribe(_ => Move(direction));


        this.UpdateAsObservable()
           .Where(_ => Actable && Input.GetKeyDown(key))
           .Subscribe(_ => Rotate(direction));

    }

    public async Task Dead()
    {
        Debug.Log("Player dead");
    }

    public async void Rotate(Vector2 direction)
    {
        Rotatable = false;
        Direction = direction;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, (Vector3)Direction);
        await Task.Run(async () =>
        {
            await Task.Delay(ShortInputTime);
            Rotatable = true;
        });
    }

    public async void Move(Vector2 direction)
    {
        Rotate(direction);
        if(CheckFront())
        {
            Rotatable = false;
            Actable = false;
            transform.DOMove(transform.position + Define.TileOffset * (Vector3)direction, MoveDuration);
            GameManager.instance.Step();
            await Task.Delay((int)MoveDuration * 1000 + ShortInputTime);
            Actable = true;
            Rotatable = true;
        }
        
    }

    public async void Shoot()
    {
        Actable = false;
        Rotatable = false;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+ Direction/2, Direction);
        if (hit.collider && hit.collider.CompareTag("Mirror"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Mirror>().ReflectShoot(Direction);
        }
        else if (hit.collider && hit.collider.CompareTag("Ghost"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, Direction * 10000f, Color.red);
        }

        GameManager.instance.Step();
        await Task.Run(async () =>
        {
            await Task.Delay(ShortInputTime + (int)ShootDuration * 1000);
            Actable = true;
            Rotatable = true;
        });
    }
}
