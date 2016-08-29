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

		CardsInHand.Changed += (s, e) => updateDispayArea(_handArea, CardsInHand);
		CardsInDeck.Changed += (s, e) => updateDispayArea(_deckArea, CardsInDeck);
		CardsInDiscard.Changed += (s, e) => updateDispayArea(_discardArea, CardsInDiscard);
		CardsInPlay.Changed += (s, e) => updateDispayArea(_playArea, CardsInPlay);

		CreateStarterDeck();
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

	private bool _aiStarted;
	private bool _aiDone;
	// Update is called once per frame
	void Update()
	{
		if (_isAI && GameManager.Instance.CurrentPlayer == this)
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
				GameManager.Instance.ScrapPanel.gameObject.SetActive(false);
		}
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

		yield return delay;
		_aiDone = true;
	}
	public void BoughtCard(Card c)
	{
		CardsInDiscard.Add(c);
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
	}
	public void EndTurn()
	{
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
				foreach (var c in CardsInDeck)
				{
					c.Location = CardLocations.PlayerDeck;
				}
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
