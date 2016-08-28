using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardLocations
{
	DrawDeck,
	PlayerDeck,
	PlayerDiscard,
	BuyRow,
	Hand,
	PlayArea
}
public class Card : MonoBehaviour, ISelectable, IPointerClickHandler
{
	[SerializeField]
	private Text CostText;
	[SerializeField]
	private Text TitleText;
	[SerializeField]
	private Text DescriptionText;
	[SerializeField]
	private Text FlavorText;

	public bool Selected { get; set; }

	public CardLocations Location;

	public int Cost;
	public string Title;
	public string Description;
	public string Flavor;
	public Action<Player> OnBought;
	public Action<Player> OnPlayed;
	public Action<Player> OnDrawn;
	public Action<Player> OnActivated;
	public Action<Player> OnDiscarded;

	public Card Clone()
	{
		var c = Instantiate(this);
		c.Cost = Cost;
		c.Title = Title;
		c.name = c.Title;
		c.Description = Description;
		c.Flavor = Flavor;
		c.OnBought = OnBought;
		c.OnPlayed = OnPlayed;
		c.OnDrawn = OnDrawn;
		c.OnActivated = OnActivated;
		c.OnDiscarded = OnDiscarded;
		return c;
	}

	public void Update()
	{
		if (Location == CardLocations.BuyRow)
		{
			CostText.transform.parent.gameObject.SetActive(true);
			CostText.text = Cost.ToString();
		}
		else
			CostText.transform.parent.gameObject.SetActive(false);

		TitleText.text = Title;
		DescriptionText.text = Description;
		FlavorText.text = Flavor;
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		Selected = true;
		if (eventData.clickCount == 2)
		{
			ProcessDoubleClick();
		}
	}
	private void ProcessDoubleClick()
	{
		var p = GameManager.Instance.CurrentPlayer;
		Selected = false;
		switch (Location)
		{
			case CardLocations.DrawDeck:
				break;
			case CardLocations.PlayerDeck:
				break;
			case CardLocations.BuyRow:
				TryBuyCard(p);
				break;
			case CardLocations.Hand:
				PlayCard(p);
				break;
			case CardLocations.PlayArea:
				ActivateCard(p);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void TryBuyCard(Player p)
	{
		if (p.Money >= Cost)
		{
			p.Money -= Cost;
			Location = CardLocations.PlayerDeck;
			p.BoughtCard(this);
			if (OnBought != null)
				OnBought(p);
		}
	}
	public void Drawn(Player p)
	{
		Location = CardLocations.Hand;
		if (OnDrawn != null)
			OnDrawn(p);
	}
	private void PlayCard(Player p)
	{
		Location = CardLocations.PlayArea;
		p.PlayCard(this);
		if (OnPlayed != null)
			OnPlayed(p);
	}
	private void ActivateCard(Player p)
	{
		if (OnActivated != null)
			OnActivated(p);
	}
	private void DiscardCard(Player p)
	{
		Location = CardLocations.PlayerDiscard;
		if (OnDiscarded != null)
			OnDiscarded(p);
	}
}
