using CsvHelper.Configuration.Attributes;

namespace EvaluationsTests;

record Data([Index(0)] double x, [Index(1)] double y, [Index(2)] string id);