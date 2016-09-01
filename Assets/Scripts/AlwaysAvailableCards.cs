using UnityEngine;

public class AlwaysAvailableCards : MonoBehaviour
{
	public Card AvailableCard;
	// Update is called once per frame
	void Update()
	{
		if (transform.childCount == 0)
		{
			var c = Instantiate(GameManager.Instance.CardPrefab);
			AvailableCard = c;
			c.Location = CardLocations.BuyRow;
			//c.FuelCost = 1;
			//c.MoneyCost = 1;
			c.MetalCost = 2;
			c.Title = "Nails";
			c.name = c.Title;
			c.Description = "+2 Coins\nConsumed On Use";
			c.Consumable = true;
			c.BlockAutoPlay = true;
			c.Flavor = "Nails, everyone needs them so they are a good source of money.";
			c.OnPlayed += (s, p) =>
			{
				s.MetaData = "Played";
				p.Coins += 2;
			};

			c.OnDiscarded += (s, p) =>
			{
				if (s.MetaData == "Played")
				{
					p.CardsInDiscard.Remove(c);
					s.Location = CardLocations.BuyRow;
					s.MoveTo(transform);
					s.MetaData = string.Empty;
				}
			};
			c.transform.SetParent(transform);
		}
	}
}
