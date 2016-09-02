using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{

	[SerializeField]
	private Player _targetPlayer;
	[SerializeField]
	private Text _moneyText;
	[SerializeField]
	private Text _fuelText;
	[SerializeField]
	private Text _metalText;
	[SerializeField]
	private Text _victoryPointsText;


	// Update is called once per frame
	void Update()
	{
		if (_moneyText != null)
			_moneyText.text = _targetPlayer.Coins.ToString();
		if (_fuelText != null)
			_fuelText.text = _targetPlayer.Fuel.ToString();
		if (_metalText != null)
			_metalText.text = _targetPlayer.Metal.ToString();
		if (_victoryPointsText != null)
			_victoryPointsText.text = _targetPlayer.VictoryPoints.ToString();
	}
}
