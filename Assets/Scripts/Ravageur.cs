using System.Collections;
using UnityEngine;

public class Ravageur : MonoBehaviour, IRaycastable
{
    [Header("Paramètres de Mouvement")]
    public float walkSpeed = 3.5f; // Vitesse de balade
    public float runSpeed = 7.0f;  // Vitesse quand il vole (plus rapide !)
    public float wanderRadius = 5f;
    public Vector2 pauseDurationRange = new Vector2(1f, 4f);

    private Vector3 initialPos;
    private Vector3 currentTarget;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // --- NOUVEAU : Stockage de la direction ---
    private Vector2 lastDirection;

    // --- États ---
    private bool isStealing = false;
    private bool isWaiting = false;
    private bool hasStolen = false;
    private GameObject targetPlantObject;

    void OnEnable()
    {
        initialPos = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetRandomWanderTarget();
    }

    void Update()
    {
        // On détermine la vitesse actuelle selon l'état
        float currentMaxSpeed = isStealing ? runSpeed : walkSpeed;

        if (isStealing)
        {
            HandleStealingBehavior(currentMaxSpeed);
        }
        else
        {
            HandleWanderBehavior(currentMaxSpeed);
        }

        // Mise à jour de l'animation à chaque frame
        UpdateAnimation(currentMaxSpeed);
    }

    void UpdateAnimation(float intendedSpeed)
    {
        float distance = Vector3.Distance(transform.position, currentTarget);
        // On vérifie qu'on n'est pas en attente et qu'on a encore un peu de chemin à faire
        bool isMoving = !isWaiting && distance > 0.05f;

        float currentSpeedForAnim = isMoving ? intendedSpeed : 0f;
        animator.SetFloat("Speed", currentSpeedForAnim);

        // NOUVEAU : On envoie la direction au Blend Tree uniquement si on bouge
        if (isMoving)
        {
            animator.SetFloat("MoveX", lastDirection.x);
            animator.SetFloat("MoveY", lastDirection.y);
        }
    }

    void HandleWanderBehavior(float moveSpeed)
    {
        if (isWaiting) return;

        MoveTo(currentTarget, moveSpeed);

        if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            StartCoroutine(WaitBeforeNextMove());
        }
    }

    void HandleStealingBehavior(float moveSpeed)
    {
        MoveTo(currentTarget, moveSpeed);

        if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            if (!hasStolen)
            {
                if (targetPlantObject != null) Destroy(targetPlantObject);
                Debug.Log("Vol effectué ! Passage en mode COURSE pour rentrer !");
                hasStolen = true;
                currentTarget = initialPos;
            }
            else
            {
                // Arrivé au terrier
                isStealing = false;
                hasStolen = false;
                SetRandomWanderTarget();
            }
        }
    }

    void MoveTo(Vector3 target, float currentSpeed)
    {
        // On mémorise la position de départ pour trouver la vraie direction
        Vector3 startPos = transform.position;

        // Déplacement effectif
        transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

        // NOUVEAU : Calcul de la direction normalisée (entre -1 et 1)
        Vector3 dir = (target - startPos).normalized;

        // Si le personnage s'est physiquement déplacé ce coup-ci
        if (dir.magnitude > 0.01f)
        {
            // On sauvegarde cette direction pour l'Animator
            lastDirection = new Vector2(dir.x, dir.y);

            // Gestion du Sprite miroir pour la Gauche/Droite
            if (dir.x < -0.01f)
            {
                spriteRenderer.flipX = true; // Va vers la gauche : on inverse le sprite "Droite"
            }
            else if (dir.x > 0.01f)
            {
                spriteRenderer.flipX = false; // Va vers la droite : sprite normal
            }
        }
    }

    IEnumerator WaitBeforeNextMove()
    {
        isWaiting = true;
        float waitTime = Random.Range(pauseDurationRange.x, pauseDurationRange.y);
        yield return new WaitForSeconds(waitTime);

        SetRandomWanderTarget();
        isWaiting = false;
    }
    void SetRandomWanderTarget()
    {
        // On tire à pile ou face l'axe de déplacement : 0 = Horizontal (X), 1 = Vertical (Y)
        int axe = Random.Range(0, 2);

        // On choisit une distance de déplacement aléatoire
        float distance = Random.Range(-wanderRadius, wanderRadius);

        // On part de la position ACTUELLE du panda pour garantir un mouvement en ligne droite
        Vector3 newTarget = transform.position;

        if (axe == 0)
        {
            // On modifie uniquement l'axe X
            newTarget.x += distance;
        }
        else
        {
            // On modifie uniquement l'axe Y
            newTarget.y += distance;
        }

        // Sécurité : On s'assure que le panda ne s'éloigne pas trop de son point de départ (initialPos)
        // Mathf.Clamp bloque la valeur entre un minimum et un maximum
        newTarget.x = Mathf.Clamp(newTarget.x, initialPos.x - wanderRadius, initialPos.x + wanderRadius);
        newTarget.y = Mathf.Clamp(newTarget.y, initialPos.y - wanderRadius, initialPos.y + wanderRadius);

        currentTarget = newTarget;
    }

    public void OnHitByRaycast()
    {
        if (!isStealing)
        {
            GameObject[] allPlants = GameObject.FindGameObjectsWithTag("Plant");
            if (allPlants.Length > 0)
            {
                StopAllCoroutines();
                isWaiting = false;
                isStealing = true;
                targetPlantObject = allPlants[Random.Range(0, allPlants.Length)];
                currentTarget = targetPlantObject.transform.position;
            }
        }
    }
}