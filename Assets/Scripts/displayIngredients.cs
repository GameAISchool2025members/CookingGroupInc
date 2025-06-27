using TMPro;
using UnityEngine;

public class displayIngredients : MonoBehaviour
{
    [SerializeField]
    PlayerInventory playerInventory;

    string textString;

    TMP_Text displayText;

    private void Start()
    {
        displayText = GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
        textString = "ingredients:\n";
        foreach (var item in playerInventory.ingriedientNames)
        {
            textString += (item + "\n");
        }

        displayText.text = textString;

    }
}
