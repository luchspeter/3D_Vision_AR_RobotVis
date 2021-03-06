﻿
using RosSharp.RosBridgeClient;
using RosSharp.Urdf;
using RosSharp.Urdf.Attachables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ARRobotInteraction.Base
{
    /// <summary>
    /// Handles the synchronization life-cycle of a Robot GameObject with its URDF Counterpart
    /// </summary>
    public class UrdfSyncher
    {

        #region Static Inits

        private static AttachableComponentFactory<IAttachableComponent> tooltipFactory;

        /// <summary>
        /// Initializes all static classes needed for the urdfSyncher to work
        /// </summary>
        public static void InitializeUrdfSyncher()
        {
            //Attach tooltip factory to the Robot Parser
            tooltipFactory = new AttachableComponentFactory<IAttachableComponent>("tooltip")
            {
                Constructor = () => new RosSharp.Urdf.Attachables.AttachedDataValue(),
            };

            Robot.attachableComponentFactories.Add(tooltipFactory);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneContainerObject">The gameobject which should be used to contain this robot</param>
        /// <param name="robotName">The name of the robot instance</param>
        /// <param name="recreateObjectAfterSynchronizations">A counter that specifies after how many urdf synchronizations a complete rebuild should be performed</param>
        public UrdfSyncher(GameObject sceneContainerObject, RosConnector rosConnector, string robotName, GameObject robotPrefab, int recreateObjectAfterSynchronizations = -1, bool useUnityPhysics = false)
        {
            RosConnector = rosConnector;
            SceneContainerGameObject = sceneContainerObject;
            synchronizationCounterMax = recreateObjectAfterSynchronizations;
            RobotName = robotName;
            RobotPrefab = robotPrefab;
            this.useUnityPhysics = useUnityPhysics;
        }

        public RosConnector RosConnector;

        /// <summary>
        /// The Root gameobject which will be used to spawn all robots. This will be the parent
        /// </summary>
        public GameObject SceneContainerGameObject;
        public GameObject RobotPrefab;

        public string RobotName;


        /// <summary>
        /// The root gameobject of this robot
        /// </summary>
        private GameObject RobotRootObject;

        private bool hasUrdfAssetsImported = false;

        private int synchronizationCounterWithoutFullRegeneration = 0;
        private int synchronizationCounterMax = -1;
        private bool useUnityPhysics;

        /// <summary>
        /// Compares the current devices in the scene with the urdf file and performs adjustments
        /// </summary>
        /// <param name="urdf"></param>
        public void SynchUrdf(string urdf)
        {
            ReportSynch();

            Robot robot = Robot.FromContent(urdf);

            foreach (var c in robot.attachedComponents)
            {
                if (c.component is AttachedDataValue av && String.IsNullOrEmpty(av.topic))
                {
                    av.topic = "/img/joint_position";
                }
            }


            if (!hasUrdfAssetsImported)
            {
                ImportInitialUrdfModel(robot);
                return;
            }

            //we do not return here since we want to apply any possible changes that were passed in the last urdf update.

            RobotBuilder builder = new RobotBuilder();
            builder.Synchronize(robot, RobotRootObject);

            synchronizationCounterWithoutFullRegeneration++;
        }

        /// <summary>
        /// Imports the URDF Model initially to download all textures, assets and store them correctly etc.
        /// </summary>
        /// <param name="robot"></param>
        private void ImportInitialUrdfModel(Robot robot)
        {
            hasUrdfAssetsImported = false;

            if (RobotRootObject == null)
            {
                RobotRootObject = GameObject.Instantiate(RobotPrefab);
                RobotRootObject.name = RobotName;
            }

            RobotBuilder builder = new RobotBuilder();
            builder.Synchronize(robot, RobotRootObject);

            if (!useUnityPhysics)
            {
                foreach (Rigidbody rb in RobotRootObject.GetComponentsInChildren<Rigidbody>())
                    rb.isKinematic = true;
            }

            //Attach a joint state subscriber that subscribes all joints to the state published by Gazebo or another source
            RosConnector.gameObject.AddComponentIfNotExists<JointStateSubscriber>(out var jointStateSubscriber);

            jointStateSubscriber.Topic = @"/joint_states";
            jointStateSubscriber.JointStateWriters = new List<JointStateWriter>();
            jointStateSubscriber.JointNames = new List<string>();
            foreach (UrdfJoint urdfJoint in RobotRootObject.GetComponentsInChildren<UrdfJoint>())
            {
                if (urdfJoint.JointType != UrdfJoint.JointTypes.Fixed)
                {
                    urdfJoint.gameObject.AddComponentIfNotExists<JointStateWriter>(out var writer);
                    jointStateSubscriber.JointStateWriters.Add(writer);
                    jointStateSubscriber.JointNames.Add(urdfJoint.JointName);
                }
            }

            if (SceneContainerGameObject != null)
            {
                RobotRootObject.transform.SetParent(SceneContainerGameObject.transform);
            }

            hasUrdfAssetsImported = true;
        }

        /// <summary>
        /// If drift errors occurr in the system (e.g. by Unity physics etc.) this method forces a re-import after a number of updates
        /// </summary>
        private void ReportSynch()
        {
            if (synchronizationCounterMax == -1) return;

            if (synchronizationCounterWithoutFullRegeneration == synchronizationCounterMax)
            {
                GameObject.Destroy(RobotRootObject);
                RobotRootObject = null;
                synchronizationCounterWithoutFullRegeneration = 0;
            }
        }


        /// <summary>
        /// Destroys everything managed by this instance
        /// </summary>
        internal void Destroy()
        {
            GameObject.Destroy(RobotRootObject);
        }
    }
}