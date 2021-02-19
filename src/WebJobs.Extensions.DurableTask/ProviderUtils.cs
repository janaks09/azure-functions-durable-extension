﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
#if !FUNCTIONS_V1
using Microsoft.Azure.WebJobs.Host.Scale;
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    /// <summary>
    /// Provides access to internal functionality for the purpose of implementing durability providers.
    /// </summary>
    public static class ProviderUtils
    {
#if !FUNCTIONS_V1
        /// <summary>
        /// Providers can implement this interface to override the autoscaling.
        /// </summary>
        public interface IProviderWithAutoScaling
        {
            /// <summary>
            /// Tries to obtain a scale monitor for autoscaling.
            /// </summary>
            /// <param name="extension">The extension, which is used for tracing.</param>
            /// <param name="scaleMonitor">The scale monitor.</param>
            /// <returns>True if autoscaling is supported, false otherwise.</returns>
            bool TryGetScaleMonitor(DurableTaskExtension extension, out IScaleMonitor scaleMonitor);
        }
#endif

        /// <summary>
        /// Returns the instance id of the entity scheduler for a given entity id.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>The instance id of the scheduler.</returns>
        public static string GetSchedulerIdFromEntityId(EntityId entityId)
        {
            return EntityId.GetSchedulerIdFromEntityId(entityId);
        }

        /// <summary>
        /// Reads the state of an entity from the serialized entity scheduler state.
        /// </summary>
        /// <param name="state">The orchestration state of the scheduler.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="result">The serialized state of the entity.</param>
        /// <returns>true if the entity exists, false otherwise.</returns>
        public static bool TryGetEntityStateFromSerializedSchedulerState(OrchestrationState state, JsonSerializerSettings serializerSettings, out string result)
        {
            if (state != null
                && state.OrchestrationInstance != null
                && state.Input != null)
            {
                var schedulerState = JsonConvert.DeserializeObject<SchedulerState>(state.Input, serializerSettings);

                if (schedulerState.EntityExists)
                {
                    result = schedulerState.EntityState;
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Converts the DTFx representation of the orchestration state into the DF representation.
        /// </summary>
        /// <param name="orchestrationState">The orchestration state.</param>
        /// <returns>The orchestration status.</returns>
        public static DurableOrchestrationStatus ConvertOrchestrationStateToStatus(OrchestrationState orchestrationState)
        {
            return DurableClient.ConvertOrchestrationStateToStatus(orchestrationState);
        }

        /// <summary>
        /// Trace an informational event for the extension.
        /// </summary>
        /// <param name="extension">The durable task extension.</param>
        /// <param name="hubName">The name of the task hub.</param>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="message">The message.</param>
        /// <param name="writeToUserLogs">Whether to write this event to the user logs.</param>
        public static void TraceInformationalEvent(this DurableTaskExtension extension, string hubName, string instanceId, string functionName, string message, bool writeToUserLogs)
        {
            extension.TraceHelper.ExtensionInformationalEvent(hubName, instanceId, functionName, message, writeToUserLogs);
        }

        /// <summary>
        /// Trace a warning event for the extension.
        /// </summary>
        /// <param name="extension">The durable task extension.</param>
        /// <param name="hubName">The name of the task hub.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="message">The message.</param>
        public static void TraceWarningEvent(this DurableTaskExtension extension, string hubName, string functionName, string instanceId, string message)
        {
            extension.TraceHelper.ExtensionWarningEvent(hubName, functionName, instanceId, message);
        }
    }
}
