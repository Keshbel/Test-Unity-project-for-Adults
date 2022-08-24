using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Jun_TweenBase : MonoBehaviour
{
	[System.Serializable]
	public class OnFinish : UnityEvent{}

	public enum PlayType
	{
		Deful,
		One,
		Loop,
		PingPong
	}

	public PlayType playType = PlayType.Deful;
	public AnimationCurve curve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));
	public float animationTime = 1.0f;

	[Space(10)]
	public bool enablePlay = false;
	public bool awakePlay = true;
	public bool isLocal = true;
	public bool lgnoreTimeScale = true;

	[SerializeField]
	private OnFinish m_onFinish = new OnFinish();

	protected bool _isPlaying = false;
	public bool isPlaying {get{return _isPlaying;}}

	protected float playTimeNote;
	protected bool isPing = true;//true时为顺序，false为逆序

	public float currentCurveValue
	{
		get
		{
			if(!isPlaying)
				return 0;
			
			float curTimeLenght = lgnoreTimeScale? (Time.time - playTimeNote):(Time.unscaledTime - playTimeNote);
			float curValue = curTimeLenght/animationTime;

			if(!isPing)
			{
				curValue = 1 - curValue;
				if(curValue <= 0)
				{
					StopPlay ();
				}
			}

			else
			{
				if(curValue >= 1)
				{
					StopPlay ();
				}
			}

			curValue = curValue < 0?0:curValue;
			curValue = curValue > 1?1:curValue;



			float curveValue = curve.Evaluate (curValue);
			return curveValue;
		}
	}

	void Awake ()
	{
		if(awakePlay)
		{
			Play ();
		}
	}

	void OnEnable()
	{
		if(enablePlay)
			Play ();
	}

	public void AddOnFinished (UnityAction call)
	{
		m_onFinish.AddListener (call);
	}

	public void ClearOnFinished ()
	{
		m_onFinish.RemoveAllListeners ();
	}

	public virtual void Play ()
	{
		_isPlaying = true;
		NoteTime ();
		this.enabled = true;
	}

	protected void NoteTime ()
	{
		if(!lgnoreTimeScale)
			playTimeNote = Time.unscaledTime;
		else
			playTimeNote = Time.time;
	}

	public virtual void StopPlay ()
	{
		switch (playType)
		{
		case PlayType.Deful:
		case PlayType.One:
			_isPlaying = false;
			m_onFinish.Invoke ();
			this.enabled = false;
			break;

		case PlayType.Loop:
			NoteTime ();
			break;

		case PlayType.PingPong:
			NoteTime ();
			isPing = !isPing;
			break;
		}
	}
}
