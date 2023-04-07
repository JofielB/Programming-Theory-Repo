using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : Shape
{
    protected override void SetParameters()
    {
        m_Speed = 5;
        m_Force = 1.5f;
        m_Health = 4;
    }
}
