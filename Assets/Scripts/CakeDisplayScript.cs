using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CakeDisplayScript : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] GameObject CakeUI;
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text Desc;
    

    private void Start()
    {
        closeButton.onClick.AddListener(() => CloseWindow());
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void CloseWindow()
    {
        CakeUI.SetActive(false);
        //Title.text = "";
        //Desc.text = "";
        Time.timeScale = 1;
    }
}
