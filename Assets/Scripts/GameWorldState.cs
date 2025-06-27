using UnityEngine;

public class GameWorldState : MonoBehaviour
{
    [SerializeField]
    GameObject UIObject;


    public void DoneWithAnimaiton()
    {
        WorldState.animationDone = true;
        UIObject.SetActive(true);
    }




}


public static class WorldState
{
    public static bool animationDone = false;
}
