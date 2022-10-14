using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class AutoOutlineWidth : MonoBehaviour
{
    public Material MyMaterial;

    void Update()
    {
        if (!MyMaterial) { return; }

        UpdateShader();
    }

    private void UpdateShader()
    {
        float ScaledWidth = transform.localScale.magnitude *0.25f;
        if (ScaledWidth < 0) { ScaledWidth = -ScaledWidth; }

        MyMaterial.SetFloat("_Outline", ScaledWidth);
    }
}
