using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private const string RuntimeLifeSpriteResource = "life_indicator";

    public Image heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private List<Image> hearts = new List<Image>();

    private void Awake()
    {
        Sprite runtimeLifeSprite = LoadRuntimeLifeSprite();
        if (runtimeLifeSprite != null)
        {
            fullHeartSprite = runtimeLifeSprite;
            emptyHeartSprite = runtimeLifeSprite;
        }
    }

    public void SetMaxHearts(int maxHearts)
    {
        foreach(Image heart in hearts)
        {
            Destroy(heart.gameObject);
        }

        hearts.Clear();

        for(int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, transform);
            newHeart.sprite = fullHeartSprite;
            newHeart.color = Color.white;
            newHeart.preserveAspect = true;
            if (newHeart.rectTransform != null)
            {
                newHeart.rectTransform.sizeDelta = new Vector2(56f, 56f);
            }
            hearts.Add(newHeart);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for(int i = 0; i  < hearts.Count ; i++)
        {
            if(i < currentHealth)
            {
                hearts[i].sprite = fullHeartSprite;
                hearts[i].color = Color.white;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
                hearts[i].color = new Color(1f, 1f, 1f, 0.25f);
            }
        }
    }

    private static Sprite LoadRuntimeLifeSprite()
    {
        Sprite importedSprite = Resources.Load<Sprite>(RuntimeLifeSpriteResource);
        if (importedSprite != null)
        {
            return importedSprite;
        }

        Texture2D texture = Resources.Load<Texture2D>(RuntimeLifeSpriteResource);
        if (texture == null)
        {
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }
}
