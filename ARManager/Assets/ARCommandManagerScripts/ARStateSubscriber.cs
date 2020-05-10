﻿using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class ARStateSubscriber : Subscriber<RosSharp.RosBridgeClient.Messages.Standard.String>
    {
        string lastMsg = "";
        public ARCommander ArCommander;

        public bool Enabled = true;
        public bool ForceUpdateOnce = false;

        protected override void ReceiveMessage(RosSharp.RosBridgeClient.Messages.Standard.String str)
        {
            if (!Enabled) return;

            //filter unchanged strings
            if (lastMsg == str.data && !ForceUpdateOnce)
                return;

            lastMsg = str.data;
            ForceUpdateOnce = false;

            ARStateRosMessage CurrentMessage = JsonConvert.DeserializeObject<ARStateRosMessage>(str.data);
            Debug.Log($"Received {CurrentMessage.data.Length} running unfiltered instances");

            if (ArCommander != null)
                MainThreadDispatcher.Instance.AddItem(() => ArCommander.ReceiveUnfilteredMessage(CurrentMessage.data));
            else
                Debug.LogError("AR Commander must be set!");
        }

        class ARStateRosMessage
        {
            public RunningInstance[] data;
        }
    }
}
