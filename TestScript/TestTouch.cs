using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class TestTouch : MonoBehaviour
{
    public string text;

    public void GetStringDebug(Vector2 vt2)
    {
        text = vt2.ToString();
    }
}
