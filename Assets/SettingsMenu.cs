using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void displayMenu()
    {
        gameObject.SetActive(true);
    }
    public void hideMenu()
    {
        gameObject.SetActive(false);
    }
}
