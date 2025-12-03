using System.Collections;
using UnityEngine;

public class PandaVoleur : MonoBehaviour, IRaycastable
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
        // Si on vole -> vitesse course, sinon -> vitesse marche
        float currentMaxSpeed = isStealing ? runSpeed : walkSpeed;

        if (isStealing)
        {
            HandleStealingBehavior(currentMaxSpeed);
        }
        else
        {
            HandleWanderBehavior(currentMaxSpeed);
        }

        // MISE A JOUR ANIMATION
        UpdateAnimation(currentMaxSpeed);
    }

    void UpdateAnimation(float intentedSpeed)
    {
        float distance = Vector3.Distance(transform.position, currentTarget);
        bool isMoving = !isWaiting && distance > 0.1f;

        float currentSpeedForAnim = isMoving ? intentedSpeed : 0f;

        // AVANT (avec lissage) :
        // animator.SetFloat("Speed", currentSpeedForAnim, 0.1f, Time.deltaTime);

        // MAINTENANT (Instantané) :
        // On envoie la valeur brute directement.
        animator.SetFloat("Speed", currentSpeedForAnim);
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

    // J'ai ajouté l'argument "speed" ici pour pouvoir varier entre marche et course
    void MoveTo(Vector3 target, float currentSpeed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

        Vector3 direction = target - transform.position;

        // --- CORRECTION DU STRETCH ---
        // On ne touche plus à transform.rotation !

        // Si on va vers la gauche (x négatif), on flip le sprite
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Ou false, ça dépend du sens de ton dessin original
        }
        // Si on va vers la droite (x positif), on remet le sprite normal
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    // ... (Le reste : WaitBeforeNextMove, SetRandomWanderTarget, OnHitByRaycast ne change pas) ...
    // Je remets les fonctions manquantes pour que tu aies le bloc complet si besoin de copier-coller

    IEnumerator WaitBeforeNextMove()
    {
        isWaiting = true;
        // Note: UpdateAnimation mettra la speed à 0 automatiquement ici
        float waitTime = Random.Range(pauseDurationRange.x, pauseDurationRange.y);
        yield return new WaitForSeconds(waitTime);

        SetRandomWanderTarget();
        isWaiting = false;
    }

    void SetRandomWanderTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        currentTarget = initialPos + new Vector3(randomPoint.x, randomPoint.y, 0);
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