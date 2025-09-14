using UnityEngine;

[System.Serializable]
public struct PlayerDirectionalArt
{
    [SerializeField]
    private Sprite eatSprite;

    [SerializeField]
    private Sprite regurgitateSprite;

    [SerializeField]
    private bool flipX;

    public void GetSprite(bool isEat, out Sprite sprite, out bool flipX)
    {
        flipX = this.flipX;
        sprite = isEat ? this.eatSprite : this.regurgitateSprite;
    }
}