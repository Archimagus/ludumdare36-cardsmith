using System;
using System.Collections.Generic;
using System.Linq;
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
	private Text MoneyCostText;
	[SerializeField]
	private Text FuelCostText;
	[SerializeField]
	private Text MetalCostText;
	[SerializeField]
	private Text TitleText;
	[SerializeField]
	private Text DescriptionText;
	[SerializeField]
	private Text FlavorText;
	[SerializeField]
	private GameObject CardBack;

	public static CardList AllCards = new CardList();
	private RandomizedAudioLoop _audioLoop;
	private bool _selected;
	public bool Selected
	{
		get { return _selected; }
		set
		{
			if (value && !GameManager.Instance.AllowMultiSelect)
			{
				foreach (var card in AllCards)
				{
					card.Selected = false;
				}
			}
			_selected = value;
		}
	}

	public bool Marked { get; set; }

	public string MetaData { get; set; }
	public bool Interactable { get; private set; }

	public CardLocations Location;

	public int TotalCost { get { return MoneyCost + MetalCost + FuelCost; } }

	public int MoneyCost;
	public int MetalCost;
	public int FuelCost;
	public string Title;
	public string Description;
	public string Flavor;
	public bool Consumable;
	public bool BlockAutoPlay;
	public Action<Card, Player> OnBought;
	public Action<Card, Player> OnPlayed;
	public Action<Card, Player> OnDrawn;
	public Action<Card, Player> OnActivated;
	public Action<Card, Player> OnDiscarded;
	public Action<Card, Player> OnRemovedFromPlay;

	void Start()
	{
		AllCards.Add(this);
		_audioLoop = GetComponent<RandomizedAudioLoop>();
	}

	public Card Clone()
	{
		var c = Instantiate(this);
		c.MoneyCost = MoneyCost;
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
			MoneyCostText.transform.parent.gameObject.SetActive(MoneyCost > 0);
			MoneyCostText.text = MoneyCost.ToString();

			FuelCostText.transform.parent.gameObject.SetActive(FuelCost > 0);
			FuelCostText.text = FuelCost.ToString();

			MetalCostText.transform.parent.gameObject.SetActive(MetalCost > 0);
			MetalCostText.text = MetalCost.ToString();
		}
		else
		{
			MoneyCostText.transform.parent.gameObject.SetActive(false);
			FuelCostText.transform.parent.gameObject.SetActive(false);
			MetalCostText.transform.parent.gameObject.SetActive(false);
		}

		CardBack.SetActive(Location <= CardLocations.PlayerDeck);

		if (!GameManager.Instance.ScrapPanel.isActiveAndEnabled)
		{
			if (Location == CardLocations.PlayArea)
				Marked = (OnActivated != null && MetaData == string.Empty);
			else if (Location == CardLocations.BuyRow)
				Marked = CanAffordCard(GameManager.Instance.CurrentPlayer);
			else
				Marked = false;
		}


		TitleText.text = Title;
		DescriptionText.text = Description;
		FlavorText.text = Flavor;

		DoMovement();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Interactable)
			return;
		Selected = true;
		if (eventData.clickCount == 2)
		{
			ProcessDoubleClick();
		}
	}

	private void ProcessDoubleClick()
	{
		// Don't allow using cards while the scrap panel is open.
		if (GameManager.Instance.ScrapPanel.isActiveAndEnabled)
			return;
		var p = GameManager.Instance.CurrentPlayer;
		Selected = false;
		switch (Location)
		{
			case CardLocations.DrawDeck:
				break;
			case CardLocations.PlayerDeck:
				break;
			case CardLocations.PlayerDiscard:
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
			case CardLocations.OutOfPlay:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void TryBuyCard(Player p)
	{
		if (CanAffordCard(p))
		{
			p.Coins -= MoneyCost;
			p.Fuel -= FuelCost;
			p.Metal -= MetalCost;
			p.BoughtCard(this);
			if (OnBought != null)
				OnBought(this, p);
		}
	}

	private bool CanAffordCard(Player p)
	{
		return (p.Coins >= MoneyCost && p.Fuel >= FuelCost && p.Metal >= MetalCost);
	}

	public void Drawn(Player p)
	{
		if (OnDrawn != null)
			OnDrawn(this, p);
	}

	public void PlayCard(Player p)
	{
		p.PlayCard(this);
		if (OnPlayed != null)
			OnPlayed(this, p);
	}

	public void ActivateCard(Player p)
	{
		if (OnActivated != null)
			OnActivated(this, p);
	}

	public void DiscardCard(Player p)
	{
		p.Discard(this);
		if (OnDiscarded != null)
			OnDiscarded(this, p);
	}

	public void RemoveFromPlay(Player p)
	{
		if (OnRemovedFromPlay != null)
			OnRemovedFromPlay(this, p);
	}

	private struct CardDestination
	{
		public GameObject _placeholder;
		public CardLocations _location;

		public CardDestination(GameObject placeholder, CardLocations location)
		{
			this._placeholder = placeholder;
			this._location = location;
		}
	}

	private CardLocations _newLocation;
	private GameObject _placeholder;
	private Queue<CardDestination> _placeholders = new Queue<CardDestination>();
	public void MoveTo(Transform newParent, CardLocations newLocation, int childPlacement = 0)
	{
		if (newParent == null)
			Debug.LogError("Moving to null", this);
		var ph = Instantiate(GameManager.Instance.CardPlaceholderPrefab);
		ph.transform.SetParent(newParent);

		if (childPlacement < 0)
			ph.transform.SetAsFirstSibling();
		if (childPlacement > 0)
			ph.transform.SetAsLastSibling();

		_placeholders.Enqueue(new CardDestination(ph, newLocation));

	}
	void DoMovement()
	{
		if (_placeholder == null && _placeholders.Any())
		{
			Interactable = false;
			var ph = _placeholders.Dequeue();
			_placeholder = ph._placeholder;
			_newLocation = ph._location;
			transform.SetParent(GameManager.Instance.Canvas, true);
			if (_audioLoop != null)
				_audioLoop.Play();
		}
		if (_placeholder == null)
		{
			Interactable = true;
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, _placeholder.transform.position, 1000f * Time.deltaTime);
			if (Vector3.Distance(_placeholder.transform.position, transform.position) < 1f)
			{
				var newParent = _placeholder.transform.parent;
				var newIndex = _placeholder.transform.GetSiblingIndex();
				Location = _newLocation;
				Destroy(_placeholder);
				_placeholder = null;
				if (_placeholders.Count == 0)
				{
					transform.SetParent(newParent);
					transform.SetSiblingIndex(newIndex);
					Interactable = true;
				}
			}
		}
	}

}
