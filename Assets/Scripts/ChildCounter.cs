using UnityEngine;
using UnityEngine.UI;

public class ChildCounter : MonoBehaviour
{
	[SerializeField]
	private Transform _transformToCount;

	[SerializeField]
	private Text _text;


	// Update is called once per frame
	void Update()
	{
		_text.text = _transformToCount.childCount.ToString();
	}
}
