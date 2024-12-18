using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImageSet", menuName = "ScriptableObjects/ImageSet", order = 1)]
public class ImageSet : ScriptableObject
{
    public Sprite[] images;  // Массив картинок (например, цифры, буквы)
}
