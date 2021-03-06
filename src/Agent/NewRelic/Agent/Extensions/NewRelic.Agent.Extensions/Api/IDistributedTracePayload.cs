/*
* Copyright 2020 New Relic Corporation. All rights reserved.
* SPDX-License-Identifier: Apache-2.0
*/
namespace NewRelic.Agent.Api
{
    public interface IDistributedTracePayload
    {
        string HttpSafe();

        string Text();

        bool IsEmpty();
    }
}
