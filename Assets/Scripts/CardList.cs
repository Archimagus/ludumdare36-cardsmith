using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public enum CardListChangeType
{
	Add,
	Remove,
	Move,
}

public class CardListChangedEventArgs : EventArgs
{
	public CardListChangeType ChangeType { get; private set; }
	public Card Card { get; private set; }

	public CardListChangedEventArgs(Card card, CardListChangeType cardListChangeType)
	{
		Card = card;
		ChangeType = cardListChangeType;
	}
}
public class CardList : IEnumerable<Card>
{
	readonly List<Card> _cards = new List<Card>();

	public int Count { get { return _cards.Count; } }

	public event EventHandler<CardListChangedEventArgs> Changed;
	private void OnChanged(Card card, CardListChangeType changeType)
	{
		if (Changed != null)
		{
			Changed(this, new CardListChangedEventArgs(card, changeType));
		}
	}

	public Card Dequeue()
	{
		if (_cards.Count == 0)
			return null;
		var c = _cards.First();
		_cards.RemoveAt(0);
		OnChanged(c, CardListChangeType.Remove);
		return c;
	}

	internal bool Contains(Card c)
	{
		return _cards.Contains(c);
	}

	public void Enqueue(Card card)
	{
		_cards.Add(card);
		OnChanged(card, CardListChangeType.Add);
	}
	public void AddRange(IEnumerable<Card> cards)
	{
		_cards.AddRange(cards);
		foreach (var card in cards)
		{
			OnChanged(card, CardListChangeType.Add);
		}
	}
	public void Add(Card card)
	{
		_cards.Add(card);
		OnChanged(card, CardListChangeType.Add);
	}
	public void Remove(Card card)
	{
		if (_cards.Contains(card))
		{
			_cards.Remove(card);
			OnChanged(card, CardListChangeType.Remove);
		}
	}
	public void Clear()
	{
		var cards = new List<Card>(_cards);
		_cards.Clear();
		foreach (var card in cards)
		{
			OnChanged(card, CardListChangeType.Remove);
		}
	}

	public Card this[int index]
	{
		get { return _cards[index]; }
		set
		{
			if (_cards[index] != value)
			{
				OnChanged(_cards[index], CardListChangeType.Remove);
				_cards[index] = value;
				OnChanged(value, CardListChangeType.Add);
			}
		}
	}

	public void Shuffle()
	{

		if (_cards.Count < 2)
			return;
		var lastCard = _cards[_cards.Count - 1];
		Card currentCard;
		do
		{
			currentCard = _cards[0];
			_cards.Remove(currentCard);
			_cards.Insert(Random.Range(0, _cards.Count + 1), currentCard);
		} while (currentCard.name != lastCard.name);
		foreach (var card in _cards)
		{
			OnChanged(card, CardListChangeType.Move);
		}
	}

	public IEnumerator<Card> GetEnumerator()
	{
		return _cards.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

}
