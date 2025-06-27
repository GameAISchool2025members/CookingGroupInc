using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCheckerScript : MonoBehaviour
{
    static CameraCheckerScript instance;
    public GameObject playerObject;

    [SerializeField]
    private float smoothTime = 1f;
    Vector3 currentVelocity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
        }
        else if (WorldState.animationDone)
        {
            transform.position = Vector3.SmoothDamp(transform.position, playerObject.transform.position + new Vector3(0, 0, -10), ref currentVelocity, smoothTime);
        }
    }
}
