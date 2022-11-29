using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider attraction, speed, agility, alignment, cohesion, separation;
    public ParticleController particleController;
    public GameObject boidSettings;
    private void Start()
    {
        gameObject.SetActive(false);
        if (particleController.state == ParticleController.State.Boid)
        {
            boidSettings.SetActive(true);
        }
        else
        {
            boidSettings.SetActive(false);
        }
    }
    public void displayMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void enterFreeFly()
    {
        particleController.state = ParticleController.State.FreeFly;
        boidSettings.SetActive(false);
    }
    public void enterBoid()
    {
        particleController.state = ParticleController.State.Boid;
        boidSettings.SetActive(true);
    }
}
