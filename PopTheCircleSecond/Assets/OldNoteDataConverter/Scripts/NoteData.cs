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
					return NoteType.PopNote;
				else if (isSpace)
					return NoteType.SpaceNote;
				else if (length > 0)
					return NoteType.LongNote;
				else
					return NoteType.NormalNote;
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
				case NoteType.NormalNote:
					return "NormalNote | " + (line + 1) + " | " + Time;
				case NoteType.PopNote:
					return "PopNote | " + (line + 1) + " | " + Time;
				case NoteType.LongNote:
					return "LongNote | " + (line + 1) + " | " + Time + " | " + LongEndTime + " | " + (endLine + 1);
				case NoteType.SpaceNote:
					return "SpaceNote | " + Time + " | " + LongEndTime;
			}
		}
	}

	public enum NoteType
	{
		NormalNote,
		PopNote,
		LongNote,
		SpaceNote,
	}
}