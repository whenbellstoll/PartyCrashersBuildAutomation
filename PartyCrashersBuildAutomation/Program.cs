using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


namespace PartyCrashersBuildAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            // check directory
            // if directory doesn't exis, create
            // clone repos
            // checkout Normcore

            int buildNumber = 1;
            while(running)
            {
                Console.WriteLine("Party Crashers Automated Build System - Start Loop");
                Console.WriteLine();
                // wait 5 minutes
                System.Threading.Thread.Sleep(200000);
                // pull git
                string uptodate = RunGitCommand("pull", "", "C:/PartyCrashersBuildAutomation/Funky-Virtual-Party");
                

                // are there changes?
                if ( !string.Equals(uptodate, "Already up to date.,") )
                {
                    Console.WriteLine("Changes detected. Starting build... ");
                    // make a new build
                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "\"C:/Program Files/Unity/Hub/Editor/2021.3.24f1/Editor/Unity.exe\"",
                            Arguments = "-projectPath C:/PartyCrashersBuildAutomation/Funky-Virtual-Party/FunkyVirtualParty_Unity -quit -batchmode -nographics -executeMethod BuildCommand.BuildWebGL",
                        }

                    };
                    proc.Start();
                    proc.WaitForExit();

                    Console.WriteLine("Build Process Finished. Pulling latest website changes...");


                    // pull vrpartygame
                    RunGitCommand("pull", "", "C:/PartyCrashersBuildAutomation/vrpartygame");

                    // Move built WebGL files to web repo
                    string soureFileData = "C:/PartyCrashersBuildAutomation/Funky-Virtual-Party/FunkyVirtualParty_Unity/Build/WebGL/Build/WebGL.data.gz";
                    string soureFileFramework = "C:/PartyCrashersBuildAutomation/Funky-Virtual-Party/FunkyVirtualParty_Unity/Build/WebGL/Build/WebGL.framework.js.gz";
                    string soureFileWasm = "C:/PartyCrashersBuildAutomation/Funky-Virtual-Party/FunkyVirtualParty_Unity/Build/WebGL/Build/WebGL.wasm.gz";
                    
                    string destFileData = "C:/PartyCrashersBuildAutomation/vrpartygame/assets/TestingToo/Build/WebGL.data.gz";
                    string destFileFramework = "C:/PartyCrashersBuildAutomation/vrpartygame/assets/TestingToo/Build/WebGL.framework.js.gz";
                    string destFileWasm = "C:/PartyCrashersBuildAutomation/vrpartygame/assets/TestingToo/Build/WebGL.wasm.gz";

                    if(File.Exists(destFileData))
                    {
                        File.Delete(destFileData);
                    }
                    if (File.Exists(destFileFramework))
                    {
                        File.Delete(destFileFramework);
                    }
                    if (File.Exists(destFileWasm))
                    {
                        File.Delete(destFileWasm);
                    }
                    File.Move(soureFileData, destFileData);
                    File.Move(soureFileFramework, destFileFramework);
                    File.Move(soureFileWasm, destFileWasm);

                    Console.WriteLine("Added files to website, pushing new build...");

                    // add, commit, and push build to git
                    RunGitCommand("add", "-A", "C:/PartyCrashersBuildAutomation/vrpartygame");
                    RunGitCommand("commit", "-m \"Automated Build #" + buildNumber + "\"", "C:/PartyCrashersBuildAutomation/vrpartygame");
                    RunGitCommand("push", "", "C:/PartyCrashersBuildAutomation/vrpartygame");

                    Console.WriteLine("Pushed Build #" + buildNumber +", process complete.");
                    Console.WriteLine();

                    // repeat
                    buildNumber++;
                }
                else
                {
                    Console.WriteLine("No changes detected.");
                }


            }

            string RunGitCommand(string command, string arguments, string workingDirectory)
            {
                string git = "git";
                var results = "";
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = git,
                        Arguments = $"{command} {arguments}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WorkingDirectory = workingDirectory,
                    }
                };
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    results += $"{proc.StandardOutput.ReadLine()},";
                }
                proc.WaitForExit();
                return results;
            }
        }

       
    }
}
