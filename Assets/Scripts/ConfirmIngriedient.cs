using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmIngriedient : MonoBehaviour
{
    [SerializeField]
    TMP_InputField input;
    PlayerInventory playerInventory;

    private void Start()
    {
        
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    public void ButtonPres()
    {
        playerInventory.AddIngredient(input.text.ToLower());
        input.text = "";
        Time.timeScale = 1.0f;
    }
}
