using System.Collections.Generic;
using UnityEngine;

public class StewMiniGameSystem : MonoBehaviour
{
    [SerializeField]
    Transform start, end;


    [SerializeField]
    GameObject hitObjects;

    int spawnAmount = 3;
    int rounds = 2;

    GameObject hitEntity;

    public bool Over = false;
    public void Start()
    {
        rounds = 4;
        Over = false;
    }

    private void Generate()
    {
        float randomX = Random.Range(start.position.x, end.position.x);
        Vector3 position = new Vector3(randomX,transform.position.y,transform.position.z);
        hitEntity = Instantiate(hitObjects,position,Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (hitEntity == null && rounds >= 1)
        {
            rounds--;
            Generate();
        }
        else if (rounds <= 0 && hitEntity == null)
        {
            Over = true;
        }
    }
}
