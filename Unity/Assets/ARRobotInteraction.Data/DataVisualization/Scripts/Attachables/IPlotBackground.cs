﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace ARRobotInteraction.Data
{
    public interface IPlotBackground
    {
        bool IsVisible { set; }

        void OnContentChange(Vector3 localContentSize, Vector3 localContentOffset, Transform contentParentTransform);
    }
}