/*using UnityEngine;

public class FoodTray : MonoBehaviour
{
    public Texture2D dishTexture { get; private set; }

    private CreativeCookv2 CreativeCook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CreativeCook = FindObjectOfType<CreativeCookv2>();
    }

    public void SetDish(Texture2D texture)
    {
        dishTexture = texture;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            renderer.sprite = Sprite.Create(texture, rect, pivot);
        }
    }

    private void OnMouseDown()
    {
        if (dishTexture != null && CreativeCook != null)
        {
            Debug.Log("Food tray clicked, showwing preview.");
            CreativeCook.ShowDishPreview(dishTexture);
        }
    }
}*/
