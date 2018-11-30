using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatController : MonoBehaviour {

    public int crew = 10,    //human
               energy = 100,  //fuel
               supply = 100;  //food,water

    public int[] triggeredEvents;
    public Text text_crew, text_energy, text_supply;

    private void Update()
    {
        text_crew.text = crew.ToString();
        text_energy.text = energy.ToString()+"%";
        text_supply.text = supply.ToString() + "%";
    }
}
