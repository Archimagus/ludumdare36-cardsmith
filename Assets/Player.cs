using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField]
	private bool _isAI = false;
	[SerializeField]
	private Transform _handArea;
	[SerializeField]
	private Transform _deckArea;
	[SerializeField]
	private Transform _discardArea;
	[SerializeField]
	private Transform _playArea;


	private static int _idGen = 0;
	public int ID { get; private set; }
	public int Money { get; set; }
	public int Fuel { get; set; }
	public int Metal { get; set; }
	public int VictoryPoints { get; set; }

	public readonly CardList CardsInHand = new CardList();
	public readonly CardList CardsInDeck = new CardList();
	public readonly CardList CardsInDiscard = new CardList();
	public readonly CardList CardsInPlay = new CardList();

	// Use this for initialization
	void Start()
	{
		ID = _idGen++;
		GameManager.Instance.Players.Add(this);

		CardsInHand.Changed += (s, e) => updateDispayArea(_handArea, CardsInHand);
		CardsInDeck.Changed += (s, e) => updateDispayArea(_deckArea, CardsInDeck);
		CardsInDiscard.Changed += (s, e) => updateDispayArea(_discardArea, CardsInDiscard);
		CardsInPlay.Changed += (s, e) => updateDispayArea(_playArea, CardsInPlay);

		CreateStarterDeck();
	}

	private void CreateStarterDeck()
	{
		var cp = GameManager.Instance.CardPrefab;

		var c = Instantiate(cp);
		c.Location = CardLocations.PlayerDeck;
		c.Cost = 1;
		c.Title = "Charcoal";
		c.name = c.Title;
		c.Description = "+1 Fuel";
		c.Flavor = "Not as good as coal, but it'll get you started.";
		c.OnPlayed += p => p.Fuel++;
		CardsInDeck.Add(c);
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());

		c = Instantiate(cp);
		c.Location = CardLocations.PlayerDeck;
		c.Cost = 1;
		c.Title = "Copper Coin";
		c.name = c.Title;
		c.Description = "+1 Money";
		c.Flavor = "A little jangle in your pocket.";
		c.OnPlayed += p => p.Money++;
		CardsInDeck.Add(c);
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());

		CardsInDeck.Shuffle();
	}

	void updateDispayArea(Transform area, CardList list)
	{
		if (area == null)
			return;
		area.DetachChildren();
		foreach (var card in list)
		{
			card.transform.SetParent(area);
			//card.transform.SetAsFirstSibling();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (_isAI && GameManager.Instance.CurrentPlayer == this)
		{
			// Do AI Turn.
			GameManager.Instance.SwitchTurns();
		}
	}
	public void BoughtCard(Card c)
	{
		CardsInDiscard.Add(c);
	}
	public void PlayAllCards()
	{
		while (CardsInHand.Count > 0)
		{
			CardsInHand[0].PlayCard(this);
		}
	}
	// Called by the Card, don't call.
	public void PlayCard(Card c)
	{
		CardsInHand.Remove(c);
		CardsInPlay.Add(c);
	}
	public void StartTurn()
	{
		DrawCards(GameManager.Instance.HandSize);
	}
	public void EndTurn()
	{
		Money = 0;
		Metal = 0;
		Fuel = 0;
		CardsInDiscard.AddRange(CardsInHand);
		CardsInDiscard.AddRange(CardsInPlay);
		CardsInHand.Clear();
		CardsInPlay.Clear();
	}

	public void DrawCards(int cards = 1)
	{
		for (int i = 0; i < cards; i++)
		{
			var c = drawCard();
			if (c == null)
				break;
			CardsInHand.Add(c);
			c.Drawn(this);
		}
	}

	private Card drawCard()
	{
		if (CardsInDeck.Count == 0)
		{
			if (CardsInDiscard.Count > 0)
			{
				CardsInDiscard.Shuffle();
				CardsInDeck.AddRange(CardsInDiscard);
				foreach (var c in CardsInDeck)
				{
					c.Location = CardLocations.PlayerDeck;
				}
				CardsInDiscard.Clear();
			}
		}
		return CardsInDeck.Dequeue();
	}

}
