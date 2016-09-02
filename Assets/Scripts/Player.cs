using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
	public bool _isAI = false;
	[SerializeField]
	private Transform _handArea;
	[SerializeField]
	private Transform _deckArea;
	[SerializeField]
	private Transform _discardArea;
	[SerializeField]
	private Transform _playArea;

	private bool _myTurn;

	private static int _idGen = 0;
	public int ID { get; private set; }
	public int Coins { get; set; }
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

		CardsInHand.Changed += (s, e) => updateDispayArea((CardList)s, e.Card, e.ChangeType);
		CardsInDeck.Changed += (s, e) => updateDispayArea((CardList)s, e.Card, e.ChangeType);
		CardsInDiscard.Changed += (s, e) => updateDispayArea((CardList)s, e.Card, e.ChangeType);
		CardsInPlay.Changed += (s, e) => updateDispayArea((CardList)s, e.Card, e.ChangeType);

		CardsInHand.Location = CardLocations.Hand;
		CardsInHand.DisplayArea = _handArea;
		CardsInHand.DisplayAreaOrder = -1;

		CardsInDeck.Location = CardLocations.PlayerDeck;
		CardsInDeck.DisplayArea = _deckArea;
		CardsInDeck.DisplayAreaOrder = -1;

		CardsInDiscard.Location = CardLocations.PlayerDiscard;
		CardsInDiscard.DisplayArea = _discardArea;
		CardsInDiscard.DisplayAreaOrder = 1;

		CardsInPlay.Location = CardLocations.PlayArea;
		CardsInPlay.DisplayArea = _playArea;
		CardsInPlay.DisplayAreaOrder = -1;

		CreateStarterDeck();
	}
	void updateDispayArea(CardList list, Card c, CardListChangeType type)
	{
		if (type == CardListChangeType.Add || type == CardListChangeType.Move)
		{
			c.MoveTo(list.DisplayArea, list.Location, list.DisplayAreaOrder);
		}
	}

	private bool _aiStarted;
	private bool _aiDone;
	// Update is called once per frame
	void Update()
	{
		if (_isAI && _myTurn)
		{
			if (!_aiStarted)
			{
				_aiStarted = true;
				_aiDone = false;
				StartCoroutine(DoAI());
			}
			if (_aiDone)
			{
				_aiStarted = false;
				// Do AI Turn.
				GameManager.Instance.SwitchTurns();
			}
		}
	}
	IEnumerator DoAI()
	{
		var delay = new WaitForSeconds(0.25f);
		while (CardsInHand.Any())
		{
			CardsInHand[0].PlayCard(this);
			yield return delay;
			if (GameManager.Instance.ScrapPanel.isActiveAndEnabled)
			{
				yield return delay;
				GameManager.Instance.ScrapPanel.ScrapCheapest();
				yield return delay;
			}
		}
		while (CardsInPlay.Any(c => c.Interactable == false))
			yield return null;

		var marked = GameManager.Instance.SpecialBuyArea.AvailableCards.Where(c => c.Marked).ToArray();
		while (marked.Any())
		{
			var card = marked.OrderByDescending(c => c.MetalCost + c.MoneyCost + c.FuelCost).First();
			card.TryBuyCard(this);
			marked = GameManager.Instance.SpecialBuyArea.AvailableCards.Where(c => c.Marked).ToArray();
			yield return delay;
		}

		marked = GameManager.Instance.BuyArea.AvailableCards.Where(c => c.Marked).ToArray();
		while (marked.Any())
		{
			var card = marked.OrderByDescending(c => c.MetalCost + c.MoneyCost + c.FuelCost).First();
			card.TryBuyCard(this);
			marked = GameManager.Instance.BuyArea.AvailableCards.Where(c => c.Marked).ToArray();
			yield return delay;
		}

		while (Metal >= 2 && Coins >= 2)
		{
			GameManager.Instance.AlwaysAvailableCards.AvailableCard.TryBuyCard(this);
		}

		while (CardsInPlay.Any(c => c.Interactable == false)
			|| CardsInHand.Any(c => c.Interactable == false)
			|| CardsInDeck.Any(c => c.Interactable == false)
			|| CardsInDiscard.Any(c => c.Interactable == false))
			yield return null;

		yield return delay;
		yield return delay;
		_aiDone = true;
	}
	public void BoughtCard(Card c)
	{
		CardsInDiscard.Add(c);
		c.MoveTo(_discardArea, CardLocations.PlayerDiscard, 1);
	}
	public void PlayAllCards()
	{
		var cards = CardsInHand.Take(CardsInHand.Count).ToArray();
		foreach (var c in cards)
		{
			if (!c.BlockAutoPlay)
				c.PlayCard(this);
		}
	}
	// Called by the Card, don't call.
	public void PlayCard(Card c)
	{
		CardsInHand.Remove(c);
		CardsInPlay.Add(c);
	}
	public void ScrapCards(int count)
	{
		var panel = GameManager.Instance.ScrapPanel;
		panel.Player = this;
		panel.ScrapCount = count;
		panel.gameObject.SetActive(true);
	}
	public void StartTurn()
	{
		DrawCards(GameManager.Instance.HandSize);
		_myTurn = true;
	}
	public void EndTurn()
	{
		_myTurn = false;

		Coins = 0;
		Metal = 0;
		Fuel = 0;

		while (CardsInHand.Count > 0) { CardsInHand[0].DiscardCard(this); }
		while (CardsInPlay.Count > 0) { CardsInPlay[0].DiscardCard(this); }
	}
	public void Discard(Card card)
	{
		CardsInDiscard.Add(card);
		CardsInHand.Remove(card);
		CardsInPlay.Remove(card);
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
				CardsInDiscard.Clear();
			}
		}
		return CardsInDeck.Dequeue();
	}

	private void CreateStarterDeck()
	{
		var cp = GameManager.Instance.CardPrefab;


		var c = Instantiate(cp);
		c.Location = CardLocations.PlayerDeck;
		c.MoneyCost = 1;
		c.Title = "Metal Scraps";
		c.name = c.Title;
		c.Description = "+1 Metal";
		c.Flavor = "Just some hunks of metal you found laying around.";
		c.OnPlayed += (s, p) => p.Metal++;
		CardsInDeck.Add(c);
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());

		//c = Instantiate(cp);
		//c.Location = CardLocations.PlayerDeck;
		//c.MoneyCost = 1;
		//c.Title = "Charcoal";
		//c.name = c.Title;
		//c.Description = "+1 Fuel";
		//c.Flavor = "Not as good as coal, but it'll get you started.";
		//c.OnPlayed += p => p.Fuel++;
		//CardsInDeck.Add(c);
		//CardsInDeck.Add(c.Clone());

		c = Instantiate(cp);
		c.Location = CardLocations.PlayerDeck;
		c.MoneyCost = 1;
		c.Title = "Copper Coin";
		c.name = c.Title;
		c.Description = "+1 Coins";
		c.Flavor = "A little jangle in your pocket.";
		c.OnPlayed += (s, p) => p.Coins++;
		CardsInDeck.Add(c);
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());
		CardsInDeck.Add(c.Clone());

		CardsInDeck.Shuffle();
	}

}
