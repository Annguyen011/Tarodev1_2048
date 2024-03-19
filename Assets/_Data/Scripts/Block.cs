using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int value;
    public Node node;
    public Block mergingBlock;
    public bool merging;
    public Vector2 pos => transform.position;

    [SerializeField] private SpriteRenderer ren;
    [SerializeField] private TextMeshPro text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        ren = GetComponentInChildren<SpriteRenderer>();
    }

    public void Init(BlockType type)
    {
        ren.color = type.color;
        value = type.value;
        text.text = type.value.ToString();
    }

    public void SetBlock(Node Node)
    {
        if (node != null)
        {
            node.occupiedBlock = null;
        }

        this.node = Node;
        node.occupiedBlock = this;
    }

    public void MergeBlock(Block Block)
    {
        mergingBlock = Block;

        node.occupiedBlock = null;
        Block.merging = true;
    }

    public bool CanMerge(int value) => value == this.value && !merging && !mergingBlock;
}
