﻿namespace Subnautica.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ZeroLanguage
    {
        private static IDictionary<string, string> LanguageData { get; set; } = new Dictionary<string, string>()
        {
            { "APP_BOOT_CREATE_FILES"     , "Generating boot files..." },
            { "APP_LANGUAGE_READING_ERROR", "The language file could not be read." }
        };

        public static bool LoadLanguage(string language, bool forceDownload = false)
        {
            try
            {
                string languageFile = Paths.GetLauncherLanguageFile(language);

                Log.Info($"Loading Language: {language}, Path: {languageFile}");

                if (File.Exists(languageFile) && !forceDownload)
                {
                    LanguageData = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, string>>(File.ReadAllText(languageFile));
                    return true;
                }

                string languageUrl = Paths.GetLauncherLanguageUrl(language);
                if (!FileDownloader.DownloadFile(languageUrl, languageFile))
                {
                    return false;
                }

                if (!File.Exists(languageFile))
                {
                    return false;
                }

                return LoadLanguage(language);
            }
            catch (Exception e)
            {
                Log.Error($"Language.LoadLanguage: {e}");
                return false;
            }
        }

        public static string Get(string languageKey)
        {
            if (LanguageData == null)
            {
                return languageKey;
            }

            if (LanguageData.TryGetValue(languageKey, out string text))
            {
                return text;
            }

            return languageKey;
        }

        public static string LauncherUpdateContentMessage(string newVersion)
        {
            return Get("NEW_UPDATE_CONTENT").Replace("{nowversion}", Tools.GetLauncherVersion(true, true)).Replace("{newversion}", "v" + newVersion);
        }

        public static string LauncherDownloadMessage(string version)
        {
            return Get("APP_LAUNCHER_DOWNLOADING").Replace("{version}", version);
        }

        public static string LauncherInstallMessage(string version)
        {
            return Get("APP_LAUNCHER_INSTALLING").Replace("{version}", version);
        }

        public static string LauncherDownloading(string name)
        {
            return Get("APP_DOWNLOADING_FILE_CONTENT").Replace("{name}", name);
        }

        public static string GetServerPlayerCount()
        {
            return string.Format("{0} ({1}/{2})", ZeroPlayer.CurrentPlayer.NickName, ZeroPlayer.GetAllPlayers().Count, Network.Session.Current.MaxPlayerCount);
        }

        public static string GetStoryWaitingPlayers(byte playerCount, byte maxPlayer)
        {
            return Get("GAME_STORY_WAITING_PLAYERS").Replace("{playerCount}", playerCount.ToString()).Replace("{maxPlayer}", maxPlayer.ToString());
        }
    }
}