using UnityEngine;
using UnityEngine.UI;

public class SelectionBorder : MonoBehaviour
{
	private ISelectable _selectable;
	private Image _image;

	void Start()
	{
		_selectable = GetComponentInParent<ISelectable>();
		_image = GetComponent<Image>();
	}
	void Update()
	{
		_image.enabled = _selectable.Selected;
	}
}
