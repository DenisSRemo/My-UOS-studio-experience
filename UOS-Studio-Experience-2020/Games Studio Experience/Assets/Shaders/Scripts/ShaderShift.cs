using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ShaderShift : MonoBehaviour
{
    public string SlotName = "_EmissionTex";
    public Material MyMaterial;
    private Vector2 CurrentOffset;
    public Vector2 MoveSpeed = new Vector2(0.1f,1.5f);
    void Update()
    {
        if (!MyMaterial) { return; }

        UpdateShader();
    }

    private void UpdateShader()
    {
        if (!Application.isPlaying)
        {
            CurrentOffset += MoveSpeed/60;
        }
        else
        {
            CurrentOffset += MoveSpeed * Time.deltaTime;
        }        

        MyMaterial.SetTextureOffset(SlotName, CurrentOffset);

        if (CurrentOffset.x >= 1f) { CurrentOffset.x = 0; }
        else if (CurrentOffset.x < 0f) { CurrentOffset.x = 1; }

        if (CurrentOffset.y >= 1f) { CurrentOffset.y = 0; }
        else if (CurrentOffset.y < 0f) { CurrentOffset.y = 1; }

    }
}
