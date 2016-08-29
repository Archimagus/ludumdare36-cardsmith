using UnityEditor;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEngine;
#endif

public class TitleScreen : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
		}

		if (Input.anyKeyDown)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

	}
}
