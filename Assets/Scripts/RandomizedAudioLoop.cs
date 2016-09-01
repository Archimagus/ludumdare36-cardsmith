using System.Collections;
using UnityEngine;

[AddComponentMenu("Dungeon/Generic/RandomizedAudioLoop")]
public class RandomizedAudioLoop : MonoBehaviour
{
	[SerializeField]
	private bool _playOnAwake = false;
	[SerializeField]
	private bool _loop = true;
	[Range(-0.5f, 0.0f)]
	[SerializeField]
	private float _maxPitchUp = 0;
	[Range(0.0f, 0.5f)]
	[SerializeField]
	private float _maxPitchDown = 0;

	[SerializeField]
	private float _minRepeadDelay = 0.0f;
	[SerializeField]
	private float _maxRepeadDelay = 0.0f;

	[SerializeField]
	private AudioClip[] _clips;


	AudioSource _source;
	bool _playing;
	float _defaultPitch;

	void Start()
	{
		var go = new GameObject();
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		_source = go.AddComponent<AudioSource>();
		_source.loop = false;
		_source.rolloffMode = AudioRolloffMode.Linear;
		_defaultPitch = _source.pitch;
		if (_playOnAwake)
			Play();

	}
	public void Play()
	{
		_playing = true;
		StartCoroutine(LoopClips());
	}

	public void Stop()
	{
		_playing = false;
		_source.Stop();
	}

	IEnumerator LoopClips()
	{
		while (_playing && _clips.Length > 0)
		{
			var clip = _clips[Random.Range(0, _clips.Length)];
			_source.clip = clip;
			_source.pitch = _defaultPitch + Random.Range(_maxPitchDown, _maxPitchUp);
			_source.Play();
			if (!_loop)
				break;
			yield return new WaitForSeconds(clip.length + Random.Range(_minRepeadDelay, _maxRepeadDelay));
		}
	}
}
