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
	PlayArea,
	OutOfPlay
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
	[SerializeField]
	private GameObject CardBack;

	public static CardList AllCards = new CardList();

	public bool Selected
	{
		get { return _selected; }
		set
		{
			if (value)
			{
				foreach (var card in AllCards)
				{
					card.Selected = false;
				}
			}
			_selected = value;
		}
	}

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
	public Action<Player> OnRemovedFromPlay;
	private bool _selected;

	void Start()
	{
		AllCards.Add(this);
	}
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
		c.OnRemovedFromPlay = OnRemovedFromPlay;
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

		CardBack.SetActive(Location <= CardLocations.PlayerDeck);

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
			Location = CardLocations.PlayerDiscard;
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
	public void PlayCard(Player p)
	{
		Location = CardLocations.PlayArea;
		p.PlayCard(this);
		if (OnPlayed != null)
			OnPlayed(p);
	}
	public void ActivateCard(Player p)
	{
		if (OnActivated != null)
			OnActivated(p);
	}
	public void DiscardCard(Player p)
	{
		Location = CardLocations.PlayerDiscard;
		if (OnDiscarded != null)
			OnDiscarded(p);
	}
	public void RemoveFromPlay(Player p)
	{
		Location = CardLocations.OutOfPlay;
		if (OnRemovedFromPlay != null)
			OnRemovedFromPlay(p);
	}
}
