using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    float cycle = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
   
    private void Update()
        {
        cycle -= Time.deltaTime;
        if (cycle <= 0)
        {
            TimeEvents.newDay.Invoke(); // Invoque l'événement pour signaler un nouveau jour
            cycle = 2f;
        }
    }

}

