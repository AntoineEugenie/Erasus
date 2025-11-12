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

    void OnEnable()
    {
        initialPos = transform.position;
        Debug.Log(initialPos + " / " + transform.position);
    }

    void Update()
    {
        if (finished) {
            transform.position = Vector3.MoveTowards(transform.position, initialPos, speed * Time.deltaTime);
        }
        else
        {
           
            Debug.Log("Target pos: "+target);
            // Déplacement
            
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
    }

    void FindPlant()
    {   
        Targets = new();
        foreach (GameObject plant in GameObject.FindGameObjectsWithTag("Plant"))
        {
            Vector3 pos = plant.transform.position;
            //Debug.Log(!Targets.ContainsKey(pos));
            //if (!Targets.ContainsKey(pos))
            //{
            Targets.Add(pos, false);
            finished = false; // Relance si une nouvelle plante arrive
            //}
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
