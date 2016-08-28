using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrapPanel : MonoBehaviour
{
	[SerializeField]
	private Transform _discardRow;
	[SerializeField]
	private Transform _handRow;
	[SerializeField]
	private Text _scrapText;
	[SerializeField]
	private Text _scrappingText;
	[SerializeField]
	private Button _scrapButton;

	public Player Player { get; set; }
	public int ScrapCount { get; set; }

	Transform _initialHandTransform;
	Transform _initialDiscardTransform;

	CardList _allCards = new CardList();
	CardList _cardsToScrap = new CardList();

	void Update()
	{
		_scrapText.text = string.Format("Scrap up to <Color=red>{0}</color> Cards.", ScrapCount);
		foreach (var c in _allCards)
		{
			c.Marked = false;
			if (c.Selected)
			{
				c.Selected = false;
				if (_cardsToScrap.Contains(c))
				{
					_cardsToScrap.Remove(c);
				}
				else
				{
					_cardsToScrap.Enqueue(c);
					if (_cardsToScrap.Count > ScrapCount)
						_cardsToScrap.Dequeue();
				}
			}
		}
		if (_cardsToScrap.Any())
		{

			string scrapCards = "Scrap: ";
			foreach (var c in _cardsToScrap)
			{
				c.Marked = true;
				scrapCards += c.Title + ", ";
			}
			scrapCards = scrapCards.Remove(scrapCards.Length - 2);
			_scrappingText.text = scrapCards;
			_scrapButton.interactable = true;
		}
		else
		{
			_scrapButton.interactable = false;
			_scrappingText.text = "Selct Scrap Cards.";
		}
	}

	void OnEnable()
	{
		foreach (var c in Player.CardsInHand)
		{
			_initialHandTransform = c.transform.parent;
			_allCards.Add(c);
			c.transform.SetParent(_handRow);
		}
		foreach (var c in Player.CardsInDiscard)
		{
			_initialDiscardTransform = c.transform.parent;
			_allCards.Add(c);
			c.transform.SetParent(_discardRow);
		}
	}

	public void Scrap()
	{
		foreach (var c in _cardsToScrap)
		{
			Player.CardsInHand.Remove(c);
			Player.CardsInDiscard.Remove(c);
			c.transform.SetParent(GameManager.Instance.OutOfPlayArea);
			c.RemoveFromPlay(Player);
		}
		Close();
	}

	public void Close()
	{
		_allCards.Clear();
		_cardsToScrap.Clear();
		foreach (var c in Player.CardsInHand)
		{
			c.Marked = false;
			c.transform.SetParent(_initialHandTransform);
		}
		foreach (var c in Player.CardsInDiscard)
		{
			c.Marked = false;
			c.transform.SetParent(_initialDiscardTransform);
		}
		gameObject.SetActive(false);
	}
}
