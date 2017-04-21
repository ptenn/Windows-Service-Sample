# Windows-Service-Sample

Sample Windows Service Application that will clean up files and directories based on a configurable age.

This sample project requires the following to build:

* Microsoft .NET 4.5.2
* C# 6.0 / Roslyn Compiler (for String interpolation)
* NuGet

The following dependencies are specified in NuGet `packages.config`:

* Common Logging 3.3.1 
* Common Logging Core 3.3.1 
* log4net 2.0.8
* Quartz 2.5.0

The Quartz Job and Trigger is configured using Quartz's Builders, which resemble a DSL for Jobs:

    // Create Job
    IJobDetail job =
        JobBuilder.Create<FileCleanupJob>()
            .WithIdentity("CleanupFilesJob", "CleanupFilesJobGroup")
            .Build();

    // Create Trigger that fires every minute
    ISimpleTrigger trigger =
        (ISimpleTrigger) TriggerBuilder.Create()
            .WithIdentity("CleanupFilesTrigger", "CleanupFilesTriggerGroup")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever())
            .Build();

I created an external XML file for Quartz Jobs and Triggers in the repository `quartzJobsAndTriggers.xml`, but could not figure out how to get that mechanism wired up correctly.
