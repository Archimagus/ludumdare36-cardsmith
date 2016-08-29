
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameOverScene : MonoBehaviour
{
	[SerializeField]
	private Text _winText;
	[SerializeField]
	private Text _lossText;

	void Start()
	{
		if (PlayerPrefs.GetString("WinningPlayer") == "Player")
			_lossText.enabled = false;
		else
		{
			_winText.enabled = false;
		}
	}
	public void QuitGame()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(0);
	}
}
