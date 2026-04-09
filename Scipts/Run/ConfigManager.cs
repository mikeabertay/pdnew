using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    private readonly Dictionary<string, CardConfig> _cardDict = new();
    private readonly Dictionary<string, PieceConfig> _pieceDict = new();

    private void Awake()
    {
        Instance = this;
        LoadAllConfigs();
    }

    private void LoadAllConfigs()
    {
        // 这里替换成你自己的 JSON 加载逻辑
        // 例如从 StreamingAssets / Addressables / 本地工具导出的目录中读取
    }

    public void RegisterCards(List<CardConfig> cards)
    {
        _cardDict.Clear();
        foreach (var c in cards)
        {
            _cardDict[c.cardId] = c;
        }
    }

    public void RegisterPieces(List<PieceConfig> pieces)
    {
        _pieceDict.Clear();
        foreach (var p in pieces)
        {
            _pieceDict[p.pieceId] = p;
        }
    }

    public CardConfig GetCard(string cardId)
    {
        _cardDict.TryGetValue(cardId, out var config);
        return config;
    }

    public PieceConfig GetPiece(string pieceId)
    {
        _pieceDict.TryGetValue(pieceId, out var config);
        return config;
    }
}