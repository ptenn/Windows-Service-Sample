using System;
using System.Configuration;
using System.IO;

using log4net;
using Quartz;

using PhilipTenn.FileCleanup.Settings;
using PhilipTenn.FileCleanup.Util;

namespace PhilipTenn.FileCleanup.Jobs
{
    /// <summary>
    /// This Job will, when executed, handle checking each monitored Folder for files older than the number of cleanupDays.  If the file is older,
    /// it is deleted.
    /// </summary>
    public class FileCleanupJob : IJob
    {
        /// <summary>
        /// Log4Net logger set up.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(FileCleanupJob));

        /// <summary>
        /// Implementation of the Job Execute method.
        /// </summary>
        /// <param name="context">Reference to the JobExecutionContext.</param>
        public void Execute(IJobExecutionContext context)
        {
            logger.Info("Executing FileCleanupJob");
            try
            {
                // Get the MonitorFolderSection 
                MonitorFolderSection monitorFolderSection =
                    ConfigurationManager.GetSection("monitorFolders") as MonitorFolderSection;

                if (monitorFolderSection == null)
                {
                    logger.Error("Failed to load MonitorFolderSection.");
                }
                else
                {
                    foreach (FolderConfigElement folder in monitorFolderSection.Folders)
                    {
                        logger.Info($"Checking Folder {folder.Name}, Path '{folder.Path}' for any files older than {folder.CleanupDays} days, {folder.CleanupHours} hours.");


                        if (Directory.Exists(folder.Path))
                        {
                            // Handle files
                            String[] fileNames = Directory.GetFiles(folder.Path);
                            foreach (string fileName in fileNames)
                            {
                                if (DateTime.Compare(DateTime.Now,
                                        File.GetCreationTime(fileName).AddDays(folder.CleanupDays).AddHours(folder.CleanupHours)) > 0)
                                {
                                    logger.Debug($"Found file '{fileName}' older than {folder.CleanupDays} days, {folder.CleanupHours} hours, checking file permissions.");
                                    UserFileAccessRights userFileAccessRights = new UserFileAccessRights(folder.Path);

                                    if (userFileAccessRights.canDelete())
                                    {
                                        logger.Debug($"Verified permissions to delete file '{fileName}', attempting delete.");
                                        try
                                        {
                                            File.Delete(fileName);
                                            logger.Debug($"Successfully deleted file '{fileName}'.");
                                        }
                                        catch (Exception fileEx)
                                        {
                                            logger.Debug($"Exception occurred attempting to delete file '{fileName}'.", fileEx);
                                        }

                                    }
                                    else
                                    {
                                        logger.Debug($"Do not have permissions to delete file '{folder.Path}'");
                                    }

                                }

                            }
                            // Handle deleting subdirectories (if configured to do so).
                            if (folder.DeleteSubFolders)
                            {
                                logger.Debug("Configured to delete subfolders.");
                                string[] directoryNames = Directory.GetDirectories(folder.Path);
                                foreach (string directoryName in directoryNames)
                                {
                                    if (DateTime.Compare(DateTime.Now,
                                            File.GetCreationTime(directoryName).AddDays(folder.CleanupDays).AddHours(folder.CleanupHours)) > 0)
                                    {
                                        logger.Debug($"Found subdirectory '{directoryName}' older than {folder.CleanupDays} days, {folder.CleanupHours} hours, checking file permissions.");
                                        UserFileAccessRights userFileAccessRights = new UserFileAccessRights(folder.Path);

                                        if (userFileAccessRights.canDeleteSubdirectoriesAndFiles())
                                        {
                                            logger.Debug($"Verified permissions to delete directory '{directoryName}' and subdirectories, attempting delete.");
                                            try
                                            {
                                                Directory.Delete(directoryName, true);
                                                logger.Debug($"Successfully deleted directory '{directoryName}' and subdirectories.");
                                            }
                                            catch (Exception dirEx)
                                            {
                                                logger.Debug($"Exception occurred attempting to delete directory '{directoryName}' and subdirectories.", dirEx);
                                            }

                                        }
                                        else
                                        {
                                            logger.Debug($"Do not have permissions to delete files and subdirectories in folder '{folder.Path}'");
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            logger.Warn($"Attmpted to check Folder {folder.Name}, but path '{folder.Path}' does not exist");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Attempt to run FileCleanupJob failed with exception.", ex);
            }
        }
    }
}
