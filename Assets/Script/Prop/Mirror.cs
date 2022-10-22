using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading;
using System.Threading.Tasks;
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
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction / 2, direction);
            
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
                    hit.collider.GetComponent<Ghost>().Kill();
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

    public async Task ReflectShoot(Vector2 direction)
    {
        if (!ShootReflected && (direction == EnableVector1 || direction == EnableVector2))
        {
            direction = direction == EnableVector1 ? -EnableVector2 : -EnableVector1;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + direction / 2, direction);

            if (hit.collider && hit.collider.CompareTag("Mirror"))
            {
                ShootReflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                await hit.collider.GetComponent<Mirror>().ReflectShoot(direction);
            }
            else if (hit.collider && hit.collider.CompareTag("Ghost"))
            {
                ShootReflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                await hit.collider.GetComponent<Ghost>().Dead();
            }
            else if (hit.collider && hit.collider.CompareTag("Player"))
            {
                ShootReflected = true;
                Debug.DrawLine(transform.position, hit.point, Color.red);
                hit.collider.GetComponent<Player>().Dead();
            }
            else
            {
                Debug.DrawLine(transform.position, direction * 10000f, Color.red);
            }
        }
        ShootReflected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
