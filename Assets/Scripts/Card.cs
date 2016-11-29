using UnityEngine;
using System.Collections;

public enum Suit
{
    Club,
    Diamond,
    Heart,
    Spade
}
public enum Value
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

[System.Serializable]
public class Card
{
    
    public Value currentValue;
    public Suit currentSuit;
    public bool isFaceUp = true;
    public int value;

    public Card()
    {

    }
    /// <summary>
    /// Create a card
    /// </summary>
    /// <param name="initialSuit">The suit of the card</param>
    /// <param name="initialValue">The value of the card</param>
    public Card(Suit initialSuit, Value initialValue)
    {
        currentValue = initialValue;
        currentSuit = initialSuit;
    }
    /// <summary>
    /// Create a card with a numerical value rather than a enumarated value
    /// </summary>
    /// <param name="initialSuit">The suit of the card</param>
    /// <param name="initialValue">The integer value of the card (Ace is 1, King is 13)</param>
    public Card(Suit initialSuit, int initialValue)
    {
        currentSuit = initialSuit;
        currentValue = (Value)initialValue;
    }


}
