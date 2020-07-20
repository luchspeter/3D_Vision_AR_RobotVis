﻿/*
© HoloLab Inc., 2019
Author: Yusuke Furuta (furuta@hololab.co.jp)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.Messages.TF2
{
    public class TFMessage : Message
    {
        public Geometry.TransformStamped[] transforms;

        public TFMessage()
        {
            transforms = new Geometry.TransformStamped[0]; RosMessageName = "tf2_msgs/TFMessage";
        }
    }
}