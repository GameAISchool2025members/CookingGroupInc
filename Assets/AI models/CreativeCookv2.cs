using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro; //used for string.join

//TODO - Add a text box for the food description genereated by the LLM

#region API Data 
//Data for Llama LLM
[System.Serializable]
public class LLMChatMessage { public string role; public string content; }
[System.Serializable]
public class LLMChoice { public LLMChatMessage message; }
[System.Serializable]
public class LLMRequest { public string model; public List<LLMChatMessage> messages = new List<LLMChatMessage>(); public bool stream = false; }
[System.Serializable]
public class LLMResponse { public List<LLMChoice> Choices; }

//Data for Locally hosted stable diffusion
[System.Serializable] 
public class LocalSDRequest { public string prompt; public string negative_prompt = "blurry, bad art, poorly drawn, text, watermark, multiple objects"; public int steps = 25; public int width = 180; public int height = 180; public float cfg_scale = 7; public string sampler_name = "DPM++ 2M Karras"; }
[System.Serializable]
public class LocalSDResponse { public string[] images; }
#endregion

public class CreativeCookv2 : MonoBehaviour
{
    [Header ("API config")]
    [SerializeField] private string llmApiEndpoint = "http://127.0.0.1:11434/v1/chat/completions";
    [SerializeField] private string llmModelName = "gemma:2b";
    [SerializeField] private string imageGenApiEndpoint = "http://127.0.0.1:7860/sdapi/v1/txt2img";

    [Header("UI Elements")]
    [SerializeField] private Button cookButton;
    [SerializeField] private Text selectedIngredientsText;
    [SerializeField] private TMP_Text dishNameText;
    [SerializeField] private RawImage generatedImageDisplay;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private TMP_Text dishDescription;

    [Header("Dish Preview")]
    [SerializeField] private GameObject previewPannel;
    [SerializeField] private RawImage previewImage;
    [SerializeField] private Button closePreview;

    [Header("Dish display")]
    [Tooltip("2D object with a sprite renderer")]
    [SerializeField] private SpriteRenderer dishDisplay2D;

    [Header("Ingredient Buttons")]
    [Tooltip("Add all your ingredient buttons here.")]
    [SerializeField] private Button[] ingredientButtons;

    public Dictionary<string, int> currentIngredients = new Dictionary<string, int>();

    void Start()
    {
        // Setup button listeners
        //cookButton?.onClick.AddListener(CookWithIngredients);
        //closePreview?.onClick.AddListener(HideDishPreview);

        foreach (var button in ingredientButtons)
        {
            // Get the ingredient name from the button's text component
            string ingredientName = button.GetComponentInChildren<Text>().text;
            button.onClick.AddListener(() => AddIngredient(ingredientName));
        }

        if (loadingIndicator) loadingIndicator.SetActive(false);
        //UpdateIngredientListUI();
    }

   
    public void AddIngredient(string ingredient)
    {
        if(currentIngredients.ContainsKey(ingredient))
        {
            //increments if ingredient already is in the list
            currentIngredients[ingredient]++;
        }
        else
        {
            //if it isn't, add it 
            currentIngredients.Add(ingredient, 1);
        }

        //UpdateIngredientListUI();
    }

    public void ClearIngredients()
    {
        currentIngredients.Clear();
        //UpdateIngredientListUI();
        //dishNameText.text = "Awaiting new creation...";
        generatedImageDisplay.texture = null;

    }

    /*private void UpdateIngredientListUI()
    {
        if (currentIngredients.Count == 0)
        {
            selectedIngredientsText.text = "Select your ingredients...";
        }
        else
        {
            //force a redraw due to bug
            selectedIngredientsText.gameObject.SetActive(false);
            //uses LINQ to format each ingredient and its count, e.g., "ingredient (x2)"
            var formattedIngredients = currentIngredients.Select(kvp => $"{kvp.Key} (x{kvp.Value})");
            selectedIngredientsText.text = string.Join(", ", formattedIngredients);
            selectedIngredientsText.gameObject.SetActive(true);
        }
    }*/

    public async Task CookWithIngredients()
    {
        if (currentIngredients.Count == 0)
        {
            Debug.LogError("No ingredients selected!");
            return;
        }

        //SetUIState(isGenerating: true);

        //formats dictionary into a text string for prompt
        var formattedIngredients = currentIngredients.Select(kvp => $"{kvp.Key} (quantity: {kvp.Value})");
        string ingredientsListString = string.Join(", ", formattedIngredients);
        string llmPrompt = $"Create a cake name and a short visual descriptive prompt for an image generation AI that decribes the flavour of the cake, the description must be less than 10 words in length at all times. The dish must be based on the following ingredients: {ingredientsListString}. Use the format: NAME: [Dish Name] PROMPT: [Visual Description]";

        string llmResponse = await SendChatRequest("You are a master cheif maker.", llmPrompt);

        if (string.IsNullOrEmpty(llmResponse))
        {
            Debug.LogError("LLM failed to generate a response.");
            SetUIState(isGenerating: false);
            return;
        }

        llmResponse = llmResponse.Trim('*').Trim('*');

        string dishName = "Unnamed Creation";
        string imagePrompt = "a dish";
        try
        {
            string[] parts = llmResponse.Split(new[] { "PROMPT:" }, StringSplitOptions.None);
            dishName = parts[0].Replace("NAME:", "").Trim();
            imagePrompt = parts[1].Trim();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse LLM response. Using fallback prompts. Error: " + e.Message);
            //if parsing fails, try to generate an image from the raw response
            imagePrompt = llmResponse;
        }

        dishNameText.text = dishName;
        dishDescription.text = imagePrompt;
        Debug.Log($"LLM generated dish name: {dishName}");
        Debug.Log($"LLM generated image prompt: {imagePrompt}");

        
        string finalImagePrompt = $"(photograph of: {imagePrompt}), minimalistic, on a single white plate with no background or a solid colour background";
        Texture2D generatedTexture = await GetGeneratedImage(finalImagePrompt);

        if (generatedTexture != null)
        {
            //float aspectRatio = (float)generatedTexture.width / (float)generatedTexture.height;
            generatedImageDisplay.texture = generatedTexture;

            if (dishDisplay2D != null)
            {
                Rect rect = new Rect(0, 0, generatedTexture.width, generatedTexture.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite newSprite = Sprite.Create(generatedTexture, rect, pivot);
                dishDisplay2D.sprite = newSprite;
            }
        }
        else
        {
            Debug.LogError("Failed to generate image.");
        }

        //SetUIState(isGenerating: false);
    }

    //public void ShowDishPreview(Texture2D texture)
    //{
    //    if (previewPannel != null && previewImage != null)
    //    {
    //        previewImage.texture = texture;
    //        previewPannel.SetActive(true);
    //    }
    //}

    //private void HideDishPreview()
    //{
    //    if(previewPannel != null)
    //    {
    //        previewPannel.SetActive(false);
    //    }
    //}

    #region API Communication Methods
    private async Task<string> SendChatRequest(string systemMessage, string userMessage)
    {
        LlmRequest requestBody = new LlmRequest
        {
            model = llmModelName,
            messages = new List<LlmChatMessage>
            {
                new LlmChatMessage { role = "system", content = systemMessage },
                new LlmChatMessage { role = "user", content = userMessage }
            }
        };
        string jsonRequestBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
        using (UnityWebRequest request = new UnityWebRequest(llmApiEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            var asyncOp = request.SendWebRequest();
            while (!asyncOp.isDone) { await Task.Yield(); }
            if (request.result != UnityWebRequest.Result.Success) { Debug.LogError("LLM API Error: " + request.downloadHandler.text); return null; }
            else
            {
                LlmResponse llmResponse = JsonUtility.FromJson<LlmResponse>(request.downloadHandler.text);
                if (llmResponse != null && llmResponse.choices != null && llmResponse.choices.Count > 0)
                {
                    return llmResponse.choices[0].message.content;
                }
                Debug.LogError("LLM Response was empty or invalid.");
                return null;
            }
        }
    }

    private async Task<Texture2D> GetGeneratedImage(string prompt)
    {
        LocalApiRequest requestBody = new LocalApiRequest { prompt = prompt };
        string jsonRequestBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
        using (UnityWebRequest request = new UnityWebRequest(imageGenApiEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            var asyncOp = request.SendWebRequest();
            while (!asyncOp.isDone) { await Task.Yield(); }
            if (request.result != UnityWebRequest.Result.Success) { Debug.LogError("Image Gen API Error: " + request.downloadHandler.text); return null; }
            else
            {
                LocalApiResponse apiResponse = JsonUtility.FromJson<LocalApiResponse>(request.downloadHandler.text);
                if (apiResponse != null && apiResponse.images != null && apiResponse.images.Length > 0)
                {
                    byte[] imageBytes = Convert.FromBase64String(apiResponse.images[0]);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    return tex;
                }
                Debug.LogError("Image Gen Response was empty or invalid.");
                return null;
            }
        }
    }
    #endregion

    private void SetUIState(bool isGenerating)
    {
        cookButton.interactable = !isGenerating;
        foreach (var button in ingredientButtons) { button.interactable = !isGenerating; }
        if (loadingIndicator) loadingIndicator.SetActive(isGenerating);
    }
}
