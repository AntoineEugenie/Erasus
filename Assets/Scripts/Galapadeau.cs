using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Galapadeau : MonoBehaviour, IRaycastable
{
    public float speed;
    private Dictionary<Vector3, bool> Targets;
    private Vector3 target;
    private bool finished = true;
    private Vector3 initialPos;

    // --- NOUVEAU : Variables pour l'animation ---
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastDirection;

    void OnEnable()
    {
        initialPos = transform.position;

        // On récupère les composants attachés au Galapadeau
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Log(initialPos + " / " + transform.position);
    }

    void Update()
    {
        // On mémorise la position AVANT le déplacement pour calculer la direction
        Vector3 startPos = transform.position;

        if (finished)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPos, speed * Time.deltaTime);
        }
        else
        {
            // Déplacement vers la plante
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // Vérifie si la cible est atteinte (avec une tolérance)
            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                if (Targets.ContainsKey(target) && !Targets[target])
                {
                    Targets[target] = true;

                    GameManager.instance.tileManager.ChangeWaterLevel(
                        Vector3Int.FloorToInt(target), 1, SceneManager.GetActiveScene().name);

                    if (AllTargetsVisited())
                    {
                        finished = true;
                        Debug.Log("Galapadeau a fini d’arroser toutes les plantes.");
                    }
                    else
                    {
                        target = NextPlant(); // Prochaine cible
                    }
                }
            }
        }

        // --- NOUVEAU : On met à jour l'animation ---
        UpdateAnimation(startPos);
    }

    void UpdateAnimation(Vector3 startPos)
    {
        // On calcule la direction réelle prise pendant cette frame
        Vector3 dir = (transform.position - startPos).normalized;

        // S'il s'est déplacé, la magnitude sera supérieure à 0
        bool isMoving = dir.magnitude > 0.01f;

        if (isMoving)
        {
            // On sauvegarde la direction pour l'Animator
            lastDirection = new Vector2(dir.x, dir.y);

            // Gestion du Sprite miroir pour la Gauche/Droite
            if (dir.x < -0.01f) spriteRenderer.flipX = true;
            else if (dir.x > 0.01f) spriteRenderer.flipX = false;
        }

        // On envoie les infos à l'Animator (sécurité au cas où il n'y en a pas encore)
        if (animator != null)
        {
            animator.SetFloat("Speed", isMoving ? speed : 0f);

            if (isMoving)
            {
                animator.SetFloat("MoveX", lastDirection.x);
                animator.SetFloat("MoveY", lastDirection.y);
            }
        }
    }

    void FindPlant()
    {
        Targets = new();
        foreach (GameObject plant in GameObject.FindGameObjectsWithTag("Plant"))
        {
            Vector3 pos = plant.transform.position;
            Targets.Add(pos, false);
            finished = false; // Relance si une nouvelle plante arrive
        }
    }

    Vector3 NextPlant()
    {
        foreach (var kvp in Targets)
        {
            if (!kvp.Value)
            {
                return kvp.Key;
            }
        }
        return initialPos;
    }

    bool AllTargetsVisited()
    {
        foreach (bool visited in Targets.Values)
        {
            if (!visited) return false;
        }
        return true;
    }

    public void OnHitByRaycast()
    {
        Debug.Log("Galapadeau touchée ! Arrosage en cours...");
        FindPlant();
        target = NextPlant();
    }
}