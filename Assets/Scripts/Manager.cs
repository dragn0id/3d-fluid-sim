using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject optionsMenu;
    void EnableOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }
    void DisableOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (optionsMenu.activeSelf)
            {
                DisableOptionsMenu();
            }
            else
            {
                EnableOptionsMenu();
            }
        }
    }
}
