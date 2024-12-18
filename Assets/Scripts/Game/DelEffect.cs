using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelEffect : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        // Получаем компонент Particle System
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Проверяем, закончил ли проигрываться Particle System
        if (ps && !ps.IsAlive())
        {
            Destroy(gameObject); // Удаляем объект
        }
    }
}
