using UnityEngine;

namespace Common.Helpers.PlayerPreferencesManager
{
    public class PlayerPrefsManager
    {
        private const string TextLanguage = "TextLanguage";
        private const string VoiceLanguage = "VoiceLanguage";
        private const string MusicVolume = "MusicVolume";
        private const string SoundEffectsVolume = "SoundEffectsVolume";
        private const string VoiceVolume = "VoiceVolume";

        public static string SavedTextLanguage
        {
            get => PlayerPrefs.GetString(TextLanguage, string.Empty);
            set => PlayerPrefs.SetString(TextLanguage, value);
        }
        
        public static string SavedVoiceLanguage
        {
            get => PlayerPrefs.GetString(VoiceLanguage, string.Empty);
            set => PlayerPrefs.SetString(VoiceLanguage, value);
        }
        
        public static float SavedMusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicVolume, 50);
            set => PlayerPrefs.SetFloat(MusicVolume, value);
        }
        
        public static float SavedSoundEffectsVolume
        {
            get => PlayerPrefs.GetFloat(SoundEffectsVolume, 50);
            set => PlayerPrefs.SetFloat(SoundEffectsVolume, value);
        }
        
        public static float SavedVoiceVolume
        {
            get => PlayerPrefs.GetFloat(VoiceVolume, 50);
            set => PlayerPrefs.SetFloat(VoiceVolume, value);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}