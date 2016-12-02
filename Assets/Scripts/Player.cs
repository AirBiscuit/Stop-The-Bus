﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string playerName;
    public AnimationCurve xCurve, yCurve;
    public int lives = 3;
    public GameObject cardPrefab;
    public CardBehaviour[] hand = new CardBehaviour[4];
    public bool hasFourCards = false, initialDealComplete = false, isDealer = false, isCurrentPlayer = false, isHumanControlled = false;
    public GameObject[] cards = new GameObject[4];
    [SerializeField]
    int handValue, highestSuitValue;
    bool firstUpdateComplete = false;

    [Header("Local UI Elements")]
    public Text highSuitCount;
    public Image suitImage;
    public Button btnStop;

    Suit highSuit = Suit.Club;

    void Start()
    {
        highSuitCount = GameObject.FindGameObjectWithTag("Value").GetComponent<Text>();
        suitImage = GameObject.FindGameObjectWithTag("SuitIcon").GetComponent<Image>();
        btnStop = GameObject.FindGameObjectWithTag("Finish").GetComponent<Button>();
    }

    void Update()
    {
        if (initialDealComplete)
        {
            if (!firstUpdateComplete)
            {
                CalculateHandValue();
                firstUpdateComplete = true;
                RefreshHand();
            }
        }
        if (isCurrentPlayer)
        {
            if (hasFourCards)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (hit != null)
                    {
                        Debug.Log("Hit " + hit.GetComponent<CardBehaviour>().card.currentValue + hit.GetComponent<CardBehaviour>().card.currentSuit);
                        LogicManager.Instance.UpdateWasteCard(hit.gameObject.GetComponent<CardBehaviour>());
                        Destroy(hit.gameObject);
                        hasFourCards = false;
                        LogicManager.Instance.ChangeTurn();
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (hit != null)
                    {
                        if (hit.gameObject.tag == "Waste" || hit.gameObject)
                        {

                        }
                        Debug.Log("Hit " + hit.GetComponent<CardBehaviour>().card.currentValue + hit.GetComponent<CardBehaviour>().card.currentSuit);
                        LogicManager.Instance.UpdateWasteCard(hit.gameObject.GetComponent<CardBehaviour>());
                        Destroy(hit.gameObject);
                        hasFourCards = false;
                        LogicManager.Instance.ChangeTurn();
                    }
                }
            }
        }
        //Pick up a card if you only have 3

        //Stop if you're allowed and want to

        //Discard a card

    }

    void GenerateHand()
    {
        if (isHumanControlled)
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
        else
        {
            if (hasFourCards)
            {
                for (int i = 0; i < 4; i++)
                {
                    var newCard = (GameObject)Instantiate(cardPrefab, this.transform);
                    hand[i].card = newCard.GetComponent<CardBehaviour>().card;
                    newCard.GetComponent<SpriteRenderer>().sprite = LogicManager.Instance.backSide;
                    cards[i] = newCard;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    var newCard = (GameObject)Instantiate(cardPrefab, this.transform);
                    hand[i].card = newCard.GetComponent<CardBehaviour>().card;
                    newCard.GetComponent<SpriteRenderer>().sprite = LogicManager.Instance.backSide;
                    cards[i] = newCard;
                }
            }
        }
    }

    void RefreshHand()
    {
        //If this player is being controlled by a human player, show the card faces
        if (isHumanControlled)
        {
            UpdateUI();
            //Do nothing here, Formerly: Update all the sprites in the hand
        }
        //Otherwise, obscure them from view
        else
            ObscureCards();

    }

    public void ObscureCards()
    {
        int iterations = 3;

        if (hasFourCards)
            iterations = 4;
        for (int i = 0; i < iterations; i++)
        {
            cards[i].GetComponent<SpriteRenderer>().sprite = LogicManager.Instance.backSide;
            cards[i].GetComponent<CardBehaviour>().card.isFaceUp = false;
        }
    }

    void CalculateHandValue()
    {
        int clubs = 0, diamonds = 0, spades = 0, hearts = 0, cardCount = 3;

        if (hasFourCards)
            cardCount = 4;

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
        {
            highestSuitValue = clubs;
            highSuit = Suit.Club;
        }
        if (highestSuitValue < diamonds)
        {
            highestSuitValue = diamonds;
            highSuit = Suit.Diamond;
        }
        if (highestSuitValue < hearts)
        {
            highestSuitValue = hearts;
            highSuit = Suit.Heart;
        }
        if (highestSuitValue < spades)
        {
            highestSuitValue = spades;
            highSuit = Suit.Spade;
        }
    }

    public void BeginTurn()
    {
        //ToDo: Enable the button to allow stopping the bus
        isCurrentPlayer = true;
        if (highestSuitValue >= 21)
        {

        }
    }

    public void EndTurn()
    {
        isCurrentPlayer = false;

        //ToDo: Disable the button to allow stopping the bus
    }

    public void EnableDiscardMode()
    {
        //---Make all the changes here that enable to the player to click on a card and remove it from their hand and send it to the waste pile---//

        //Disable picking up from the Deck/Waste

        //Enable each card as a clickable object, where clicking it will send it to the waste pile

        //Set a bool of isTurnComplete to true, end the turn

        hasFourCards = false;
    }

    public void UpdateUI()
    {
        if (isHumanControlled)
        {
            highSuitCount.text = highestSuitValue.ToString();
            switch (highSuit)
            {
                case Suit.Club:
                    suitImage.sprite = LogicManager.Instance.SuitIcons[0];
                    break;
                case Suit.Diamond:
                    suitImage.sprite = LogicManager.Instance.SuitIcons[1];
                    break;
                case Suit.Heart:
                    suitImage.sprite = LogicManager.Instance.SuitIcons[2];
                    break;
                case Suit.Spade:
                    suitImage.sprite = LogicManager.Instance.SuitIcons[3];
                    break;
                default:
                    break;
            }
        }

    }

    public void CheckStoppable()
    {
        bool stoppable = false;
        if (!LogicManager.Instance.isBusStopped && isHumanControlled && isCurrentPlayer)
        {
            
            if (highestSuitValue >= 21)
            {
                Suit s = hand[0].card.currentSuit;
                stoppable = true;
                for (int i = 0; i < 3; i++)
                {
                    if (hand[i].card.currentSuit != s)
                        stoppable = false;
                }
            }
        }
        btnStop.enabled = stoppable;
    }
}
