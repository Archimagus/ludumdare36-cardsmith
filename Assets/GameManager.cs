using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public int HandSize = 5;
	public Card CardPrefab;
	public static GameManager Instance { get; private set; }
	public List<Player> Players { get; private set; }
	public int Turn { get; set; }

	public Player CurrentPlayer { get { return Players[Turn]; } }

	private bool _gameStarted;

	// Use this for initialization
	void Awake()
	{
		Instance = this;
		Players = new List<Player>();
	}
	IEnumerator Start()
	{
		yield return null;
		if (!_gameStarted)
			CurrentPlayer.StartTurn();
	}
	// Update is called once per frame
	void Update()
	{

	}
	public void SwitchTurns()
	{
		_gameStarted = true;
		CurrentPlayer.EndTurn();
		Turn = Mathf.Abs(Turn - 1);
		CurrentPlayer.StartTurn();
	}
}
