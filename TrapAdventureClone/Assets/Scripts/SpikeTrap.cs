using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private const string RAISE_PARAM = "Raise";
    private const string SpikeVisualName = "Hazard Spike Visual";
    private const string SpikeResourceName = "hazard_spike";

    private GameObject hazardVisual;
    private SpriteRenderer originalSpikeRenderer;

    private void Start()
    {
        SetupHazardVisual();
    }

    private void SetupHazardVisual()
    {
        /*
         * Find the actual damaging spike underneath
         * the animated SpikeTrapper parent.
         */
        Transform trapRoot = anim != null
            ? anim.transform
            : transform.parent;

        if (trapRoot == null)
        {
            return;
        }

        Transform newSpike = FindChildRecursive(
            trapRoot,
            "NewSpike"
        );

        if (newSpike == null)
        {
            return;
        }

        originalSpikeRenderer =
            newSpike.GetComponent<SpriteRenderer>();

        if (originalSpikeRenderer == null)
        {
            return;
        }

        Sprite hazardSprite =
            Resources.Load<Sprite>(
                SpikeResourceName
            );

        if (hazardSprite == null)
        {
            Debug.LogWarning(
                "SpikeTrap could not load hazard_spike from Resources."
            );

            return;
        }

        /*
         * Create or reuse a VISUAL-ONLY child.
         */
        Transform existing =
            newSpike.Find(SpikeVisualName);

        hazardVisual =
            existing != null
                ? existing.gameObject
                : new GameObject(
                    SpikeVisualName
                );

        hazardVisual.transform.SetParent(
            newSpike,
            false
        );

        hazardVisual.transform.localPosition =
            Vector3.zero;

        hazardVisual.transform.localRotation =
            Quaternion.identity;

        SpriteRenderer visualRenderer =
            hazardVisual.GetComponent<SpriteRenderer>();

        if (visualRenderer == null)
        {
            visualRenderer =
                hazardVisual.AddComponent<SpriteRenderer>();
        }

        visualRenderer.sprite =
            hazardSprite;

        visualRenderer.color =
            Color.white;

        visualRenderer.sortingLayerID =
            originalSpikeRenderer.sortingLayerID;

        visualRenderer.sortingOrder =
            originalSpikeRenderer.sortingOrder + 1;

        /*
         * Match the NEW artwork to the ORIGINAL spike's
         * world size WITHOUT scaling NewSpike itself.
         *
         * Therefore the collider is untouched.
         */
        Vector3 originalSize =
            originalSpikeRenderer.bounds.size;

        Vector2 newSpriteSize =
            hazardSprite.bounds.size;

        Vector3 parentScale =
            newSpike.lossyScale;

        hazardVisual.transform.localScale =
            new Vector3(
                SafeScale(
                    originalSize.x,
                    newSpriteSize.x,
                    parentScale.x
                ),
                SafeScale(
                    originalSize.y,
                    newSpriteSize.y,
                    parentScale.y
                ),
                1f
            );

        /*
         * Hide the original red triangle graphic,
         * but DON'T disable the SpriteRenderer/GameObject
         * because the trap mechanics remain on NewSpike.
         */
        Color originalColor =
            originalSpikeRenderer.color;

        originalSpikeRenderer.color =
            new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                0f
            );

        /*
         * Surprise mechanic:
         * new artwork starts completely hidden.
         */
        hazardVisual.SetActive(false);
    }

    private void OnTriggerEnter2D(
        Collider2D collision
    )
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioFeedback.PlayTrapReveal();

            /*
             * Reveal our visual immediately before
             * the EXISTING Raise animation begins.
             */
            if (hazardVisual != null)
            {
                hazardVisual.SetActive(true);
            }

            if (anim != null)
            {
                anim.SetTrigger(RAISE_PARAM);
            }
        }
    }

    private static Transform FindChildRecursive(
        Transform root,
        string targetName
    )
    {
        if (root.name == targetName)
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform result =
                FindChildRecursive(
                    child,
                    targetName
                );

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static float SafeScale(
        float targetSize,
        float spriteSize,
        float parentScale
    )
    {
        if (
            Mathf.Approximately(spriteSize, 0f)
            ||
            Mathf.Approximately(parentScale, 0f)
        )
        {
            return 1f;
        }

        return
            targetSize
            /
            (
                spriteSize
                *
                Mathf.Abs(parentScale)
            );
    }
}