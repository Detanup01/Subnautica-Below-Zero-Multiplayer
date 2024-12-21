﻿namespace Subnautica.Client.MonoBehaviours.General
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Subnautica.API.Enums;
    using Subnautica.API.Features;
    using Subnautica.Client.Core;
    using Subnautica.Client.Modules;

    using UnityEngine;

    public class ConnectionSignals : MonoBehaviour
    {
        private static ConnectionSignals Instance = null;

        private WaitForSecondsRealtime UnscaledFixedRealTime { get; set; } = new WaitForSecondsRealtime(0.2f);

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            this.StartCoroutine(this.UnscaledFixedUpdate());
        }

        private IEnumerator UnscaledFixedUpdate()
        {
            while (true)
            {
                yield return this.UnscaledFixedRealTime;

                ConnectionSignals.ConsumeQueue();
            }
        }

        public static void ConsumeQueue()
        {
            try
            {
                while (NetworkClient.ConnectionSignalDataQueues.Count > 0)
                {
                    var result = NetworkClient.ConnectionSignalDataQueues.Dequeue();
                    if (result == ConnectionSignal.Connected)
                    {
                        ErrorMessages.ShowConnectionSuccess();
                    }
                    else if (result == ConnectionSignal.Rejected)
                    {
                        ErrorMessages.ShowConnectionRejected();
                    }
                    else
                    {
                        Log.Info("ConnectionSignal Detected: " + result);
                        if (result == ConnectionSignal.VersionMismatch)
                        {
                            ErrorMessages.ShowConnectionVersionMismatch();
                        }
                        else if (result == ConnectionSignal.ServerFull)
                        {
                            ErrorMessages.ShowConnectionServerFull();
                        }
                        else
                        {
                            ErrorMessages.ShowConnectionError();
                        }

                        NetworkClient.Disconnect();

                        ZeroGame.StopLoadingScreen();
                        ZeroGame.QuitToMainMenu();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"ConnectionSignals.Update: {e}");
            }
        }
    }
}
