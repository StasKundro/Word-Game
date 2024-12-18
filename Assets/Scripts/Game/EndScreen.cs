using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public Image fadeImage; // ������ �� UI-������ � ����������� Image ��� ������� ����������
    public float fadeDuration = 1f; // ������������ ������� Fade
    private PlateSpawner spawner;

    private void Start()
    {
        spawner = GetComponent<PlateSpawner>();
        spawner.enabled = false; // ��������� ������� ��� ������
        FadeIn();
    }

    public void GoRestart()
    {
        // ��������� ������ ����������
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
                // �������� ������� ����� ���������� ������� FadeIn
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
