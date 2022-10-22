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
    public bool Key = false;
    public LineRenderer lineRenderer;
    public Animator animator;
    public Vector2 Direction;

    public void Start()
    {
        animator = GetComponent<Animator>();    
        lineRenderer = GetComponent<LineRenderer>();
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
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction * Define.TileOffset / 2, direction);
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
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+ Direction * Define.TileOffset / 2, Direction);
        if (hit.collider)
        {
            if (hit.collider.CompareTag("Mirror"))
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
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
        else
        {
            Debug.DrawLine(transform.position, Direction * 10000f, Color.red);

        }
    }
    public bool CheckFront()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+ Direction * Define.TileOffset / 2, Direction,Define.TileOffset/2);
        if (hit.collider && (hit.collider.CompareTag("Mirror") || hit.collider.CompareTag("Wall")))
            return false;
        else if (hit.collider && hit.collider.CompareTag("Door"))
        {
            if (Key)
                hit.transform.GetComponent<Door>().Kill();
            return false;
        }
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
    public void Update()
    {
        if (Direction == Vector2.up)
        {
            animator.SetInteger("State", 1);
        }
        else if (Direction == Vector2.down)
        {
            animator.SetInteger("State", 2);
        }
        else if (Direction == Vector2.left)
        {
            animator.SetInteger("State", 3);
        }
        else if (Direction == Vector2.right)
        {
            animator.SetInteger("State", 4);
        }
        animator.SetBool("Walk", !Actable);
    }

    public async Task Dead()
    {
        Debug.Log("Player dead");
    }

    public async void Rotate(Vector2 direction)
    {
        Rotatable = false;
        Direction = direction;
        animator.SetBool("Walk", true);
        await Task.Run(async () =>
        {
            await Task.Delay(ShortInputTime);
            Rotatable = true;
        });
    }

    public async Task Move(Vector2 direction)
    {
        Rotate(direction);
        if(CheckFront())
        {
            Rotatable = false;
            Actable = false;
            transform.DOMove(transform.position + Define.TileOffset * (Vector3)direction, MoveDuration)
                .OnComplete(() => {
                    Actable = true;
                    Rotatable = true;
                });
            GameManager.instance.Step();
        }
        
    }

    public async void Shoot()
    {
        Actable = false;
        Rotatable = false;
        int count = 2;
        lineRenderer.positionCount = count;
        lineRenderer.SetPosition(0, transform.localPosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+ Direction * Define.TileOffset / 2, Direction);
        if (hit.collider && hit.collider.CompareTag("Mirror"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            lineRenderer.positionCount = count;
            lineRenderer.SetPosition(1, hit.transform.position);
            await hit.collider.GetComponent<Mirror>().ReflectShoot(Direction, count);
        }
        else if (hit.collider && hit.collider.CompareTag("Ghost"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            lineRenderer.SetPosition(1, hit.transform.position);
            await hit.collider.GetComponent<Ghost>().PassShoot(Direction, count);
        }
        else if (hit.collider && hit.collider.CompareTag("Door"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            lineRenderer.SetPosition(1, hit.transform.position);
            await Task.Delay(1000);
            lineRenderer.positionCount = 0;
        }
        else
        {
            Debug.DrawLine(transform.position, Direction * 10000f, Color.red);
            lineRenderer.SetPosition(1, transform.localPosition + (Vector3)Direction * 10f * Define.TileOffset);
            await Task.Delay(1000);
            lineRenderer.positionCount = 0;
            
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
