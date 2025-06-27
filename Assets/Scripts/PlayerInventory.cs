using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<string> ingriedientNames= new List<string>();

    [SerializeField] int amountOfIngredientRequierdToMake;
    public bool startedGenerating = false;
    public bool doneGenerating = false;
    public bool HaveCake = false;

    [SerializeField]
    CreativeCookv2 creativeCookv2;


    public void AddIngredient(string nameOfIng)
    {
        ingriedientNames.Add(nameOfIng);
        creativeCookv2.currentIngredients.Add(nameOfIng, 1);
    }


    private void Update()
    {
        if (amountOfIngredientRequierdToMake <= ingriedientNames.Count && !startedGenerating)
        {
            startedGenerating = true;
            doneGenerating = false;
            StartCoroutine("GeneratingImage");
            Debug.Log("Generating");
        }
    }

    private IEnumerator GeneratingImage()
    {
        Task task = creativeCookv2.CookWithIngredients();
        yield return new WaitUntil(() => task.IsCompleted); ; //Generate the image here
        Debug.Log("Done");
        doneGenerating = true;
        yield return null;
    }

    public void ResetList()
    {
        ingriedientNames.Clear();
    }
}
