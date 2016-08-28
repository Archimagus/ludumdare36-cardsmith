using UnityEngine;

public class SpecialBuyArea : MonoBehaviour
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
		c.Cost = 10;
		c.Title = "Excalibur";
		c.name = c.Title;
		c.Description = "+7 VP";
		c.Flavor = "Such a masterfull sword you've crafted.";
		c.OnBought += p => p.VictoryPoints += 7;
		c.OnRemovedFromPlay += p => p.VictoryPoints -= 7;
		Deck.Add(c);
		Deck.Add(c.Clone());

		c = Instantiate(cp);
		c.Cost = 10;
		c.Title = "Apprentice";
		c.name = c.Title;
		c.Description = "+3 VP\nOnPlay: Remove a card in your discard from play.";
		c.Flavor = "Someone needs to clean up arround here.";
		c.OnPlayed += p => p.Metal += 2;
		c.OnBought += p => p.VictoryPoints += 3;
		c.OnRemovedFromPlay += p => p.VictoryPoints -= 3;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		c = Instantiate(cp);
		c.Cost = 6;
		c.Title = "Shield of Perseus";
		c.name = c.Title;
		c.Description = "+3 VP";
		c.Flavor = "So bright you can see yourself in it.";
		c.OnBought += p => p.VictoryPoints += 3;
		c.OnRemovedFromPlay += p => p.VictoryPoints -= 3;
		Deck.Add(c);
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());
		Deck.Add(c.Clone());

		Deck.Shuffle();
	}
}
