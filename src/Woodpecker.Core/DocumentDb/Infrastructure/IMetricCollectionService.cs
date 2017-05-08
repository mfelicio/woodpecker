﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public interface IMetricCollectionService
    {
        Task<IEnumerable<MetricModel>> CollectMetrics(DateTime startTimeUtc, DateTime endTimeUtc, IMetricsInfo metricsInfo);
    }
}