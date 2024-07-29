using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    
    public Row[] rows;
    public Tile[,] Tiles { get; private set; }
    
    public TextMeshProUGUI resetCountText;

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.25f;
    
    private int _resetCount = 0; 
    private const int MaxResets = 2;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
        UpdateResetCountText();
    }

    public void Init()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Height; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                
                Tiles[x, y] = tile;
            }
        }
    }
    
    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours,tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                _selection.Add(tile);
            }
        }
        
        if (_selection.Count < 2) return;
        
        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
        }
        
        _selection.Clear();
    }

    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play().AsyncWaitForCompletion();
        
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;
    }

    private bool CanPop()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var connectedTiles = Tiles[x, y].GetConnectedTiles();

                // 3개가 만나면 가로 또는 세로 일직선이어야 함
                if (connectedTiles.Count == 3)
                {
                    bool isHorizontal = connectedTiles.All(tile => tile.y == y);
                    bool isVertical = connectedTiles.All(tile => tile.x == x);

                    if (isHorizontal || isVertical)
                    {
                        return true;
                    }
                }
                
                // 4개 이상이면 일직선이 아니어도 됨
                else if (connectedTiles.Count > 3)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async void Pop()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();
                
                if (connectedTiles.Count == 3)
                {
                    bool isHorizontal = connectedTiles.All(t => t.y == y);
                    bool isVertical = connectedTiles.All(t => t.x == x);

                    if (!isHorizontal && !isVertical) continue;
                }
                // 4개 이상이면 일직선이 아니어도 됨
                else if (connectedTiles.Count < 4)
                {
                    continue;
                }

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                }
                
                SoundManager.instance.PlaySFX(0);
                UIManager.Instance.Score += tile.Item.value * connectedTiles.Count;
                
                await deflateSequence.Play().AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                    inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }
                
                await inflateSequence.Play().AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }
    }
    
    public void ResetTiles()
    {
        if (_resetCount < MaxResets)
        {
            _resetCount++;
            Init();
            UpdateResetCountText();
        }
    }
    
    private void UpdateResetCountText()
    {
        resetCountText.text = $"{MaxResets - _resetCount}";
    }
}