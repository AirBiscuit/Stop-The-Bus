using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour
{

    public static LogicManager _Instance;
    public static LogicManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = (LogicManager)FindObjectOfType(typeof(LogicManager));
            }
            return _Instance;
        }
    }

    public Card[] deck = new Card[52];
    public Player[] players;
    public Player currentPlayer;
    public Sprite[] Clubs, Diamonds, Hearts, Spades, SuitIcons;
    public Sprite backSide;
    public float playerCircleRadius = 4;
    public bool isFreeLifeUsed = false, isBusStopped = false, isFirstTurnComplete = false;
    public GameObject cardPrefab, wasteCard;

    int turnIndex = 0;

    public void Start()
    {
        int dealer = Random.Range(0, players.Length);
        GetAllPlayers();
        PopulateDeck(true);
        DealAll(players.Length, dealer);
        AdjustPlayerPositions(playerCircleRadius);
        currentPlayer = players[dealer];
        DetermineHumanPlayer();
    }

    void GetAllPlayers()
    {
        //Initialise the player array by finding the number of players, then fill it with said players
        var totalPlayers = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[totalPlayers.Length];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = totalPlayers[i].GetComponent<Player>();
        }
        Debug.Log(players.Length + " players remain");
    }

    /// <summary>
    /// Fills the deck up with 52 cards, to be used at the start of the deal
    /// </summary>
    /// <param name="shuffle">Should the deck be shuffled before dealing?</param>
    void PopulateDeck(bool shuffle)
    {
        //NB: This HAS to be shuffled once used if not ran with the shuffle parameter

        //Add all of the Clubs in order to the deck
        for (int i = 1; i < 14; i++)
        {
            Card c = new Card(Suit.Club, i);
            deck[i - 1] = c;
        }
        for (int i = 1; i < 14; i++)
        {
            Card c = new Card(Suit.Diamond, i);
            deck[i + 12] = c;
        }
        for (int i = 1; i < 14; i++)
        {
            Card c = new Card(Suit.Heart, i);
            deck[i + 25] = c;
        }
        for (int i = 1; i < 14; i++)
        {
            Card c = new Card(Suit.Spade, i);
            deck[i + 38] = c;
        }
        Debug.Log("Deck populated, " + deck.Length + " cards added");
        if (shuffle)
        {
            ShuffleDeck();
        }
    }

    public void ShuffleDeck()
    {
        int n = deck.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            var value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
        Debug.Log("Deck shuffled");
    }

    public void DealAll(int NumPlayers, int DealerIndex)
    {
        Debug.Log("Dealing cards...");
        Stack s = new Stack();

        //Take everything from the deck array and put it into a stack
        for (int i = 0; i < deck.Length; i++)
        {
            s.Push(deck[i]);
        }

        float translateAmount = 0f;

        //Deal the cards out to the players
        for (int p = 0; p < NumPlayers; p++)
        {
            for (int i = 0; i < 3; i++)
            {
                players[p].cards[i] = (GameObject)Instantiate(cardPrefab, players[p].gameObject.transform);
                if (i > 0)
                    translateAmount = i / 5;
                else translateAmount = 0;
                players[p].cards[i].transform.Translate(i - 1, 0, translateAmount);

                players[p].cards[i].GetComponent<CardBehaviour>().card = (Card)s.Pop();
                players[p].cards[i].GetComponent<CardBehaviour>().PerformManualStart();
            }
        }
        Debug.Log("Main deal complete...");

        //Deal one more card to the dealer after all other players have gotten their 3 cards here

        var dealerLastCard = (GameObject)Instantiate(cardPrefab, players[DealerIndex].gameObject.transform);
        dealerLastCard.transform.Translate(2, 0, 1);
        dealerLastCard.GetComponent<CardBehaviour>().card = (Card)s.Pop();
        dealerLastCard.GetComponent<CardBehaviour>().PerformManualStart();

        var dealer = players[DealerIndex].GetComponent<Player>();
        dealer.hasFourCards = true;
        dealer.cards[3] = dealerLastCard;
        dealer.isDealer = true;
        dealer.isCurrentPlayer = true;
        currentPlayer = dealer;

        Debug.Log("Additional card dealt");
        foreach (Player p in players)
        {
            p.initialDealComplete = true;
            //p.CompleteDeal();

        }
        //TODO Return the remaining cards to the Card array version of deck
        deck = new Card[s.Count];
        for (int i = 0; i < s.Count; i++)
        {
            deck[i] = (Card)s.Pop();
        }
    }

    /// <summary>
    /// Spreads out the players evenly in a circle and intervals based on the number of players 
    /// </summary>
    /// <param name="Radius">The radius of the circle players will be spread out along</param>
    void AdjustPlayerPositions(float Radius)
    {

        //NOTE: When using Cos and Sin to get the values for X and Y, the circle starts from (1,0) i.e. leftmost position on the unit circle

        int numPositions = players.Length;
        float degreeInterval = 360 / numPositions;

        Debug.Log(string.Format("{0} players, {1} degrees between each, {2} radius", numPositions, degreeInterval, Radius));

        //foreach (Player p in players)
        //{
        //    p.gameObject.transform.Translate(new Vector2(Mathf.Cos(degreeInterval), Mathf.Sin(degreeInterval) * Radius));
        //}
        for (int i = 0; i < players.Length; i++)
        {
            float currentDegree = i * degreeInterval;
            float currentCos = Mathf.Cos(currentDegree * Mathf.Deg2Rad);
            float currentSin = Mathf.Sin(currentDegree * Mathf.Deg2Rad);

            Debug.Log(string.Format("New Position for player {0}: {1:f2},{2:f2}", i + 1, currentCos, currentSin));
            players[i].gameObject.transform.Translate(currentCos * Radius, currentSin * Radius, 0);
        }
    }

    public void UpdateWasteCard()
    {
        CardBehaviour wasteCardBehaviour = wasteCard.GetComponent<CardBehaviour>();
        //wasteCardBehaviour.PerformManualStart();
    }
    public void UpdateWasteCard(CardBehaviour card)
    {
        CardBehaviour wasteCardBehaviour = wasteCard.GetComponent<CardBehaviour>();
        wasteCardBehaviour = card;

        //Not needed as cards that are in the waste pile have all already been started at some point
        //wasteCardBehaviour.PerformManualStart();
    }

    public void BeginRound(int dealerPosition)
    {
        if (!CheckForWinner())
        {
            GetAllPlayers();
            PopulateDeck(true);
            DealAll(players.Length, dealerPosition);
            AdjustPlayerPositions(playerCircleRadius);
            currentPlayer = players[dealerPosition];
        }
    }

    public void ChangeTurn()
    {
        //End the previous player's turn
        currentPlayer.EndTurn();

        if (turnIndex < players.Length)
            turnIndex++;
        else
            turnIndex = 0;
        //Change the player
        currentPlayer = players[turnIndex];

        //Begin their turn
        currentPlayer.BeginTurn();
    }

    /// <summary>
    /// Reduce the player's lives by 1, eliminating them if necessary
    /// </summary>
    /// <param name="player">The player to subtract a life from</param>
    public void ReduceLife(Player player)
    {
        player.lives--;
        if (player.lives == 0 && isFreeLifeUsed)
        {
            EliminatePlayer(player);
        }
        else if (player.lives == 0 && !isFreeLifeUsed)
            isFreeLifeUsed = true;
    }

    /// <summary>
    /// Reduce multiple player's lives by 1, eliminating them if necessary
    /// </summary>
    /// <param name="playersLost">The list of players to subtract lives from</param>
    public void ReduceLife(Player[] playersLost)
    {
        foreach (Player player in playersLost)
        {
            player.lives--;
            if (player.lives == 0 && isFreeLifeUsed)
            {
                EliminatePlayer(player);
            }
            else if (player.lives == 0 && !isFreeLifeUsed)
                isFreeLifeUsed = true;
        }
    }

    public void EliminatePlayer(Player eliminatee)
    {
        Destroy(eliminatee.gameObject);
        //Begin a new round now
    }

    public bool CheckForWinner()
    {
        bool hasWon = false;
        int activePlayers = 0;
        foreach (Player p in players)
        {
            if (p.lives > 0)
                activePlayers++;
        }
        if (activePlayers > 1)
            hasWon = false;
        else hasWon = true;

        return hasWon;
    }

    public void DetermineHumanPlayer()
    {
        foreach (Player p in players)
        {
            if (p.playerName != "AI")
            {
                p.isHumanControlled = true;
            }
        }
    }

    public void HideAICards()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].isHumanControlled)
            {
                players[i].ObscureCards();
            }
        }
    }
    /*
     * List of things to do/events that happen in the game:
     * 
     * Each player begins with 3 lives.                                     DONE
     * 
     * The round begins, each player is dealt 3 cards, the dealer gets 4.   DONE
     * 
     * The Dealer discards a card, it goes to the waste pile                NOT DONE
     * 
     * The active player switches to the player on the dealer's left.       DONE
     * 
     * This player can pick a card from either the deck or the waste
     *  card pile, whichever is more advantageous to them.                  NOT DONE
     *  
     *  The player must then discard one of their cards.                    NOT DONE
     *  
     *  Once play reaches the dealer again, the bus may be stopped by any
     *  player while it is their turn and they have 3 cards in the same
     *  suit, totalling over 21 in value.                                   NOT DONE
     *  
     *  Once the bus is stopped, all players get one last turn.             NOT DONE
     *  
     *  When play reaches the dealer again, all hands are revealed in turn  NOT DONE
     *  
     *  If any player has a Gabby (31), all other players lose a life       NOT DONE
     *  
     *  If there are multiple Gabbies, nobody loses a life                  NOT DONE
     *  
     *  The player with the lowest total loses a life. If there are
     *  multiple people with the same lowest value, they all lose a life.   NOT DONE
     *  
     *  The next round begins, with the player to the left of the dealer
     *  acting as the dealer this time.                                     NOT DONE
     *  
     *  The first player to lose all 3 of their lives gets a freebie        DONE
     *  
     *  Rounds continue until only 1 person has any lives left.             NOT DONE
     */
}