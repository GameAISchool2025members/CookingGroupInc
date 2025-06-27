using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!WorldState.animationDone)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            SceneManager.LoadScene("SampleScene");
        }


        if (Input.GetKey(KeyCode.D) && gameObject.transform.position.x <= 18.2f)
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A) && gameObject.transform.position.x >= -1.3)
        {
            gameObject.transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }
}
