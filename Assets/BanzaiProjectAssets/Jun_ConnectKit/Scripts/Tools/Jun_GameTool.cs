using UnityEngine;
using System.Collections;

public class Jun_GameTool
{
	public static string TimeString (int second)
	{
		return TimeString (second,3);
	}

	public static string TimeString (int second,int strCount)
	{
		int v = 6000;
		string hourStr = "00";
		int hour = second/v;
		if(hour != 0)
		{
			if(hour.ToString().Length == 1)
				hourStr = "0" + hour.ToString();
			else
				hourStr = hour.ToString ();
		}
		
		string minStr = "00";
		int min = (second - hour*v)/100;
		if(min != 0)
		{
			if(min.ToString().Length == 1)
				minStr = "0" + min.ToString();
			else
				minStr = min.ToString ();
		}
		
		string secStr = "00";
		int sec = second - hour*v - min*100;
		if(sec != 0)
		{
			if(sec.ToString().Length == 1)
				secStr = "0" + sec.ToString();
			else
				secStr = sec.ToString ();
		}

		switch (strCount)
		{
		case 1:
			return secStr;

		case 2:
			return minStr + ":" + secStr;
		}
		return hourStr + ":" + minStr + ":" + secStr;
	}
}
