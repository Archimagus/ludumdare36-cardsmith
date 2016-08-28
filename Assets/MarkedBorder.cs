using UnityEngine;
using UnityEngine.UI;

public class MarkedBorder : MonoBehaviour
{
	private Card _card;
	private Image _image;

	void Start()
	{
		_card = GetComponentInParent<Card>();
		_image = GetComponent<Image>();
	}
	void Update()
	{
		_image.enabled = _card.Marked;
	}
}
