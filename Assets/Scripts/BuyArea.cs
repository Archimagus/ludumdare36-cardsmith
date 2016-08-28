using UnityEngine;

public class BuyArea : MonoBehaviour
{
	public int BuyAreaCount = 5;
	public Transform DeckArea;
	public Transform DeckDiscardArea;
	public CardList Deck = new CardList();
	public CardList DeckDiscard = new CardList();

	// Use this for initialization
	void Start()
	{
		CreateDeck();
		foreach (var card in Deck)
		{
			card.transform.SetParent(DeckArea);
			card.transform.SetAsFirstSibling();
		}
	}


	// Update is called once per frame
	void Update()
	{
		if (Deck.Count == 0 && DeckDiscard.Count > 0)
		{
			DeckDiscard.Shuffle();
			Deck.AddRange(DeckDiscard);
			DeckDiscard.Clear();
			foreach (var card in Deck)
			{
				card.Location = CardLocations.DrawDeck;
				card.transform.SetParent(DeckArea);
			}
		}

		if (transform.childCount < BuyAreaCount && Deck.Count > 0)
		{
			var c = Deck.Dequeue();
			c.Location = CardLocations.BuyRow;
			c.transform.SetParent(transform);
			c.transform.SetAsFirstSibling();
		}
	}

	public void Discard()
	{
		if (transform.childCount > 0)
		{
			var t = transform.GetChild(transform.childCount - 1);
			t.SetParent(DeckDiscardArea);
			DeckDiscard.Add(t.GetComponent<Card>());
		}
	}

	private void CreateDeck()
	{
		var cp = GameManager.Instance.CardPrefab;


		///////////// METAL ////////////////////////
		var c = Instantiate(cp);
		c.MoneyCost = 4;
		c.Title = "Iron Lump";
		c.name = c.Title;
		c.Description = "+3 Metal";
		c.Flavor = "You need raw materials if you are going to make anything.";
		c.OnPlayed += (s, p) => p.Metal += 3;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		c = Instantiate(cp);
		c.MoneyCost = 6;
		c.Title = "Steel Bar";
		c.name = c.Title;
		c.Description = "+4 Metal";
		c.Flavor = "You need raw materials if you are going to make anything.";
		c.OnPlayed += (s, p) => p.Metal += 4;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		c = Instantiate(cp);
		c.MoneyCost = 8;
		c.Title = "Fine Metal Bars";
		c.name = c.Title;
		c.Description = "+5 Metal\n+1 VP";
		c.Flavor = "Ooh Shiney!";
		c.OnPlayed += (s, p) => p.Metal += 5;
		c.OnBought += (s, p) => p.VictoryPoints += 1;
		c.OnRemovedFromPlay += (s, p) => p.VictoryPoints -= 1;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		//////////////// MONEY ///////////////////////

		c = Instantiate(cp);
		c.MoneyCost = 4;
		c.Title = "Silver Coin";
		c.name = c.Title;
		c.Description = "+2 Coins";
		c.Flavor = "A bit o coin.";
		c.OnPlayed += (s, p) => p.Coins += 2;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		c = Instantiate(cp);
		c.MoneyCost = 5;
		c.Title = "Gold Coin";
		c.name = c.Title;
		c.Description = "+3 Coins";
		c.Flavor = "A bit o coin.";
		c.OnPlayed += (s, p) => p.Coins += 3;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		c = Instantiate(cp);
		c.MoneyCost = 9;
		c.Title = "Gold Pouch";
		c.name = c.Title;
		c.Description = "+5 Coins\n+1 VP";
		c.Flavor = "Big Spender!";
		c.OnPlayed += (s, p) => p.Coins += 5;
		c.OnBought += (s, p) => p.VictoryPoints += 1;
		c.OnRemovedFromPlay += (s, p) => p.VictoryPoints -= 1;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		/////////////// DOUBLES //////////////////////

		c = Instantiate(cp);
		c.MoneyCost = 3;
		c.MetalCost = 2;
		c.Title = "Tongs";
		c.name = c.Title;
		c.Description = "+1 Coins\n+2 Metal";
		c.Flavor = "Helps you work better.";
		c.OnPlayed += (s, p) =>
		{
			p.Coins += 1;
			p.Metal += 2;
		};
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		c = Instantiate(cp);
		c.MoneyCost = 2;
		c.MetalCost = 3;
		c.Title = "Heavy Hammer";
		c.name = c.Title;
		c.Description = "+2 Coins\n+1 Metal";
		c.Flavor = "Helps you work better.";
		c.OnPlayed += (s, p) =>
		{
			p.Coins += 2;
			p.Metal += 1;
		};
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		c = Instantiate(cp);
		c.MoneyCost = 3;
		c.MetalCost = 3;
		c.Title = "Auto Bellows";
		c.name = c.Title;
		c.Description = "+2 Coins\n+2 Metal";
		c.Flavor = "Helps you work better.";
		c.OnPlayed += (s, p) =>
		{
			p.Coins += 2;
			p.Metal += 2;
		};
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		////////////// EFFECTS ///////////////////////

		c = Instantiate(cp);
		c.MoneyCost = 4;
		c.Title = "Tool Box";
		c.name = c.Title;
		c.Description = "Draw 2 Cards";
		c.Flavor = "Yeah";
		c.OnPlayed += (s, p) => p.DrawCards(2);
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		c = Instantiate(cp);
		c.MoneyCost = 3;
		//c.FuelCost = 6;
		c.MetalCost = 3;
		c.Title = "Apprentice";
		c.name = c.Title;
		c.Description = "+2 Coins\nOptional: Remove a card in your hand or discard from play.";
		c.Flavor = "Someone needs to clean up arround here.";
		c.OnPlayed += (s, p) => { p.Coins += 2; s.MetaData = string.Empty; };
		c.OnActivated += (s, p) =>
		{
			if (s.MetaData != "Played")
			{
				p.ScrapCards(1);
				s.MetaData = "Played";
			}
		};
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		c = Instantiate(cp);
		c.MoneyCost = 2;
		//c.FuelCost = 6;
		c.MetalCost = 2;
		c.Title = "Waste Bin";
		c.name = c.Title;
		c.BlockAutoPlay = true;
		c.Description = "Optional: Remove a card in your hand or discard from play.";
		c.Flavor = "Someone needs to clean up arround here.";
		c.OnPlayed += (s, p) =>
		{
			p.ScrapCards(1);
		};
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		//c = Instantiate(cp);
		//c.MoneyCost = 2;
		//c.Title = "Coal";
		//c.name = c.Title;
		//c.Description = "+2 Fuel";
		//c.Flavor = "Put a little fire under that metal";
		//c.OnPlayed += p => p.Fuel += 2;
		//Deck.Add(c);
		//Deck.Add(c.Clone());
		//Deck.Add(c.Clone());
		//Deck.Add(c.Clone());

		Deck.Shuffle();
	}
}
