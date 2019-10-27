using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody Rb;
    float fuerza=10.0f;
    Vector3 currentDireccion = Vector3.zero;
    int bounces = 0;
    public int limitofBounces = 3;
    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    }

    public void Shoot(Vector3 direccion)
    {
        currentDireccion = direccion;
        Rb.AddForce(direccion * fuerza * 30.0f);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (bounces < limitofBounces && !collision.collider.CompareTag("Player"))
        {
            Vector3 normal = collision.GetContact(0).normal;
            Vector3 newDireccion = Vector3.Reflect(currentDireccion, normal);
            Rb.velocity = Vector3.zero;
            Shoot(newDireccion.normalized);
            transform.LookAt(transform.position+newDireccion);
            //Debug.Log("normal " + normal +" collider "+ collision.collider.name);
            bounces++;
        }
        else
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<TankManager>().TakeDMG(30);
            }
            Destroy(gameObject);
        }
    }
}
