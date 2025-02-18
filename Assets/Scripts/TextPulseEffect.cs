using UnityEngine;
using TMPro;

public class TextPulseEffect : MonoBehaviour
{
    public TMP_Text textComponent;
    public float pulseSpeed = 1.5f;
    public Color pulseColor = Color.white;

    private TMP_TextInfo textInfo;
    private Color[] newVertexColors;

    void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        textInfo = textComponent.textInfo;
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();
        int characterCount = textInfo.characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            var newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1.0f) / 2.0f;
            Color32 c = Color.Lerp(textComponent.color, pulseColor, alpha);

            newVertexColors[vertexIndex + 0] = c;
            newVertexColors[vertexIndex + 1] = c;
            newVertexColors[vertexIndex + 2] = c;
            newVertexColors[vertexIndex + 3] = c;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
