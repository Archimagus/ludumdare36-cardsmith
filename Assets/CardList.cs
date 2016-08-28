using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardList : IEnumerable<Card>
{
	readonly List<Card> _cards = new List<Card>();

	public int Count { get { return _cards.Count; } }

	public event EventHandler Changed;
	private void OnChanged()
	{
		if (Changed != null)
		{
			Changed(this, EventArgs.Empty);
		}
	}

	public Card Dequeue()
	{
		if (_cards.Count == 0)
			return null;
		var c = _cards.First();
		_cards.RemoveAt(0);
		OnChanged();
		return c;
	}

	public void Enqueue(Card card)
	{
		_cards.Add(card);
		OnChanged();
	}
	public void AddRange(IEnumerable<Card> cards)
	{
		_cards.AddRange(cards);
		OnChanged();
	}
	public void Add(Card card)
	{
		_cards.Add(card);
		OnChanged();
	}
	public void Remove(Card card)
	{
		_cards.Remove(card);
		OnChanged();
	}
	public void Clear()
	{
		_cards.Clear();
		OnChanged();
	}

	public Card this[int index]
	{
		get { return _cards[index]; }
		set
		{
			_cards[index] = value;
			OnChanged();
		}
	}

	public void Shuffle()
	{

		if (_cards.Count < 2)
			return;
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		var lastCard = _cards[_cards.Count - 1];
		Card currentCard;
		int iterations = 0;
		do
		{
			currentCard = _cards[0];
			_cards.Remove(currentCard);
			_cards.Insert(Random.Range(0, _cards.Count + 1), currentCard);
			iterations++;
		} while (currentCard.name != lastCard.name);
		sw.Stop();
		Debug.Log("Shuffle Time " + sw.ElapsedMilliseconds + "ms");
		Debug.Log("Iterations " + iterations);
		OnChanged();
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
