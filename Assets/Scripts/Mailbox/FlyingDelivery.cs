using UnityEngine;

public class FlyingDelivery : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 3f; // Temps avant que le sprite disparaisse
    public Vector3 flyDirection = new Vector3(1, 1, 0); // Diagonale haut-droite par défaut

    void Start()
    {
        // On normalise la direction pour que la vitesse soit constante
        flyDirection = flyDirection.normalized;

        // Détruit cet objet après 'lifeTime' secondes
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Déplace l'objet à chaque frame
        transform.position += flyDirection * speed * Time.deltaTime;
    }
}