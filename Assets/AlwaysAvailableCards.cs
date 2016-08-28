using UnityEngine;

public class AlwaysAvailableCards : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		if (transform.childCount == 0)
		{
			var c = Instantiate(GameManager.Instance.CardPrefab);
			c.Location = CardLocations.BuyRow;
			c.Cost = 2;
			c.Title = "Nails";
			c.name = c.Title;
			c.Description = "+2 Money\nConsumable";
			c.Flavor = "Nails, everyone needs them so they are a good source of money.";
			c.OnPlayed += p =>
			{
				p.Money += 2;
				p.CardsInPlay.Remove(c);
				c.Location = CardLocations.BuyRow;
				c.transform.SetParent(transform);
			};

			c.transform.SetParent(transform);
		}
	}
}
