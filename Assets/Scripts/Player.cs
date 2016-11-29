using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string playerName;
    public int lives = 3;
    public GameObject cardPrefab;
    public CardBehaviour[] hand = new CardBehaviour[4];
    public bool hasFourCards = false, initialDealComplete = false, isDealer = false;
    public GameObject[] cards = new GameObject[4];
    [SerializeField]
    int handValue, highestSuitValue;
    bool firstUpdateComplete = false;

    void Start()
    {
    }

    void Update()
    {
        if (initialDealComplete && !firstUpdateComplete)
        {
            //CompleteDeal();
            CalculateHandValue();
            firstUpdateComplete = true;
        }
    }

    void GenerateHand()
    {
        if (hasFourCards)
        {
            for (int i = 0; i < 4; i++)
            {
                var newCard = (GameObject)Instantiate(cardPrefab, this.transform);
                hand[i].card = newCard.GetComponent<CardBehaviour>().card;
                newCard.GetComponent<SpriteRenderer>().sprite = newCard.GetComponent<CardBehaviour>().GetSprite();
                cards[i] = newCard;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                var newCard = (GameObject)Instantiate(cardPrefab, this.transform);
                hand[i].card = newCard.GetComponent<CardBehaviour>().card;
                newCard.GetComponent<SpriteRenderer>().sprite = hand[i].GetSprite();
                cards[i] = newCard;
            }
        }
    }

    void RefreshHand()
    {
        if (hasFourCards)
        {
            for (int i = 0; i < 4; i++)
            {
                cards[i].GetComponent<SpriteRenderer>().sprite = hand[i].GetSprite();
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                cards[i].GetComponent<SpriteRenderer>().sprite = hand[i].GetSprite();
            }
        }

    }

    void CalculateHandValue()
    {
        int clubs = 0, diamonds = 0, spades = 0, hearts = 0, cardCount = 3;

        if (hasFourCards)
        {
            cardCount = 4;
        }

        for (int i = 0; i < cardCount; i++)
        {
            int value = cards[i].GetComponent<CardBehaviour>().card.value;
            handValue += value;

            switch (cards[i].GetComponent<CardBehaviour>().card.currentSuit)
            {
                case Suit.Club:
                    clubs += value;
                    break;
                case Suit.Diamond:
                    diamonds += value;
                    break;
                case Suit.Heart:
                    hearts += value;
                    break;
                case Suit.Spade:
                    spades += value;
                    break;
                default:
                    break;
            }
        }
        if (highestSuitValue < clubs)
            highestSuitValue = clubs;
        if (highestSuitValue < diamonds)
            highestSuitValue = diamonds;
        if (highestSuitValue < hearts)
            highestSuitValue = hearts;
        if (highestSuitValue < spades)
            highestSuitValue = spades;

    }

    public void CompleteDeal()
    {
        int i = 0;
        foreach (GameObject g in cards)
        {
            hand[i] = g.GetComponent<CardBehaviour>();
            i++;
        }
    }
}
