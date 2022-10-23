using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading;
using System.Threading.Tasks;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Mirror : MonoBehaviour
{
    public Vector2 Normal;
    public bool Reflected = false;
    public bool ShootReflected = false;
    public Vector2 EnableVector1;
    public Vector2 EnableVector2;
    // Start is called before the first frame update
    void Start()
    {

    }

    public async Task Reflect(Vector2 direction)
    {
        if (!Reflected && (direction==EnableVector1 || direction==EnableVector2))
        {
            direction = direction == EnableVector1 ? -EnableVector2 : -EnableVector1;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction * Define.TileOffset / 2, direction);
            
            if (hit.collider && hit.collider.CompareTag("Mirror"))
            {
                Reflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                await hit.collider.GetComponent<Mirror>().Reflect(direction.normalized);
            }
            else if(hit.collider && hit.collider.CompareTag("Ghost"))
            {
                Reflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                if (-direction == hit.collider.GetComponent<Ghost>().Direction)
                    hit.collider.GetComponent<Ghost>().Kill(transform.position);
                else
                    await hit.collider.GetComponent<Ghost>().Reveal(direction.normalized);
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
       
        Reflected = false;
    }

    public async Task ReflectShoot(Vector2 direction,int count)
    {
        if (!ShootReflected && (direction == EnableVector1 || direction == EnableVector2))
        {
            direction = direction == EnableVector1 ? -EnableVector2 : -EnableVector1;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction * Define.TileOffset / 2, direction);

            if (hit.collider && hit.collider.CompareTag("Mirror"))
            {
                ShootReflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                Player.instance.lineRenderer.positionCount = count+1;
                Player.instance.lineRenderer.SetPosition(count, hit.transform.position);
                await hit.collider.GetComponent<Mirror>().ReflectShoot(direction, ++count);
            }
            else if (hit.collider && hit.collider.CompareTag("Ghost"))
            {
                ShootReflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                Player.instance.lineRenderer.positionCount = count+1;
                Player.instance.lineRenderer.SetPosition(count, hit.transform.position);
                await hit.collider.GetComponent<Ghost>().Dead();
            }
            else if (hit.collider && hit.collider.CompareTag("Player"))
            {
                ShootReflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                Player.instance.lineRenderer.positionCount = count + 1;
                Player.instance.lineRenderer.SetPosition(count, hit.transform.position);
                hit.collider.GetComponent<Player>().Dead();
            }
            else
            {
                Debug.DrawLine(transform.position, direction * 10000f, Color.red);
                Player.instance.lineRenderer.positionCount = count + 1;
                Player.instance.lineRenderer.SetPosition(count, this.transform.position + (Vector3)direction * 10f * Define.TileOffset);
            }
            await Task.Delay(1000);
            Player.instance.lineRenderer.positionCount = 0;
        }
        ShootReflected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
