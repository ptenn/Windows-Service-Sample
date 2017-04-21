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
                                            logger.Debug(String.Format("Successfully deleted file '{0}'.", fileName));
                                        }
                                        catch (Exception fileEx)
                                        {
                                            logger.Debug(String.Format("Exception occurred attempting to delete file '{0}'.", fileName), fileEx);
                                        }

                                    }
                                    else
                                    {
                                        logger.Debug(String.Format("Do not have permissions to delete file '{0}'", folder.Path));
                                    }

                                }

                            }
                            // Handle deleting subdirectories (if configured to do so).
                            if (folder.DeleteSubFolders)
                            {
                                logger.Debug("Configured to delete subfolders.");
                                String[] directoryNames = Directory.GetDirectories(folder.Path);
                                foreach (String directoryName in directoryNames)
                                {
                                    if (DateTime.Compare(DateTime.Now,
                                            File.GetCreationTime(directoryName).AddDays(folder.CleanupDays).AddHours(folder.CleanupHours)) > 0)
                                    {
                                        logger.Debug(String.Format(
                                            "Found subdirectory '{0}' older than {1} days, {2} hours, checking file permissions.",
                                            directoryName, folder.CleanupDays, folder.CleanupHours));
                                        UserFileAccessRights userFileAccessRights = new UserFileAccessRights(folder.Path);

                                        if (userFileAccessRights.canDeleteSubdirectoriesAndFiles())
                                        {
                                            logger.Debug(String.Format("Verified permissions to delete directory '{0}' and subdirectories, attempting delete.", directoryName));
                                            try
                                            {
                                                Directory.Delete(directoryName, true);
                                                logger.Debug(String.Format("Successfully deleted directory '{0}' and subdirectories.", directoryName));
                                            }
                                            catch (Exception dirEx)
                                            {
                                                logger.Debug(String.Format("Exception occurred attempting to delete directory '{0}' and subdirectories.", directoryName), dirEx);
                                            }

                                        }
                                        else
                                        {
                                            logger.Debug(String.Format("Do not have permissions to delete files and subdirectories in folder '{0}'", folder.Path));
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            logger.Warn(String.Format("Attmpted to check Folder {0}, but path '{1}' does not exist",
                                folder.Name, folder.Path));
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
