using CsvHelper.Configuration.Attributes;

namespace UpdatedEvaluations;

record Data([Index(0)] double x, [Index(1)] double y);

record DataResults([Index(0)] double x, [Index(1)] double y, [Index(2)] int cluster);