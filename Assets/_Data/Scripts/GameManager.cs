using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("# Board size")]
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;

    [Header("# Prefabs")]
    [SerializeField] private Node nodePrefab;
    [SerializeField] private SpriteRenderer boardPrefabs;

    private void Start()
    {
        GenderateGrid();
    }

    private void GenderateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x,y),Quaternion.identity);
            }
        }

        Vector2 center = new Vector2(width / 2 - .5f, height / 2 - .5f);

        var board = Instantiate(boardPrefabs, center, Quaternion.identity);
        board.size = new Vector2(width, height);
    }
}
