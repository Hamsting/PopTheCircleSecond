using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.OldNoteDataConverter
{
	[System.Serializable]
	public class NoteData
	{
		public NoteType NoteDataType
		{
			get
			{
				if (isPop)
					return NoteType.Pop;
				else if (isSpace)
					return NoteType.Space;
				else if (length > 0)
					return NoteType.Long;
				else
					return NoteType.Normal;
			}
		}
		public float Time
		{
			get
			{
				return (float)(timeMillis * 0.001f);
			}
		}
		public float LongEndTime
		{
			get
			{
				return (float)((timeMillis + length) * 0.001f);
			}
		}

		public int timeMillis = 0;
		public int line = 0;
		public int endLine = 0;
		public int length = 0;
		public bool isPop = false;
		public bool isSpace = false;



		public string GetNoteDataString()
		{
			switch (NoteDataType)
			{
				default:
				case NoteType.Normal:
					return "NormalNote | " + (line + 1) + " | " + Time;
				case NoteType.Pop:
					return "PopNote | " + (line + 1) + " | " + Time;
				case NoteType.Long:
					return "LongNote | " + (line + 1) + " | " + Time + " | " + LongEndTime + " | " + (endLine + 1);
				case NoteType.Space:
					return "SpaceNote | " + Time + " | " + LongEndTime;
			}
		}
	}

	public enum NoteType
	{
		Normal,
		Pop,
		Long,
		Space,
	}
}