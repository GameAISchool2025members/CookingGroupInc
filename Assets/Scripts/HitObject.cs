using UnityEngine;

public class HitObject : MonoBehaviour
{
    bool hittingObject;
    GameObject currenObject;

    public bool SeeIfHit()
    {
        if (hittingObject)
        {
            Destroy(currenObject);
            hittingObject = false;
            return true;
        }


        return false;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        hittingObject = true;
        currenObject = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hittingObject=false;
    }
}
