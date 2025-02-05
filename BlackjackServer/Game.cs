using System.Text;

namespace BlackjackServer;

public class Card
{
    public string Suit { get; set; }
    public string Rank { get; set; }

    public Card(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}

public class Deck
{
    private List<Card> _cards;
    private Random _random;

    public Deck()
    {
        _cards = new List<Card>();
        _random = new Random();
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                _cards.Add(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            Card temp = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = temp;
        }
    }

    public Card DrawCard()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }

        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }
}

public class Player
{
    public List<Card> Hand { get; set; }

    public Player()
    {
        Hand = new List<Card>();
    }

    public void AddCard(Card card)
    {
        Hand.Add(card);
    }

    public int CalculateHandValue()
    {
        int value = 0;
        int aces = 0;

        foreach (var card in Hand)
        {
            if (card.Rank == "Ace")
            {
                aces++;
                value += 11;
            }
            else if (card.Rank == "Jack" || card.Rank == "Queen" || card.Rank == "King")
            {
                value += 10;
            }
            else
            {
                value += int.Parse(card.Rank);
            }
        }

        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }
}
public class Game
{
    public Deck Deck { get; private set; }
    public Player Player { get; private set; } // Обычный игрок
    public Player Dealer { get; private set; } // Дилер
    public bool IsGameOver { get; private set; }

    public Game()
    {
        Deck = new Deck();
        Deck.Shuffle();
        Player = new Player();
        Dealer = new Player();
        IsGameOver = false;
    }

    public void StartNewGame()
    {
        Deck = new Deck();
        Deck.Shuffle();
        Player = new Player();
        Dealer = new Player();
        IsGameOver = false;

        // Раздача начальных карт
        Player.AddCard(Deck.DrawCard());
        Dealer.AddCard(Deck.DrawCard());
        Player.AddCard(Deck.DrawCard());
        Dealer.AddCard(Deck.DrawCard());
    }

    public string GetPlayerState()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Ваши карты: " + string.Join(", ", Player.Hand));
        sb.AppendLine("Счет: " + Player.CalculateHandValue());
        sb.AppendLine("Карты дилера: " + Dealer.Hand[0] + ", [скрыта]");
        return sb.ToString();
    }

    public string GetDealerState()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Карты дилера: " + string.Join(", ", Dealer.Hand));
        sb.AppendLine("Счет дилера: " + Dealer.CalculateHandValue());
        sb.AppendLine("Карты игрока: " + Player.Hand[0] + ", [скрыта]");
        return sb.ToString();
    }

    public void PlayerHit()
    {
        if (!IsGameOver)
        {
            Player.AddCard(Deck.DrawCard());
            if (Player.CalculateHandValue() > 21)
            {
                IsGameOver = true;
            }
        }
    }

    public void DealerHit()
    {
        if (!IsGameOver)
        {
            Dealer.AddCard(Deck.DrawCard());
            if (Dealer.CalculateHandValue() > 21)
            {
                IsGameOver = true;
            }
        }
    }

    public void PlayerStand()
    {
        if (!IsGameOver)
        {
            IsGameOver = true;
        }
    }

    public string GetGameResult()
    {
        if (!IsGameOver)
        {
            return "Игра еще не завершена.";
        }

        int playerValue = Player.CalculateHandValue();
        int dealerValue = Dealer.CalculateHandValue();

        if (playerValue > 21)
        {
            return "Игрок проиграл! Перебор.";
        }
        else if (dealerValue > 21)
        {
            return "Дилер проиграл! У дилера перебор.";
        }
        else if (playerValue > dealerValue)
        {
            return "Игрок выиграл!";
        }
        else if (playerValue < dealerValue)
        {
            return "Дилер выиграл.";
        }
        else
        {
            return "Ничья!";
        }
    }
}