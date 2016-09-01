using UnityEngine;

public class SpecialBuyArea : MonoBehaviour
{
	public int BuyAreaCount = 5;
	public Transform DeckArea;
	public CardList Deck = new CardList();
	public CardList AvailableCards = new CardList();
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
		if (transform.childCount < BuyAreaCount && Deck.Count > 0)
		{
			var c = Deck.Dequeue();
			AvailableCards.Add(c);
			c.OnBought += CardBought;
			c.Location = CardLocations.BuyRow;
			c.MoveTo(transform);
		}
	}

	void CardBought(Card c, Player p)
	{
		AvailableCards.Remove(c);
		c.OnBought -= CardBought;
	}

	private void CreateDeck()
	{
		var cp = GameManager.Instance.CardPrefab;

		{
			var c = Instantiate(cp);
			//c.FuelCost = 10;
			c.MetalCost = 10;
			c.Title = "Excalibur";
			c.name = c.Title;
			c.Description = "+7 VP";
			c.Flavor = "Such a masterfull sword you've crafted.";
			c.OnBought += (s, p) => p.VictoryPoints += 7;
			c.OnRemovedFromPlay += (s, p) => p.VictoryPoints -= 7;
			c.OnPlayed += (s, p) => p.Discard(s);
			Deck.Add(c);
			Deck.Add(c.Clone());
		}
		{
			var c = Instantiate(cp);
			//c.FuelCost = 3;
			c.MetalCost = 7;
			c.Title = "Plate Mail";
			c.name = c.Title;
			c.Description = "+4 VP";
			c.Flavor = "Only for the most noble.";
			c.OnBought += (s, p) => p.VictoryPoints += 4;
			c.OnRemovedFromPlay += (s, p) => p.VictoryPoints -= 4;
			c.OnPlayed += (s, p) => p.Discard(s);
			Deck.Add(c);
			Deck.Add(c.Clone());
			Deck.Add(c.Clone());
			Deck.Add(c.Clone());
		}
		{
			var c = Instantiate(cp);
			//c.FuelCost = 3;
			c.MetalCost = 4;
			c.Title = "Shiny Shield";
			c.name = c.Title;
			c.Description = "+2 VP";
			c.Flavor = "So bright you can see yourself in it.";
			c.OnBought += (s, p) => p.VictoryPoints += 2;
			c.OnRemovedFromPlay += (s, p) => p.VictoryPoints -= 2;
			c.OnPlayed += (s, p) => p.Discard(s);
			Deck.Add(c);
			Deck.Add(c.Clone());
			Deck.Add(c.Clone());
			Deck.Add(c.Clone());
			Deck.Add(c.Clone());
			Deck.Add(c.Clone());
		}

		Deck.Shuffle();
	}
}
