using System;
using System.IO;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
namespace com.example
{
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
				// NotificationManager.PostMessage(NotificationManager.NotificationType.Debug,
				// 	"Session Saved");
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
				// NotificationManager.PostMessage(NotificationManager.NotificationType.Debug,
				// 	"Session Deleted");
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
