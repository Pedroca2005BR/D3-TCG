using System.Collections.Generic;

[System.Serializable]
public class DeckDTO
{
    public string id;
    public string name;
    public List<string> cardKeys; // agora chamamos de keys (addressable keys)
    public int version = 1;
}
