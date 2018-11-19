using UnityEngine;
using UnityEngine.UI;

public class FPSUI : MonoBehaviour 
{
	public  float updateInterval = 0.5F;
	
	private float _accum   = 0; // FPS accumulated over the interval
	private int   _frames  = 0; // Frames drawn over the interval
	private float _timeleft; // Left time for current interval

	private Text _text;
	
	void Start()
	{
		_timeleft = updateInterval; 
		_text = GetComponent<Text>();
	}
	
	void Update()
	{
		_timeleft -= Time.deltaTime;
		_accum += Time.timeScale/Time.deltaTime;
		++_frames;
		
		// Interval ended - update GUI text and start new interval
		if( _timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = _accum/_frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			_text.text = format;
			
			if(fps < 30)
				_text.color = Color.yellow;
			else 
				if(fps < 10)
					_text.color = Color.red;
			else
				_text.color = Color.green;

			_timeleft = updateInterval;
			_accum = 0.0F;
			_frames = 0;
		}
	}
}