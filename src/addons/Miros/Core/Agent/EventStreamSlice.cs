using System;

namespace Miros.Core;

public class EventStreamArgs(string eventStreamName)
{
    public string EventStreamName { get; } = eventStreamName;
}