using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeScript : MonoBehaviour
{
    [SerializeField]
    float timeBeforeTheEndOfGame;
    float startTime;
    float timeRemaining;

    TMP_Text text;
    private void OnEnable()
    {
        startTime = Time.time;
        timeRemaining = 0f;
    }

    public void AddTime(float addMoreTime)
    {
        timeBeforeTheEndOfGame += addMoreTime;
    }

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining = timeBeforeTheEndOfGame-(Time.time - startTime);
        int minuteDisplay = (int)(timeRemaining / 60f);
        int secondsDisplay = (int)(timeRemaining % 60f);
        text.text = $"{minuteDisplay}:{secondsDisplay}";

        if (timeRemaining <= 0f)
        {
            SceneManager.LoadScene("SampleScene",LoadSceneMode.Single);
        }
    }
}
