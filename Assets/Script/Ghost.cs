using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
public class Ghost : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer spriteRenderer;
    public Vector2 Direction;
    void Start()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, (Vector3)Direction);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //GameManager.instance.StepSubject.Subscribe(_ => Step());
        Hide();
    }
    private void Update()
    {
        Hide();
    }
    public void Kill()
    {
        Show();
        Debug.Log("kill");
    }
    public async Task Dead()
    {
        Debug.Log("Ghost dead");
    }

    public async Task PassShoot(Vector2 direction,int count)
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction * Define.TileOffset / 2, direction);
        if (hit.collider && hit.collider.CompareTag("Mirror"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Player.instance.lineRenderer.positionCount = count + 1;
            Player.instance.lineRenderer.SetPosition(count, hit.transform.position);
            await hit.collider.GetComponent<Mirror>().ReflectShoot(direction,++count);
        }
        else if (hit.collider && hit.collider.CompareTag("Ghost"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Player.instance.lineRenderer.positionCount = count + 1;
            Player.instance.lineRenderer.SetPosition(count, hit.transform.position);
            await hit.collider.GetComponent<Ghost>().PassShoot(direction, ++count);
        }
        else if (hit.collider && hit.collider.CompareTag("Player"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Player.instance.lineRenderer.positionCount = count + 1;
            Player.instance.lineRenderer.SetPosition(count, hit.transform.position);
            await hit.collider.GetComponent<Player>().Dead();
        }
        else
        {
            Debug.DrawLine(transform.position, direction * 10000f, Color.red);
        }
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
            await hit.collider.GetComponent<Ghost>().Pass(direction);
        }
        else if (hit.collider && hit.collider.CompareTag("Player"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Player>().Pass(direction);
        }
        else
        {
            Debug.DrawLine(transform.position, direction * 10000f, Color.red); 
        }
    }

    public async Task Reveal(Vector2 direction)
    {
        Show();
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction * Define.TileOffset/2, direction);
        if (hit.collider && hit.collider.CompareTag("Mirror"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Mirror>().Reflect(direction);
        }
        else if (hit.collider && hit.collider.CompareTag("Ghost"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            await hit.collider.GetComponent<Ghost>().Pass(direction);
        }
        else
        {
            Debug.DrawLine(transform.position, direction * 10000f, Color.red);
        }

    }

    public void Rotate(Vector2 direction)
    {
        Direction = direction;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, (Vector3)Direction);
    }
    public void Show()
    {
        spriteRenderer.enabled = true;
    }
    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
    public void Step()
    {

    }
}
