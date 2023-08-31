using System;
using System.Diagnostics.CodeAnalysis;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using UnityEngine;
namespace com.example
{
	public class SignInWithEmailPassword : MonoBehaviour
	{
		// Public Unity References
		public TMP_InputField EmailInput = null!;
		public TMP_InputField PasswordInput = null!;
		public TMP_Text ErrorText = null!;
		public SupabaseManager SupabaseManager = null!;

		// Private implementation
		private bool _doSignIn;
		private bool _doSignOut;

		public void SignIn()
		{
			_doSignIn = true;
		}

		public void SignOut()
		{
			_doSignOut = true;
		}

		[SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
		private async void Update()
		{
			if (_doSignOut)
			{
				await SupabaseManager.Supabase()!.Auth.SignOut();
				_doSignOut = false;
			}

			if (!_doSignIn) return;

			_doSignIn = false;
			try
			{
				Session session = (await SupabaseManager.Supabase()!.Auth.SignUp(EmailInput.text, PasswordInput.text))!;
				ErrorText.text = $"Success! Signed Up as {session.User?.Email}";
			}
			catch (GotrueException gotrueException)
			{
				ErrorText.text = $"{gotrueException.Reason} {gotrueException.Message}";
				Debug.Log(gotrueException.Message, gameObject);
				Debug.LogException(gotrueException, gameObject);
			}
			catch (Exception e)
			{
				Debug.Log(e.Message, gameObject);
				Debug.Log(e, gameObject);
			}
		}

	}
}
