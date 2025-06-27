using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using System;
using System.Collections.Generic;

[System.Serializable]
public class LlmChatMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class LlmChoice
{
    public LlmChatMessage message;
    public int index;
    public string finish_reason;
}

[System.Serializable]
public class LlmRequest
{
    public string model;
    public List<LlmChatMessage> messages = new List<LlmChatMessage>();
    public bool stream = false;
}

[System.Serializable]
public class LlmResponse
{
    // The key change is here: the response contains a list of "choices"
    public List<LlmChoice> choices;
    public string id;
    public string model;
    public DateTime created_at;
    public bool done;
}


public class LocalLLMScript : MonoBehaviour
{
    [Header("Local LLM API Configuration")]
    [Tooltip("The local URL for the Ollama/LM Studio API.")]
    [SerializeField]
    private string apiEndpoint = "http://127.0.0.1:11434/v1/chat/completions";

    [Tooltip("The name of the model to use (e.g., 'llama3.1').")]
    [SerializeField]
    private string modelName = "llama3.1";


    [Header("UI Elements")]
    [SerializeField]
    private InputField llmPromptInputField;
    [SerializeField]
    private Button generateTextButton;
    [SerializeField]
    private Text responseTextDisplay;
    [SerializeField]
    private GameObject llmLoadingIndicator;

    void Start()
    {
        generateTextButton.onClick.AddListener(() => GetLlmResponse());
        if (llmLoadingIndicator) llmLoadingIndicator.SetActive(false);
    }

    public async void GetLlmResponse()
    {
        string userPrompt = llmPromptInputField.text;
        if (string.IsNullOrEmpty(userPrompt))
        {
            Debug.LogError("LLM Prompt is empty!");
            return;
        }

        SetUIState(isGenerating: true);

        string systemPrompt = "You are a witty and slightly sarcastic waiter in a fantastical restaurant. Keep your responses brief and in character.";

        string response = await SendChatRequest(systemPrompt, userPrompt);

        if (!string.IsNullOrEmpty(response))
        {
            responseTextDisplay.text = response;
        }
        else
        {
            Debug.LogError("Failed to get LLM response. Is your local LLM server (Ollama, LM Studio) running?");
            responseTextDisplay.text = "Error: Could not connect to local LLM.";
        }

        SetUIState(isGenerating: false);
    }

    private async Task<string> SendChatRequest(string systemMessage, string userMessage)
    {
        LlmRequest requestBody = new LlmRequest
        {
            model = modelName,
            messages = new List<LlmChatMessage>
            {
                new LlmChatMessage { role = "system", content = systemMessage },
                new LlmChatMessage { role = "user", content = userMessage }
            }
        };

        string jsonRequestBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);

        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var asyncOp = request.SendWebRequest();
            while (!asyncOp.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Local LLM API Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                return null;
            }
            else
            {
                // The parsing logic is now updated to look inside the 'choices' list
                LlmResponse llmResponse = JsonUtility.FromJson<LlmResponse>(request.downloadHandler.text);

                if (llmResponse != null && llmResponse.choices != null && llmResponse.choices.Count > 0)
                {
                    // Get the message from the first choice in the list
                    return llmResponse.choices[0].message.content;
                }
                else
                {
                    Debug.LogError("Received a response, but it was empty or in an unexpected format.");
                    Debug.Log("Raw Response: " + request.downloadHandler.text);
                    return null;
                }
            }
        }
    }

    private void SetUIState(bool isGenerating)
    {
        generateTextButton.interactable = !isGenerating;
        llmPromptInputField.interactable = !isGenerating;
        if (llmLoadingIndicator) llmLoadingIndicator.SetActive(isGenerating);
    }
}