using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject tilePrefab;  // ������ ������ (� �����)
    private int rows = 1;           // ���������� �����
    private int columns = 3;        // ���������� ��������
    public float cellSize = 2.0f;  // ������ ������ �����
    public int levelsNum = 3;      // ����� �������
    public ImageSet[] imageSets;   // ������ ������� ��������
    public TMPro.TMP_Text findText;// ������ �� UI ����� ��� ����������� "Find"
    public GameObject winScreen;   // ����� ������

    [Header("Animation Settings")]
    public float spawnDelay = 0.1f;      // �������� ����� ������� ������
    public float bounceDuration = 0.5f;  // ������������ bounce-��������
    public float fadeDuration = 1.0f;    // ������������ fade-�������� ��� ������
    public GameObject victoryEffectPrefab; // ������ ������� ������

    private HashSet<Sprite> usedImages = new HashSet<Sprite>(); // ��� �������� �������������� ��������
    private ImageSet selectedSet;          // ��������� ����� ��������
    private string targetItem;             // ��, ��� �� ���� (����� ��� �����)
    private int currentLevel = 1;          // ������� �������

    private HashSet<string> usedItems = new HashSet<string>(); // ������ �������������� ����� ��������

    void Start()
    {
        StartLevel();
    }


    void StartLevel()
    {
        // ��������� ������� �������� ���� �� ������� ��������
        selectedSet = imageSets[Random.Range(0, imageSets.Length)];

        // ��������� ������� �������� � �����
        targetItem = GetRandomItem(selectedSet);
        ShowFindText(targetItem);  // ���������� ����� � ��������� ���������

        // ��������� ����� ����� ������� ������
        SpawnTiles();
    }

    void ClearOldTiles()
    {
        // ������� ��� ������� � ����� "Tile" � ������� ��
        GameObject[] oldTiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject oldTile in oldTiles)
        {
            Destroy(oldTile);
        }
    }

    void SpawnTiles()
    {
        Vector3 gridStartPosition = GetGridStartPosition();

        // ������� ���������� �������������� ����������� ����� ����� �������
        usedImages.Clear();

        // �������� � ����, ��� ������� �������� ��� ������
        Sprite targetSprite = GetSpriteFromItem(targetItem);
        usedImages.Add(targetSprite);

        // ������� ������ ��� ������ ��������� ��������
        List<Sprite> availableImages = new List<Sprite>();

        foreach (var sprite in selectedSet.images)
        {
            if (!usedImages.Contains(sprite))
            {
                availableImages.Add(sprite);
            }
        }

        // ��������� ����������� ���������� ����������� ��� �����
        while (availableImages.Count + 1 < rows * columns)
        {
            availableImages.AddRange(selectedSet.images);
        }

        // ��������� �����������
        List<Sprite> finalImages = new List<Sprite> { targetSprite };
        for (int i = availableImages.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (availableImages[i], availableImages[j]) = (availableImages[j], availableImages[i]);
        }
        finalImages.AddRange(availableImages.GetRange(0, rows * columns - 1));

        // ��������� ������ � �����
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = col * cellSize;
                float y = row * cellSize;

                Vector3 spawnPosition = gridStartPosition + new Vector3(x, y, 0);
                int index = row * columns + col;

                DOVirtual.DelayedCall(spawnDelay * index, () =>
                {
                    SpawnTileWithRandomImage(spawnPosition, finalImages[index]);
                });
            }
        }
    }

    Vector3 GetGridStartPosition()
    {
        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        screenCenter.z = 0;

        float offsetX = -(columns - 1) * cellSize / 2f;
        float offsetY = -(rows - 1) * cellSize / 2f;

        return new Vector3(screenCenter.x + offsetX, screenCenter.y + offsetY, 0);
    }

    void SpawnTileWithRandomImage(Vector3 position, Sprite image)
    {
        // ������� ����� ������ ������
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

        // �������� �������� ������, ������� �������� ������
        Transform child = tile.transform.GetChild(0); // �������� �������� ������ (�� ����)
        SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // ������������� ��������� �����������
            spriteRenderer.sprite = image;

            // ������������� ��������� ������ ������������� ������� ��� 0
            tile.transform.localScale = Vector3.zero;

            // ��������� �������� ���������� ������������� ������� (bounce ������)
            tile.transform.DOScale(Vector3.one, bounceDuration)
                .SetEase(Ease.OutBounce);

            // ��������� BoxCollider2D �� �������� ������ ��� ��������� ������
            BoxCollider2D collider = child.gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // ������������� � �������

            // ��������� ��������� ������ �� ��������� �������
            TileClick tileClick = child.gameObject.GetComponent<TileClick>();
            tileClick.Initialize(tile, image, this);
        }
    }

    Sprite GetSpriteFromItem(string item)
    {
        // ���� �������� � ����� ��������� � ������
        foreach (var sprite in selectedSet.images)
        {
            if (sprite.name == item)
            {
                return sprite;
            }
        }
        return null;
    }

    void ShowFindText(string item)
    {
        // ������������� ����� � ���, ��� ����� �����
        findText.text = $"Find {item}";

        // ��������� ������ fade (��������� ������)
        findText.DOFade(1f, fadeDuration).From(0f);
    }

    string GetRandomItem(ImageSet set)
    {
        List<Sprite> availableSprites = new List<Sprite>();

        // ��������� ������ �� �����������, ������� ��� �� ��������������
        foreach (var sprite in set.images)
        {
            if (!usedItems.Contains(sprite.name))
            {
                availableSprites.Add(sprite);
            }
        }

        // ���� ��� �������� �� ������ ������������ � �������� usedItems � �������� ������
        if (availableSprites.Count == 0)
        {
            usedItems.Clear();
            availableSprites.AddRange(set.images);
        }

        // �������� ��������� �����������
        Sprite randomImage = availableSprites[Random.Range(0, availableSprites.Count)];

        // ��������� ��������� �������� � ������ ��������������
        usedItems.Add(randomImage.name);

        return randomImage.name; // ���������� ��� ��������
    }

    // ����� ��� ��������� ������ �� ������
    public void OnTileClicked(GameObject clickedTile, Sprite clickedImage)
    {
        // ���� �������� ����������
        if (clickedImage.name == targetItem)
        {
            // �������� "easeInBounce" ��� ������
            clickedTile.transform.GetChild(0).DOPunchPosition(new Vector3(0.5f, 0, 0), bounceDuration, 10, 1f).SetEase(Ease.InBounce);

            // ��������� ������� ������
            Instantiate(victoryEffectPrefab, clickedTile.transform.position, Quaternion.identity);

            // ���� 2 �������, ����� �������� ����� �������
            DOVirtual.DelayedCall(2f, () =>
            {
                if (currentLevel < levelsNum)  // ���� �� ��������� �������
                {
                    currentLevel++;
                    if (columns > rows)
                        rows++;
                    else
                        columns++;
                    ClearOldTiles();  // ������� ������ ������ ����� ����� �������
                    StartLevel();     // ������ ������ ������
                }
                else
                {
                    // ��������� �������, ���������� ����� ������
                    winScreen.SetActive(true);
                }
            });
        }
        else
        {
            // �������� "easeInBounce" ��� ������������ ������
            clickedTile.transform.GetChild(0).DOPunchPosition(new Vector3(-0.5f, 0, 0), bounceDuration, 10, 1f).SetEase(Ease.InBounce);
        }
    }
}
