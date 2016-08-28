using UnityEngine;

public class BuyArea : MonoBehaviour
{
	public int BuyAreaCount = 5;
	public Transform DeckArea;
	public CardList Deck = new CardList();

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
			c.Location = CardLocations.BuyRow;
			c.transform.SetParent(transform);
		}
	}


	private void CreateDeck()
	{
		var cp = GameManager.Instance.CardPrefab;

		var c = Instantiate(cp);
		c.MoneyCost = 2;
		c.Title = "Coal";
		c.name = c.Title;
		c.Description = "+2 Fuel";
		c.Flavor = "Put a little fire under that metal";
		c.OnPlayed += p => p.Fuel += 2;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		c = Instantiate(cp);
		c.MoneyCost = 2;
		c.Title = "Iron Lump";
		c.name = c.Title;
		c.Description = "+2 Metal";
		c.Flavor = "You need raw materials if you are going to make anything.";
		c.OnPlayed += p => p.Metal += 2;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());


		c = Instantiate(cp);
		c.MoneyCost = 4;
		c.Title = "Tool Box";
		c.name = c.Title;
		c.Description = "Draw 2 Cards";
		c.Flavor = "Yeah";
		c.OnPlayed += p => p.DrawCards(2);
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		Deck.Shuffle();
	}
}
