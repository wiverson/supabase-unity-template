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
			// We set an option to refresh the token automatically using a background thread.
			options.AutoRefreshToken = true;

			// We start setting up the client here
			Client client = new(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);

			// The first thing we do is attach the debug listener
			client.Auth.AddDebugListener(DebugListener!);

			// Next we set up the network status listener and tell it to turn the client online/offline
			_networkStatus.Client = (Supabase.Gotrue.Client)client.Auth;

			// Next we set up the session persistence - without this the client will forget the session
			// each time the app is restarted
			client.Auth.SetPersistence(new UnitySession());

			// This will be called whenever the session changes
			client.Auth.AddStateChangedListener(SessionListener.UnityAuthListener);

			// Fetch the session from the persistence layer
			// If there is a valid/unexpired session available this counts as a user log in
			// and will send an event to the UnityAuthListener above.
			client.Auth.LoadSession();

			// Allow unconfirmed user sessions. If you turn this on you will have to complete the
			// email verification flow before you can use the session.
			client.Auth.Options.AllowUnconfirmedUserSessions = true;

			// We check the network status to see if we are online or offline using a request to fetch
			// the server settings from our project. Here's how we build that URL.
			string url = $"{SupabaseSettings.SupabaseURL}/auth/v1/settings?apikey={SupabaseSettings.SupabaseAnonKey}";
			try
			{
				// This will get the current network status
				client.Auth.Online = await _networkStatus.StartAsync(url);
			}
			catch (NotSupportedException)
			{
				// Some platforms don't support network status checks, so we just assume we are online
				client.Auth.Online = true;
			}
			catch (Exception e)
			{
				// Something else went wrong, so we assume we are offline
				ErrorText.text = e.Message;
				Debug.Log(e.Message, gameObject);
				Debug.LogException(e, gameObject);

				client.Auth.Online = false;
			}
			if (client.Auth.Online)
			{
				// Now we start up the client, which will in turn start up the background thread.
				// This will attempt to refresh the session token, which in turn may send a second
				// user login event to the UnityAuthListener.
				await client.InitializeAsync();

				// Here we fetch the server settings and log them to the console
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

		// This is called when Unity shuts down. You want to be sure to include this so that the
		// background thread is terminated cleanly. Keep in mind that if you are running the app
		// in the Unity Editor, if you don't call this method you will leak the background thread!
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
