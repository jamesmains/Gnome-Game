using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {
    public Sprite targetCursorSprite;
    private Texture2D refinedTex;

    void Awake() {
        TexturizeSprites();
    }

    void TexturizeSprites() {
        refinedTex = new Texture2D((int) targetCursorSprite.rect.width, (int) targetCursorSprite.rect.height);
        var pixels = targetCursorSprite.texture.GetPixels((int) targetCursorSprite.textureRect.x,
            (int) targetCursorSprite.textureRect.y,
            (int) targetCursorSprite.textureRect.width,
            (int) targetCursorSprite.textureRect.height);
        refinedTex.SetPixels(pixels);
        refinedTex.filterMode = FilterMode.Point;
        refinedTex.Apply();
        Cursor.SetCursor(refinedTex,Vector2.zero,CursorMode.Auto);
    }
}
