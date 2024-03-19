using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int value;

    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private TextMeshPro text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init(BlockType type)
    {
        value = type.value;
        text.text = type.value.ToString();
        renderer.color = type.color;
    }
}
