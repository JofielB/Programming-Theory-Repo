using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Shape
{
    protected override void SetParameters()
    {
        m_Speed = 2;
        m_Force = 3;
        m_Health = 4;
    }
}
