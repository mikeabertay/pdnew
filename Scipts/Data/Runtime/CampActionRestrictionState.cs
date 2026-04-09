using System.Collections.Generic;

public class CampActionRestrictionState
{
    public Camp targetCamp;

    public HashSet<int> allowedPieceRuntimeIds = new();
    public HashSet<int> forbiddenPieceRuntimeIds = new();

    public HashSet<int> allowedCardRuntimeIds = new();
    public HashSet<int> forbiddenCardRuntimeIds = new();

    public bool disableAllPieces;
    public bool disableAllCards;

    public HashSet<string> forbiddenPieceTags = new();
    public HashSet<string> forbiddenCardTags = new();

    public void Clear()
    {
        allowedPieceRuntimeIds.Clear();
        forbiddenPieceRuntimeIds.Clear();
        allowedCardRuntimeIds.Clear();
        forbiddenCardRuntimeIds.Clear();
        forbiddenPieceTags.Clear();
        forbiddenCardTags.Clear();
        disableAllPieces = false;
        disableAllCards = false;
    }
}