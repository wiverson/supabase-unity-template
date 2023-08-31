using UnityEngine;

namespace com.example
{
	[CreateAssetMenu(fileName = "Supabase", menuName = "Supabase/Supabase Settings", order = 1)]
	public class SupabaseSettings : ScriptableObject
	{
		public string SupabaseURL = null!;
		public string SupabaseAnonKey = null!;
	}
}
