using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftiingSystem : MonoBehaviour
{

    public GameObject craftingScreenUI;
    public GameObject craftingScreenArmas;
    public GameObject craftingArmas;
    public GameObject craftingEspadaDoom;



    bool isOpen;

    // public static CraftiingSystem Instance {get;set;}
    // private void Awake()
    // {
    //     if(Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         Instance = this;
    //     }
    // }
    
    void Start()
    {
        isOpen = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && !isOpen)
        {
            craftingScreenUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            craftingScreenArmas.SetActive(false);
            craftingArmas.SetActive(false);
            craftingEspadaDoom.SetActive(false);
            isOpen = false;
        }
    }
}
