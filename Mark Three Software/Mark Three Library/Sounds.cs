using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MarkThree
{

	/// <summary>
	/// Emits sounds to give audible feedback.  This class is a hack that was added to the project when Microsoft
	/// didn't get the system sounds into the final release.
	/// </summary>
	public class Sounds
	{

		/// <summary>
		/// A System Sound
		/// </summary>
		public const int MB_OK = 0x00000000;
		/// <summary>
		/// A System Sound
		/// </summary>
		public const int MB_ICONHAND = 0x00000010;
		/// <summary>
		/// A System Sound
		/// </summary>
		public const int MB_ICONQUESTION = 0x00000020;
		/// <summary>
		/// A System Sound
		/// </summary>
		public const int MB_ICONEXCLAMATION = 0x00000030;
		/// <summary>
		/// A System Sound
		/// </summary>
		public const int MB_ICONASTERISK = 0x00000040;

		[DllImport("USER32.DLL", EntryPoint="MessageBeep", SetLastError=true,
				CallingConvention=CallingConvention.StdCall)]
		private static extern int MessageBeep(uint uType);
		[DllImport("winmm.dll")]
		private static extern bool PlaySound(string filename, long hmodule, int dword);

		/// <summary>
		/// Play a system sound
		/// </summary>
		/// <param name="sound"></param>
		public static void PlaySound(uint sound)
		{
			
			try
			{

				// Play the sound from the operating system.
				MessageBeep(sound);

			}
			catch (Exception exception)
			{

				// Throw any error messages out to the debug listener.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			
		
		}

		/// <summary>
		/// Play the sound specified by the .WAV file sent as a parameter.
		/// </summary>
		/// <param name="filename">The name of the .WAV file to play.</param>
		public static void PlaySound(string filename)
		{
			
			try
			{

				// Play the sound from the filename.
				PlaySound(filename, 0L, 0x0001 | 0x00020000);

			}
			catch (Exception exception)
			{

				// Throw any error messages out to the debug listener.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

		}
	
	}
}
