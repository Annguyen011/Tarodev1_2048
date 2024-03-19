using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("# Holder")]
    [SerializeField] private Transform blockHolder;
    [SerializeField] private Transform nodeHolder;
    [SerializeField] private Transform boardHolder;


    [Header("# Settings")]
    [SerializeField] private List<BlockType> types = new();
    [SerializeField] private GameState state;
    [SerializeField] private float travelTime = .4f;
    [SerializeField] private int round;

    [Header("# Board size")]
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;

    [Header("# Prefabs")]
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private SpriteRenderer boardPrefab;

    [Header("# Collections")]
    [SerializeField] private List<Node> nodes = new();
    [SerializeField] private List<Block> blocks = new();


    private void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    private void Update()
    {
        if (state != GameState.WaitingInput) return;

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Shift(Vector2.right);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Shift(Vector2.left);
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            Shift(Vector2.up);
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            Shift(Vector2.down);
        }

    }

    private void Shift(Vector2 dir)
    {
        ChangeState(GameState.Moving);

        var orderedBlock = blocks.OrderBy(b => b.pos.x).ThenBy(b => b.pos.y).ToList();

        if (dir == Vector2.right || dir == Vector2.up)
        {
            orderedBlock.Reverse();
        }

        foreach (var block in orderedBlock)
        {
            var next = block.node;

            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.pos + dir);

                if (possibleNode != null)
                {
                    if (possibleNode.occupiedBlock && possibleNode.occupiedBlock.CanMerge(block.value))
                    {
                        block.MergeBlock(possibleNode.occupiedBlock);

                    }else                    if (!possibleNode.occupiedBlock)
                    {
                        next = possibleNode;
                    }
                }

            } while (next != block.node);

        }

        var sequence = DOTween.Sequence();

        foreach(var block in orderedBlock)
        {
            var movePoint = block.mergingBlock ? block.mergingBlock.node.pos : block.node.pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, travelTime));
        }

        sequence.OnComplete(() =>
        {
            foreach (var block in orderedBlock.Where(b=>b.mergingBlock))
            {
                MergeBlock(block.mergingBlock,block);
            }

            ChangeState(GameState.SpawningBlock);
        });
    }

    void MergeBlock(Block baseBlock , Block mergingBlock)
    {
        SpawnBlock(baseBlock.node, baseBlock.value * 2);
    
        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block)
    {
        blocks.Remove(block);
        Destroy(block.gameObject);
    }
    

    private Node GetNodeAtPosition(Vector2 pos)
    {
        return nodes.FirstOrDefault(n => n.pos == pos);
    }

    private void GenderateGrid()
    {
        round = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity, nodeHolder);
                nodes.Add(node);
            }
        }

        Vector2 center = new Vector2(width / 2 - .5f, height / 2);

        var board = Instantiate(boardPrefab, center, Quaternion.identity, boardHolder);
        board.size = new Vector2(width, height);

        Vector3 camPos = Vector3.forward * -10f;
        camPos.x = center.x;
        camPos.y = center.y;

        Camera.main.transform.position = camPos;
        Camera.main.orthographicSize = 2.5f;

        ChangeState(GameState.SpawningBlock);
    }

    private void SpawnBlock(int amount)
    {
        // TO KNOW
        var freeNode = nodes.Where(n => n.occupiedBlock == null).OrderBy(b => UnityEngine.Random.value).ToList();

        foreach (var node in freeNode.Take(amount))
        {
            SpawnBlock(node, UnityEngine.Random.value > .8f? 4 : 2);
        }

        if (freeNode.Count() == 1)
        {
            // Lost a game

            return;
        }

        ChangeState(GameState.WaitingInput);
    }

    void SpawnBlock(Node node, int value)
    {
        var block = Instantiate(blockPrefab, node.pos, Quaternion.identity, blockHolder);
        block.Init(GetBlockTypeValue(value));
        block.SetBlock(node);
        blocks.Add(block);
    }
    private BlockType GetBlockTypeValue(int value) => types.First(t => t.value == value);

    private void ChangeState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.GenerateLevel:
                GenderateGrid();
                break;
            case GameState.SpawningBlock:
                SpawnBlock(round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }
    }

}
[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlock,
    WaitingInput,
    Moving,
    Win,
    Lose
}
