using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject tilePrefab;  // Префаб плитки (с фоном)
    private int rows = 1;           // Количество строк
    private int columns = 3;        // Количество столбцов
    public float cellSize = 2.0f;  // Размер клетки сетки
    public int levelsNum = 3;      // Число уровней
    public ImageSet[] imageSets;   // Массив наборов картинок
    public TMPro.TMP_Text findText;// Ссылка на UI текст для отображения "Find"
    public GameObject winScreen;   // Экран победы

    [Header("Animation Settings")]
    public float spawnDelay = 0.1f;      // Задержка между спавном плиток
    public float bounceDuration = 0.5f;  // Длительность bounce-анимации
    public float fadeDuration = 1.0f;    // Длительность fade-анимации для текста
    public GameObject victoryEffectPrefab; // Префаб эффекта победы

    private HashSet<Sprite> usedImages = new HashSet<Sprite>(); // Для хранения использованных картинок
    private ImageSet selectedSet;          // Выбранный набор картинок
    private string targetItem;             // То, что мы ищем (буква или цифра)
    private int currentLevel = 1;          // Текущий уровень

    private HashSet<string> usedItems = new HashSet<string>(); // Хранит использованные имена картинок

    void Start()
    {
        StartLevel();
    }


    void StartLevel()
    {
        // Случайным образом выбираем один из наборов картинок
        selectedSet = imageSets[Random.Range(0, imageSets.Length)];

        // Обновляем целевую картинку и текст
        targetItem = GetRandomItem(selectedSet);
        ShowFindText(targetItem);  // Показываем текст с поисковым элементом

        // Запускаем спавн новой таблицы плиток
        SpawnTiles();
    }

    void ClearOldTiles()
    {
        // Находим все объекты с тегом "Tile" и удаляем их
        GameObject[] oldTiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject oldTile in oldTiles)
        {
            Destroy(oldTile);
        }
    }

    void SpawnTiles()
    {
        Vector3 gridStartPosition = GetGridStartPosition();

        // Очищаем предыдущие использованные изображения перед новым уровнем
        usedImages.Clear();

        // Начинаем с того, что выберем картинку для поиска
        Sprite targetSprite = GetSpriteFromItem(targetItem);
        usedImages.Add(targetSprite);

        // Создаем список для других случайных картинок
        List<Sprite> availableImages = new List<Sprite>();

        foreach (var sprite in selectedSet.images)
        {
            if (!usedImages.Contains(sprite))
            {
                availableImages.Add(sprite);
            }
        }

        // Добавляем достаточное количество изображений для сетки
        while (availableImages.Count + 1 < rows * columns)
        {
            availableImages.AddRange(selectedSet.images);
        }

        // Смешиваем изображения
        List<Sprite> finalImages = new List<Sprite> { targetSprite };
        for (int i = availableImages.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (availableImages[i], availableImages[j]) = (availableImages[j], availableImages[i]);
        }
        finalImages.AddRange(availableImages.GetRange(0, rows * columns - 1));

        // Размещаем плитки в сетке
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
        // Спавним новый объект плитки
        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

        // Получаем дочерний объект, который содержит спрайт
        Transform child = tile.transform.GetChild(0); // Получаем дочерний объект (он один)
        SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Устанавливаем случайное изображение
            spriteRenderer.sprite = image;

            // Устанавливаем начальный размер родительского объекта как 0
            tile.transform.localScale = Vector3.zero;

            // Применяем анимацию увеличения родительского объекта (bounce эффект)
            tile.transform.DOScale(Vector3.one, bounceDuration)
                .SetEase(Ease.OutBounce);

            // Добавляем BoxCollider2D на дочерний объект для обработки кликов
            BoxCollider2D collider = child.gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Устанавливаем в триггер

            // Добавляем обработку кликов по дочернему объекту
            TileClick tileClick = child.gameObject.GetComponent<TileClick>();
            tileClick.Initialize(tile, image, this);
        }
    }

    Sprite GetSpriteFromItem(string item)
    {
        // Ищем картинку с таким названием в наборе
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
        // Устанавливаем текст с тем, что нужно найти
        findText.text = $"Find {item}";

        // Применяем эффект fade (появление текста)
        findText.DOFade(1f, fadeDuration).From(0f);
    }

    string GetRandomItem(ImageSet set)
    {
        List<Sprite> availableSprites = new List<Sprite>();

        // Добавляем только те изображения, которые еще не использовались
        foreach (var sprite in set.images)
        {
            if (!usedItems.Contains(sprite.name))
            {
                availableSprites.Add(sprite);
            }
        }

        // Если все картинки из набора использованы — обнуляем usedItems и начинаем заново
        if (availableSprites.Count == 0)
        {
            usedItems.Clear();
            availableSprites.AddRange(set.images);
        }

        // Выбираем случайное изображение
        Sprite randomImage = availableSprites[Random.Range(0, availableSprites.Count)];

        // Добавляем выбранную картинку в список использованных
        usedItems.Add(randomImage.name);

        return randomImage.name; // Возвращаем имя картинки
    }

    // Метод для обработки кликов по плитке
    public void OnTileClicked(GameObject clickedTile, Sprite clickedImage)
    {
        // Если картинка правильная
        if (clickedImage.name == targetItem)
        {
            // Анимация "easeInBounce" при победе
            clickedTile.transform.GetChild(0).DOPunchPosition(new Vector3(0.5f, 0, 0), bounceDuration, 10, 1f).SetEase(Ease.InBounce);

            // Появление эффекта победы
            Instantiate(victoryEffectPrefab, clickedTile.transform.position, Quaternion.identity);

            // Ждем 2 секунды, затем начинаем новый уровень
            DOVirtual.DelayedCall(2f, () =>
            {
                if (currentLevel < levelsNum)  // Если не последний уровень
                {
                    currentLevel++;
                    if (columns > rows)
                        rows++;
                    else
                        columns++;
                    ClearOldTiles();  // Удаляем старые плитки перед новым уровнем
                    StartLevel();     // Запуск нового уровня
                }
                else
                {
                    // Последний уровень, показываем экран победы
                    winScreen.SetActive(true);
                }
            });
        }
        else
        {
            // Анимация "easeInBounce" при неправильном выборе
            clickedTile.transform.GetChild(0).DOPunchPosition(new Vector3(-0.5f, 0, 0), bounceDuration, 10, 1f).SetEase(Ease.InBounce);
        }
    }
}
