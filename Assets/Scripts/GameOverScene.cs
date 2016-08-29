using UnityEditor;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEngine;
#endif
public class GameOverScene : MonoBehaviour
{
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
