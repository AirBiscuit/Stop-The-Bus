using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string playerName;
    public AnimationCurve xCurve, yCurve;
    public int lives = 3;
    public GameObject cardPrefab;
    public bool hasFourCards = false, initialDealComplete = false, isDealer = false, isCurrentPlayer = false, isHumanControlled = false, isStoppingPlayer = false, canPickNewCard = false;
    public GameObject[] cards = new GameObject[4];
    [SerializeField]
    int handValue, highestSuitValue;
    bool firstUpdateComplete = false;

    [Header("Local UI Elements")]
    public Text highSuitCount;
    public Image suitImage;
    public Button btnStop;

    Suit highSuit = Suit.Club;
    //TODO WHEN CHANGING TO NETWORKED GAME:
    //In Start(), set isHumanControlled to isLocalInstance or whatever the thing is on the network item that knows if the player is locally controller

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
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }
        }
        //Pick up a card if you only have 3

        //Stop if you're allowed and want to

        //Discard a card

    }

    void HandleInput()
    {
        var hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (hit != null)
        {
            if (hit.gameObject.tag == "Waste" || hit.gameObject.tag == "Deck")
            {
                if (canPickNewCard)
                {
                    if (hit.gameObject.tag == "Waste")
                    {
                        cards[3] = LogicManager.Instance.CollectWasteCard();
                        //Move the card to the very right and to the front of the other cards
                        cards[3].transform.Translate(2, 0, -3);
                    }
                    else
                    {
                        //TODO: Pick up from the deck
                    }
                }
            }
            else
            {
                Debug.Log("Hit " + hit.GetComponent<CardBehaviour>().card.currentValue + hit.GetComponent<CardBehaviour>().card.currentSuit);
                LogicManager.Instance.UpdateWasteCard(hit.gameObject.GetComponent<CardBehaviour>());
                Destroy(hit.gameObject);
                hasFourCards = false;
                LogicManager.Instance.ChangeTurn();
                Stack s = new Stack();
                var oldCards = cards;
                cards = new GameObject[4];
                foreach (GameObject card in oldCards)
                {
                    s.Push(card);
                }
                Vector3 pos;
                for (int i = 0; i < 3; i++)
                {
                    pos = new Vector3(i - 1, 0, - i);
                    cards[i].transform.localPosition = pos;
                }
            }
        }
    }


    void RefreshHand()
    {
        //If this player is being controlled by a human player, show the card faces
        if (isHumanControlled)
        {
            UpdateLocalUI();
            //Do nothing here, Formerly: Update all the sprites in the hand
        }
        //Otherwise, obscure them from view
        else
            ObscureCards();

    }
    public void UpdateLocalUI()
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
        isCurrentPlayer = true;
        canPickNewCard = !hasFourCards;
        CheckStoppable();
        Debug.Log("Beginning turn: " + playerName);
    }

    public void EndTurn()
    {
        isCurrentPlayer = false;
        SetHandClickable(false);
        DisableStopButton();
        if (isStoppingPlayer)
            LogicManager.Instance.StopTheBus();
        Debug.Log("Ending turn: " + playerName);
    }

    public void EnableDiscardMode()
    {
        //---Make all the changes here that enable to the player to click on a card and remove it from their hand and send it to the waste pile---//

        //Disable picking up from the Deck/Waste
        LogicManager.Instance.wasteCard.GetComponent<CardBehaviour>().isClickable = false;
        GameObject.FindGameObjectWithTag("Deck").GetComponent<CardBehaviour>().isClickable = false;

        //Enable each card as a clickable object, where clicking it will send it to the waste pile
        SetHandClickable(true);
        //Set a bool of isTurnComplete to true, end the turn
        EndTurn();

        hasFourCards = false;
    }

    public void CheckStoppable()
    {
        bool stoppable = false;
        if (!LogicManager.Instance.isBusStopped && isHumanControlled && isCurrentPlayer)
        {

            if (highestSuitValue >= 21)
            {
                Suit s = cards[0].GetComponent<CardBehaviour>().card.currentSuit;
                stoppable = true;
                for (int i = 0; i < 3; i++)
                {
                    if (cards[i].GetComponent<CardBehaviour>().card.currentSuit != s)
                        stoppable = false;
                }
            }
        }
        btnStop.interactable = stoppable;
    }

    void DisableStopButton()
    {
        btnStop.interactable = false;
    }

    void SetHandClickable(bool clickable)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].GetComponent<CardBehaviour>().isClickable = clickable;
        }
    }
}
