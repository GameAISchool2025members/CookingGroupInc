using UnityEngine;

public class PlayerInteracting : MonoBehaviour
{

    [SerializeField]
    private GameObject showInteractionDisplay;

    public GameObject ShowInteractionDisplay
    {
        get { return showInteractionDisplay; }
    }



    Interaction interactionObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interaction interactiveObject))
        {
                showInteractionDisplay.SetActive(true);
                interactionObject = interactiveObject;
            
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactionObject?.Interact();
            showInteractionDisplay?.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        showInteractionDisplay.SetActive(false);
        interactionObject = null;
    }

}
