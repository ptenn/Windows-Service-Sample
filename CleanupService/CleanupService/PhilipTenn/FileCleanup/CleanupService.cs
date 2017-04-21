using System;
using System.Configuration;
using System.ServiceProcess;
using log4net;
using PhilipTenn.FileCleanup.Jobs;
using PhilipTenn.FileCleanup.Settings;
using Quartz;
using Quartz.Impl;

namespace PhilipTenn.FileCleanup
{
    public partial class CleanupService : ServiceBase
    {

        private static ILog logger = LogManager.GetLogger(typeof(CleanupService));

        protected IScheduler Scheduler { get; set; }


        public CleanupService()
        {
            InitializeComponent();
        }


//        protected override void OnStart(string[] args)
//        {
//            logger.Info("CleanupService OnStart called");
//            // construct a scheduler factory
//            ISchedulerFactory schedFact = new StdSchedulerFactory();
//
//            // get a scheduler.  We will keep a reference to this scheduler for graceful shutdown when the Windows Service is stopped.
//            Scheduler = schedFact.GetScheduler();
//            Scheduler.Start();
//
//            // construct job info
//            JobDetail jobDetail = new JobDetail("fileCleanupJob", null, typeof(FileCleanupJob));
//
//            // Get the MonitorFolderSection 
//            MonitorFolderSection monitorFolderSection = ConfigurationManager.GetSection("monitorFolders") as MonitorFolderSection;
//
//            // Create Trigger.  Default is daily that fires at midnight
//            Trigger trigger = TriggerUtils.MakeDailyTrigger(0, 0);
//            trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
//
//            if (monitorFolderSection != null)
//            {
//                switch (monitorFolderSection.Frequency)
//                {
//                    case "hourly":
//                        logger.Info("Quartz.net Trigger set to run hourly.");
//                        trigger = TriggerUtils.MakeHourlyTrigger();
//                        trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
//                        break;
//                    case "minutely":
//                        logger.Info("Quartz.net Trigger set to run minutely.");
//                        trigger = TriggerUtils.MakeMinutelyTrigger();
//                        trigger.StartTimeUtc = TriggerUtils.GetEvenMinuteDate(DateTime.UtcNow);
//                        break;
//                    default:
//                        logger.Info("Quartz.net Trigger set to run daily at midnight.");
//                        trigger = TriggerUtils.MakeDailyTrigger(0, 0);
//                        trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
//                        break;
//
//                }
//            }
//
//            // Name Trigger and start 
//            trigger.Name = "fileCleanupTrigger";
//            Scheduler.ScheduleJob(jobDetail, trigger);
//        }
//
        protected override void OnStart(string[] args)
        {
            logger.Info("FileCleanupWinService OnStart called");
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler.  We will keep a reference to this scheduler for graceful shutdown when the Windows Service is stopped.
            Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            // construct job info
            IJobDetail jobDetail = new JobDetailImpl("fileCleanupJob", null, typeof(FileCleanupJob));

            // Get the MonitorFolderSection 
            MonitorFolderSection monitorFolderSection = ConfigurationManager.GetSection("monitorFolders") as MonitorFolderSection;


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

            // Name Trigger and start 
            Scheduler.ScheduleJob(job, trigger);
        }

        protected override void OnStop()
        {
            logger.Info("CleanupService OnStop called");
            if (Scheduler != null)
            {
                logger.Debug("Attempting to shutdown Scheduler, will wait for all jobs to complete.");
                Scheduler.Shutdown(true);
                logger.Debug("Scheduler has successfully shutdown.");
            }
        }

        
    }
}
