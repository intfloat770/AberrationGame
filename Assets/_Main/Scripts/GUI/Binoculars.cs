using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Binoculars : MonoBehaviour
{
    [SerializeField] Transform referenceTransform;
    [SerializeField] int intervals;
    [SerializeField] float spacing;

    [SerializeField] GameObject textPrefab;

    TextMeshProUGUI[] textMeshes;

    private void Awake()
    {
        textMeshes = new TextMeshProUGUI[intervals * 2];

        for(int i = 0; i < intervals; i++)
        {
            GameObject obj = Instantiate(textPrefab, transform);
            textMeshes[i] = obj.GetComponent<TextMeshProUGUI>();

            float degree = 360 / (i + 1);

            textMeshes[i].text = degree.ToString();
        }
    }

    private void Update()
    {
        Vector2 origin = new Vector2(referenceTransform.eulerAngles.y / 360f * Screen.width, 0);
        for(int i = 0; i < intervals; i++)
        {
            textMeshes[i].rectTransform.anchoredPosition = new Vector2(origin.x + i * spacing, 0);
        }
    }
}
