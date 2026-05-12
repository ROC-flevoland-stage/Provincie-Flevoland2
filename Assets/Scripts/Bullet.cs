using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float maxLifetime = 5f;

    float lifetime;

    void Update()
    {
        // Bullet beweegt in de richting van zijn forward vector
        transform.position += transform.forward * speed * Time.deltaTime;

        // destroy na maxLifetime seconden zodat het niet oneindig blijft bestaan
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Verwijder de bullet als hij een object raakt met de tag "Hit"
        if (other.CompareTag("Hit"))
        {
            Destroy(gameObject);
        }
    }

    internal void SetDirection(Vector3 direction)
    {
        throw new NotImplementedException();
    }
}
