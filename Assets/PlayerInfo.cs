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
		_moneyText.text = _targetPlayer.Money.ToString();
		_fuelText.text = _targetPlayer.Fuel.ToString();
		_metalText.text = _targetPlayer.Metal.ToString();
		_victoryPointsText.text = _targetPlayer.VictoryPoints.ToString();
	}
}
