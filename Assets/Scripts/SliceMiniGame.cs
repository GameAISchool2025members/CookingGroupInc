using System.Collections;
using TMPro;
using UnityEngine;

public class SliceMiniGame : MonoBehaviour, Interaction
{
    PlayerController controller;
    bool startGame = false;
    [SerializeField]
    GameObject textInformation;
    [SerializeField]
    GameObject IngriedientPopUp;
    [SerializeField]
    TMP_Text textToDisplay;
    [SerializeField]
    GameObject ParticleEffect;
    [SerializeField]
    Transform placementOfEffect;

    int amountOfPresses;
    float timeWhenStarted;
    public bool playOnce = false;
    int chopAmount = 130;
    public void Interact()
    {
        if (playOnce) {
            return; 
        
        }
        playOnce = true;


        controller = GameObject.Find("Player").GetComponent<PlayerController>();

        controller.enabled = false;

        startGame = true;
        timeWhenStarted = Time.time;
        IngriedientPopUp.SetActive(true);
        textInformation.SetActive(true);
        //Start MiniGame
        //Click over a certain Time
        //5 seconds
    }

    private void Update()
    {
        if (!startGame || IngriedientPopUp.gameObject.active == true)
        {
            return;
        }   
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            amountOfPresses++;
            Instantiate(ParticleEffect, placementOfEffect);
        }

        int amountOfChopsLeft = chopAmount - amountOfPresses;

        textToDisplay.text = $"{amountOfChopsLeft} More Chops left \n Press P!!!";

        if (amountOfChopsLeft <= 0)
        {
            startGame = false;
            controller.enabled = true;
            textInformation.SetActive(false);
            textToDisplay.text = "";
            amountOfPresses = 0;

        }

    }

}
