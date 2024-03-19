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
            var block = Instantiate(blockPrefab, node.pos, Quaternion.identity, blockHolder);
            block.Init(GetBlockTypeValue(UnityEngine.Random.value > .8f ? 4 : 2));
        }

        if (freeNode.Count() == 1)
        {
            // Lost a game

            return;
        }
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
