using UnityEngine;
using UnityEngine.Rendering;

public class PlayerArtHandler : MonoBehaviour
{
    [SerializeField]
    private SerializedDictionary<FaceDirection, PlayerDirectionalArt> arts = new();

    [SerializeField]
    private Sprite failSprite;

    [SerializeField]
    private SpriteRenderer spriteRenderer;


    public void SetSprite(FaceDirection direction, bool isEating)
    {
        GetArtAtDirection(direction, isEating, out Sprite sprite, out bool flipX);
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = flipX;
    }


    private void GetArtAtDirection(FaceDirection direction, bool isEating, out Sprite sprite, out bool flipX)
    {
        if (arts.TryGetValue(direction, out PlayerDirectionalArt art))
        {
            art.GetSprite(isEating, out sprite, out flipX);
            return;
        }

        flipX = false;
        sprite = failSprite;
    }
}