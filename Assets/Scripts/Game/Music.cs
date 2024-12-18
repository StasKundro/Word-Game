using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private static Music instance;

    private void Awake()
    {
        if (instance == null)
        {
            // Если экземпляр не существует, устанавливаем этот объект как текущий экземпляр
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Если экземпляр уже существует, уничтожаем этот объект, чтобы избежать дубликатов
            Destroy(gameObject);
        }
    }
}
