using System.Collections;
using UnityEngine;

public class CustomerManager : MonoBehaviour, Interaction
{
    [SerializeField]
    GameObject customerObject;

    [SerializeField]
    GameObject textBubble;

    [SerializeField]
    PlayerInventory inventory;

    [SerializeField]
    TimeScript timeObject;

    [SerializeField]
    CreativeCookv2 creativeCookv2;

    bool isShowing = false;
    public void Interact()
    {
        if (inventory.HaveCake)
        {
            timeObject.AddTime(Random.Range(10f,30f));
            inventory.HaveCake = false;
            creativeCookv2.ClearIngredients();
            customerObject.GetComponent<Animator>().SetTrigger("NewCustomer");
            inventory.startedGenerating = false;
        }
        else
        {
            if (isShowing)
            {
                return;
            }

            isShowing = true;
            StartCoroutine("BubbleShowing");
        }


    }

    IEnumerator BubbleShowing()
    {
        textBubble.SetActive(true);
        yield return new WaitForSeconds(3f);
        isShowing = false;
        textBubble.SetActive(false);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldState.animationDone)
        {
            customerObject.SetActive(true);
        }
    }
}
