using UnityEngine;
using UnityEngine.UI;
using System;
namespace AssemblyCSharp
{
	public class FPSText : MonoBehaviour
	{
		public  float updateInterval = 0.5F;
		
		private float accum   = 0; // FPS accumulated over the interval
		private int   frames  = 0; // Frames drawn over the interval
		private float timeleft; // Left time for current interval

		public TextMesh guitext;
		private void Start()
		{
			gameObject.transform.position = new Vector3 (0.5f, 0.5f, 0f);
			guitext.text = "FPS";
			Debug.Log ("created FPS counter");
			timeleft = updateInterval;  
		}
		private void Update()
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale/Time.deltaTime;
			++frames;
			
			// Interval ended - update GUI text and start new interval
			if( timeleft <= 0.0 )
			{
				// display two fractional digits (f2 format)
				float fps = accum/frames;
				string format = System.String.Format("{0:F2} FPS",fps);
				//guitext.text = fps.ToString();
				print (format);

				timeleft = updateInterval;
				accum = 0.0F;
				frames = 0;
			}
		}
	}
}

