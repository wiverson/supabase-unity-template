using System;
using Supabase;
using Supabase.Gotrue;
using TMPro;
using UnityEngine;
using Client = Supabase.Client;

namespace com.example
{
	public class SupabaseManager : MonoBehaviour
	{

		// Public Unity references
		public SessionListener SessionListener = null!;
		public SupabaseSettings SupabaseSettings = null!;

		public TMP_Text ErrorText = null!;
		
		// Public in case other components are interested in network status
		private readonly NetworkStatus _networkStatus = new();

		// Internals
		private Client? _client;

		public Client? Supabase() => _client;

		private async void Start()
		{
			SupabaseOptions options = new();
			options.AutoRefreshToken = true;

			Client client = new(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
			client.Auth.AddDebugListener(DebugListener!);

			_networkStatus.Client = (Supabase.Gotrue.Client)client.Auth;

			client.Auth.SetPersistence(new UnitySession());
			client.Auth.AddStateChangedListener(SessionListener.UnityAuthListener);
			client.Auth.LoadSession();
			client.Auth.Options.AllowUnconfirmedUserSessions = true;

			string url = $"{SupabaseSettings.SupabaseURL}/auth/v1/settings?apikey={SupabaseSettings.SupabaseAnonKey}";
			try
			{
				client.Auth.Online = await _networkStatus.StartAsync(url);
			}
			catch (NotSupportedException)
			{
				client.Auth.Online = true;
			}
			catch (Exception e)
			{
				ErrorText.text = e.Message;
				Debug.Log(e.Message, gameObject);
				Debug.LogException(e, gameObject);

				client.Auth.Online = false;
			}
			if (client.Auth.Online)
			{
				await client.InitializeAsync();
				Settings serverConfiguration = (await client.Auth.Settings())!;
				Debug.Log($"Auto-confirm emails on this server: {serverConfiguration.MailerAutoConfirm}");
			}
			_client = client;
		}

		private void DebugListener(string message, Exception e)
		{
			ErrorText.text = message;
			Debug.Log(message, gameObject);
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (e != null)
				Debug.LogException(e, gameObject);
		}

		private void OnApplicationQuit()
		{
			if (_client != null)
			{
				_client?.Auth.Shutdown();
				_client = null;
			}
		}
	}
}
