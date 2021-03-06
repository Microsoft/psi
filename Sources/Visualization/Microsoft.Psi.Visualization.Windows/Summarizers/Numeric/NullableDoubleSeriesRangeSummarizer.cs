﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.Psi.Visualization.Summarizers
{
    using System.Collections.Generic;
    using Microsoft.Psi.Visualization.Data;

    /// <summary>
    /// Represents a range summarizer that performs interval-based data summarization over a series of nullable double values.
    /// </summary>
    [Summarizer]
    public class NullableDoubleSeriesRangeSummarizer : Summarizer<Dictionary<string, double?>, Dictionary<string, double?>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableDoubleSeriesRangeSummarizer"/> class.
        /// </summary>
        public NullableDoubleSeriesRangeSummarizer()
            : base(NumericSeriesRangeSummarizer.SeriesSummarizer, NumericSeriesRangeSummarizer.SeriesCombiner)
        {
        }
    }
}
