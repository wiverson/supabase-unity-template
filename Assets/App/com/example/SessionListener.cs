using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using TMPro;
using UnityEngine;
namespace com.example
{
	public class SessionListener : MonoBehaviour
	{
		public SupabaseManager SupabaseManager = null!;

		public TMP_Text LoggedInEmailAddress = null!;

		public void UnityAuthListener(IGotrueClient<User, Session> sender, Constants.AuthState newState)
		{
			if (sender.CurrentUser?.Email == null)
				LoggedInEmailAddress.text = "No user logged in";
			else
			{
				LoggedInEmailAddress.text = $"Logged in as {sender.CurrentUser.Email}";
			}

			switch (newState)
			{
				case Constants.AuthState.SignedIn:
					// ReSharper disable once Unity.PerformanceCriticalCodeInvocation
					Debug.Log("Signed In");
					break;
				case Constants.AuthState.SignedOut:
					// ReSharper disable once Unity.PerformanceCriticalCodeInvocation
					Debug.Log("Signed Out");
					break;
				case Constants.AuthState.UserUpdated:
					// ReSharper disable once Unity.PerformanceCriticalCodeInvocation
					Debug.Log("Signed In");
					break;
				case Constants.AuthState.PasswordRecovery:
					// ReSharper disable once Unity.PerformanceCriticalCodeInvocation
					Debug.Log("Password Recovery");
					break;
				case Constants.AuthState.TokenRefreshed:
					Debug.Log("Token Refreshed");
					break;
				case Constants.AuthState.Shutdown:
					Debug.Log("Shutting Down");
					break;
				default:
					Debug.Log($"Unknown Auth State {nameof(newState)}");
					break;
			}
		}
	}
}
