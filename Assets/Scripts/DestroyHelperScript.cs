using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyHelperScript : MonoBehaviour
{
    [SerializeField]
    private string SceneToLoad = "GameScene";

    public void ChangeLevel()
    {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
    }
}
