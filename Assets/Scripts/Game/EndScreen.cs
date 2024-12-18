using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public Image fadeImage; // Ссылка на UI-объект с компонентом Image для эффекта затемнения
    public float fadeDuration = 1f; // Длительность эффекта Fade
    private PlateSpawner spawner;

    private void Start()
    {
        spawner = GetComponent<PlateSpawner>();
        spawner.enabled = false; // Отключаем спавнер при старте
        FadeIn();
    }

    public void GoRestart()
    {
        // Запускаем эффект затемнения
        FadeOut(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    private void FadeIn()
    {
        fadeImage.DOFade(0, fadeDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Включаем спавнер после завершения эффекта FadeIn
                spawner.enabled = true;
            });
    }

    private void FadeOut(TweenCallback onComplete)
    {
        fadeImage.DOFade(1, fadeDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(onComplete);
    }
}
