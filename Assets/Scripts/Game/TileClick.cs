using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClick : MonoBehaviour
{
    private GameObject tile;
    private Sprite tileImage;
    private PlateSpawner plateSpawner;

    public void Initialize(GameObject _tile, Sprite _image, PlateSpawner _plateSpawner)
    {
        tile = _tile;
        tileImage = _image;
        plateSpawner = _plateSpawner;
    }

    private void OnMouseDown()
    {
        // Вызов метода проверки клика по плитке
        plateSpawner.OnTileClicked(tile, tileImage);
    }
}
