using Microsoft.Win32.TaskScheduler;
using System;

namespace Agent
{
    public class TaskScheduler
    {
        public bool CreateTask(String taskName, String execCommand, String execCommandArg, String execCommandWorkDir,  int startAtHour, int rndMinutes)
        {
        
            // String taskName = "SWCTRL";
            //     String execCommand = "winagent.exe";
            //     String execCommandArg = "send";
            //     String execCommandWorkDir = "c:\\windows";
            //     int startAtHour = 10;
            rndMinutes = new Random().Next(rndMinutes);
            Console.WriteLine("Start creating Task (" + execCommand + ") start at "+startAtHour+":"+rndMinutes);
            using (TaskService ts = new TaskService())
            {
                Microsoft.Win32.TaskScheduler.Task task = ts.GetTask(taskName);
                if (task != null)
                {
                    ts.RootFolder.DeleteTask(taskName);
                }

                TaskDefinition td = ts.NewTask();

                td.Triggers.Add(new DailyTrigger { 
                    DaysInterval = 1,
                    StartBoundary = DateTime.Today + TimeSpan.FromHours(startAtHour) + TimeSpan.FromMinutes(rndMinutes)
                });
                td.Actions.Add(new ExecAction(execCommand, execCommandArg, execCommandWorkDir));
                td.Principal.Id = "NT AUTHORITY\\SYSTEM";
                td.Principal.LogonType = TaskLogonType.ServiceAccount;
                td.Principal.RunLevel = TaskRunLevel.LUA;
                td.Principal.UserId = "NT AUTHORITY\\SYSTEM";
                td.Settings.AllowDemandStart = true;
                td.Settings.AllowHardTerminate = false;
                td.Settings.DeleteExpiredTaskAfter = new TimeSpan();
                td.Settings.ExecutionTimeLimit = new TimeSpan();
                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.IdleSettings.StopOnIdleEnd = false;
                td.Settings.StartWhenAvailable = true;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.Enabled = true;
                td.RegistrationInfo.Author = "auriga.corp";
                

                ts.RootFolder.RegisterTaskDefinition(@taskName, td);
                                
            }
        
        Console.WriteLine("End creating Task");
            return true;
        }
    }
}