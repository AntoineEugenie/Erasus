using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float cycle;
    private float time;

    void Awake()
    {
        time = cycle;
    }

    // Update is called once per frame
   
    private void Update()
        {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            TimeEvents.newDay.Invoke(); // Invoque l'événement pour signaler un nouveau jour
            time = cycle;
        }
    }

}

