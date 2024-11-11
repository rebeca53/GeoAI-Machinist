using UnityEngine;

public class ImageMatrix : MonoBehaviour
{
    Texture2D texture;
    public float scale = 5f;
    private SpriteRenderer spriteRenderer;

    int size = 64;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        texture = new Texture2D(size, size);
        texture.wrapMode = TextureWrapMode.Repeat;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = (x & y) != 0 ? Color.white : Color.gray;
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        Rect rect = new Rect(Vector2.zero, new Vector2(size, size));
        spriteRenderer.sprite = Sprite.Create(texture, rect, Vector2.zero);
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    // void OnGUI()
    // {
    //     float width = size;

    //     Debug.Log("x: " + drawingRenderer.transform.position.x + ", y: " + drawingRenderer.transform.position.y);
    //     GUI.DrawTexture(new Rect(drawingRenderer.transform.position.x, drawingRenderer.transform.position.y, width, width), texture);
    // }
}

