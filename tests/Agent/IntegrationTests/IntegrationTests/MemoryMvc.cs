/*
* Copyright 2020 New Relic Corporation. All rights reserved.
* SPDX-License-Identifier: Apache-2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NewRelic.Agent.IntegrationTestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace NewRelic.Agent.IntegrationTests
{
    [NetFrameworkTest]
    public class MemoryMvc : IClassFixture<RemoteServiceFixtures.BasicMvcApplicationTestFixture>
    {
        private readonly RemoteServiceFixtures.BasicMvcApplicationTestFixture _fixture;

        public MemoryMvc(RemoteServiceFixtures.BasicMvcApplicationTestFixture fixture, ITestOutputHelper testLogger)
        {
            _fixture = fixture;
            _fixture.TestLogger = testLogger;
            _fixture.Actions
            (
                setupConfiguration: () =>
                {
                    var configModifier = new NewRelicConfigModifier(_fixture.DestinationNewRelicConfigFilePath);
                },
                exerciseApplication: () =>
                {
                    _fixture.Get();
                    var startTime = DateTime.Now;
                    while (DateTime.Now <= startTime.AddSeconds(60))
                    {
                        if (_fixture.AgentLog.GetMetrics().Any(metric => metric.MetricSpec.Name == "Memory/Physical"))
                            break;
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            );
            _fixture.Initialize();
        }

        [Fact]
        public void Test()
        {
            var expectedMetrics = new List<Assertions.ExpectedMetric>
            {
                new Assertions.ExpectedMetric {metricName = @"Memory/Physical"}
            };

            var metrics = _fixture.AgentLog.GetMetrics().ToList();

            Assertions.MetricsExist(expectedMetrics, metrics);
        }
    }
}
