using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelEffect : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        // �������� ��������� Particle System
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // ���������, �������� �� ������������� Particle System
        if (ps && !ps.IsAlive())
        {
            Destroy(gameObject); // ������� ������
        }
    }
}
