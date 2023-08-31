using System;
using System.IO;
using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using UnityEngine;
namespace com.example
{
	/// <summary>
	/// This is a simple implementation of IGotrueSessionPersistence that uses Unity's
	/// Application.persistentDataPath to store the session.
	///
	/// This implementation works on iOS, Android, as well as desktop platforms and the Unity Editor.
	///
	/// Because this implementation uses the .NET System.IO packages, it will work even if SaveSession is called
	/// by the refresh background thread.
	///
	/// Don't use the Unity PlayerPrefs to store the session - you can only use PlayerPrefs from the main UI thread,
	/// and this will break the refresh background thread.
	/// </summary>
	public class UnitySession : IGotrueSessionPersistence<Session>
	{
		private static string FilePath()
		{
			const string cacheFileName = "gotrue.cache";
			string filePath = Path.Join(Application.persistentDataPath, cacheFileName);
			return filePath;
		}

		public void SaveSession(Session? session)
		{
			if (session == null)
			{
				DestroySession();
				return;
			}

			try
			{
				string filePath = FilePath();
				string str = JsonConvert.SerializeObject(session);
				using StreamWriter file = new(filePath);
				file.Write(str);
				file.Dispose();
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
				Debug.LogException(e);
				throw;
			}
		}

		public void DestroySession()
		{
			string filePath = FilePath();
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public Session? LoadSession()
		{
			string filePath = FilePath();

			if (!File.Exists(filePath)) return null;

			using StreamReader file = new(filePath);
			string sessionJson = file.ReadToEnd();

			if (string.IsNullOrEmpty(sessionJson))
			{
				return null;
			}

			try
			{
				return JsonConvert.DeserializeObject<Session>(sessionJson);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
				Debug.LogException(e);
				return null;
			}
		}
	}
}
