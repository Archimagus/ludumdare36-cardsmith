using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameManager : MonoBehaviour
{
	public int HandSize = 5;
	public int VictoryPointGoal = 21;
	public Card CardPrefab;
	public Transform OutOfPlayArea;
	public ScrapPanel ScrapPanel;
	public BuyArea BuyArea;
	public GameObject QuitGamePanel;

	public static GameManager Instance { get; private set; }
	public List<Player> Players { get; private set; }
	public int Turn { get; set; }
	public bool AllowMultiSelect { get; set; }

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
		if (Input.GetKeyDown(KeyCode.Escape))
			QuitGamePanel.gameObject.SetActive(true);
	}
	public void QuitGame()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	public void SwitchTurns()
	{
		_gameStarted = true;
		CurrentPlayer.EndTurn();

		if (CheckWinCondition())
		{
			return;
		}

		Turn = Mathf.Abs(Turn - 1);
		if (Turn == 0)
			BuyArea.Discard();
		CurrentPlayer.StartTurn();

	}

	private bool CheckWinCondition()
	{
		if (CurrentPlayer.VictoryPoints > VictoryPointGoal)
		{
			PlayerPrefs.SetInt("WinningPlayer", Turn);
			SceneManager.LoadScene("VictoryScene");
			return true;
		}
		return false;
	}

}
