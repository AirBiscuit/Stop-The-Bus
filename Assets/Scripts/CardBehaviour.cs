using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{

    [SerializeField]
    public Card card;
    public bool isCardInitialised = false, isSpriteSelected = false;
    
    // Use this for initialization
    void Start()
    {
        //Automatic start can be used here if the card has the right tag.
        if (this.gameObject.tag == "Deck")
        {
            SelectSprite();
        }
    }

    public void PerformManualStart()
    {
        SelectSprite();
        CalculateActualValue();
        isCardInitialised = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCardInitialised && !isSpriteSelected)
        {
            isSpriteSelected = true;
            SelectSprite();
        }
    }
    public void SelectSprite()
    {
        #region Face showing stuff

        if (card.isFaceUp)
        {
            Sprite[] selectedSuit = new Sprite[13];
            int index = 0;

            switch (card.currentSuit)
            {
                case Suit.Club:
                    selectedSuit = LogicManager.Instance.Clubs;
                    break;
                case Suit.Diamond:
                    selectedSuit = LogicManager.Instance.Diamonds;
                    break;
                case Suit.Heart:
                    selectedSuit = LogicManager.Instance.Hearts;
                    break;
                case Suit.Spade:
                    selectedSuit = LogicManager.Instance.Spades;
                    break;
                default:
                    break;
            }
            switch (card.currentValue)
            {
                case Value.Ace:
                    index = 0;
                    break;
                case Value.Two:
                    index = 1;
                    break;
                case Value.Three:
                    index = 2;
                    break;
                case Value.Four:
                    index = 3;
                    break;
                case Value.Five:
                    index = 4;
                    break;
                case Value.Six:
                    index = 5;
                    break;
                case Value.Seven:
                    index = 6;
                    break;
                case Value.Eight:
                    index = 7;
                    break;
                case Value.Nine:
                    index = 8;
                    break;
                case Value.Ten:
                    index = 9;
                    break;
                case Value.Jack:
                    index = 10;
                    break;
                case Value.Queen:
                    index = 11;
                    break;
                case Value.King:
                    index = 12;
                    break;
                default:
                    break;
            }
            this.GetComponent<SpriteRenderer>().sprite = selectedSuit[index];
        }
        #endregion
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = LogicManager.Instance.backSide;
        }
    }
    public Sprite GetSprite()
    {
        Sprite currentSprite = LogicManager.Instance.Clubs[0];
        if (card.isFaceUp)
        {
            Sprite[] selectedSuit = new Sprite[13];
            int index = 0;

            switch (card.currentSuit)
            {
                case Suit.Club:
                    selectedSuit = LogicManager.Instance.Clubs;
                    break;
                case Suit.Diamond:
                    selectedSuit = LogicManager.Instance.Diamonds;
                    break;
                case Suit.Heart:
                    selectedSuit = LogicManager.Instance.Hearts;
                    break;
                case Suit.Spade:
                    selectedSuit = LogicManager.Instance.Spades;
                    break;
                default:
                    break;
            }
            switch (card.currentValue)
            {
                case Value.Ace:
                    index = 0;
                    break;
                case Value.Two:
                    index = 1;
                    break;
                case Value.Three:
                    index = 2;
                    break;
                case Value.Four:
                    index = 3;
                    break;
                case Value.Five:
                    index = 4;
                    break;
                case Value.Six:
                    index = 5;
                    break;
                case Value.Seven:
                    index = 6;
                    break;
                case Value.Eight:
                    index = 7;
                    break;
                case Value.Nine:
                    index = 8;
                    break;
                case Value.Ten:
                    index = 9;
                    break;
                case Value.Jack:
                    index = 10;
                    break;
                case Value.Queen:
                    index = 11;
                    break;
                case Value.King:
                    index = 12;
                    break;
                default:
                    break;
            }
            currentSprite = selectedSuit[index];
        }
        else currentSprite = LogicManager.Instance.backSide;
        return currentSprite;
    }

    public void CalculateActualValue()
    {
        switch (card.currentValue)
        {
            case Value.Ace:
                card.value = 11;
                break;
            case Value.Two:
                card.value = 2;
                break;
            case Value.Three:
                card.value = 3;
                break;
            case Value.Four:
                card.value = 4;
                break;
            case Value.Five:
                card.value = 5;
                break;
            case Value.Six:
                card.value = 6;
                break;
            case Value.Seven:
                card.value = 7;
                break;
            case Value.Eight:
                card.value = 8;
                break;
            case Value.Nine:
                card.value = 9;
                break;
            case Value.Ten:
                card.value = 10;
                break;
            case Value.Jack:
                card.value = 10;
                break;
            case Value.Queen:
                card.value = 10;
                break;
            case Value.King:
                card.value = 10;
                break;
            default:
                break;
        }
    }

}