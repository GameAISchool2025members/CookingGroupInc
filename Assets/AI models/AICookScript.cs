using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using System;

[System.Serializable]
public class LocalApiRequest
{
    //api settings for local 
    public string prompt;
    public string negative_prompt = "blurry, bad art, poorly drawn, text, watermark";
    public int steps = 25;
    public int width = 512;
    public int height = 512;
    public int n_iter = 1;
    public int batch_size = 1;
    public float cfg_scale = 7;
    public string sampler_name = "DPM++ 2M Karras"; // A popular, fast sampler
}

[System.Serializable]
public class LocalApiResponse
{
    public string[] images; // The API returns an array of base64 encoded strings
    public string info;
}

public class AICookScript : MonoBehaviour
{
    //UI & API Configuration
    [Header("Local API Configuration")]
    [Tooltip("The local URL for the AUTOMATIC1111 API.")]
    [SerializeField]
    private string apiEndpoint = "http://127.0.0.1:7860/sdapi/v1/txt2img";

    [Header("UI Elements")]
    [SerializeField]
    private InputField promptInputField;
    [SerializeField]
    private Button generateButton;
    [SerializeField]
    private RawImage generatedImageDisplay;
    [SerializeField]
    private GameObject loadingIndicator;


    void Start()
    {
        generateButton.onClick.AddListener(() => GenerateImageFromPrompt());
        if (loadingIndicator) loadingIndicator.SetActive(false);
    }

    public async void GenerateImageFromPrompt()
    {
        string userPrompt = promptInputField.text;
        if (string.IsNullOrEmpty(userPrompt))
        {
            Debug.LogError("Prompt is empty! Please enter a food order.");
            return;
        }

        SetUIState(isGenerating: true);

        // Prompt for model for extra detail
        string finalPrompt = $"(photograph of a {userPrompt} on a white plate with no background or a solid colour background.) Video game, Game Object, Minimalistic";

        Texture2D generatedTexture = await GetGeneratedImage(finalPrompt);

        if (generatedTexture != null)
        {
            generatedImageDisplay.texture = generatedTexture;
        }
        else
        {
            Debug.LogError("Failed to generate image. Is your local Stable Diffusion server running with the --api flag?");
        }

        SetUIState(isGenerating: false);    }

    private async Task<Texture2D> GetGeneratedImage(string prompt)
    {
        //Create the request for the api
        LocalApiRequest requestBody = new LocalApiRequest { prompt = prompt };
        string jsonRequestBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);

        //take the request and create the web request
        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            //send the request and wait
            var asyncOp = request.SendWebRequest();
            while (!asyncOp.isDone)
            {
                // You could update a progress bar here based on `asyncOp.progress`
                await Task.Yield();
            }

            //handle the response
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Local API Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                return null;
            }
            else
            {
                LocalApiResponse apiResponse = JsonUtility.FromJson<LocalApiResponse>(request.downloadHandler.text);

                if (apiResponse.images == null || apiResponse.images.Length == 0)
                {
                    Debug.LogError("No image data received in the local API response.");
                    return null;
                }

                string imageBase64 = apiResponse.images[0];

                //decode the base64 string into an image
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(imageBase64);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    return tex;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to decode base64 string: " + e.Message);
                    return null;
                }
            }
        }
    }

    private void SetUIState(bool isGenerating)
    {
        generateButton.interactable = !isGenerating;
        promptInputField.interactable = !isGenerating;
        if (loadingIndicator) loadingIndicator.SetActive(isGenerating);
    }
}
