/*
 * Title:   Profile Program Installer
 * Author:  Logan McArthur
 * Version: 1.0.0
 * Date:    8/21/2016
 * Description:
 *      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Diagnostics;

namespace Profile_Program_Installer
{
    class EntryPoint
    {
        static String RemoteProfileDirectory = "C:/Install/";
        static String ProfileDataFile = "profiles.txt";
        static String ProgramDataFile = "programs.txt";
        
        // Main has the primary purpose of identifying which profile we wish to use
        //  and then directing the program control over to the installation routines
        // Main expects a few different situations with the passed parameters
        //  To be extrapolated later though
        static void Main(string[] args)
        {
            List<Profile> Installations = new List<Profile>();
            if (args.Length == 0)
            {
                // There are no passed parameters, we need to ask for some
                Console.WriteLine("Welcome to the Profile Program Installer");
                Console.WriteLine("Input the profile to be installed, or type exit to terminate the program.");
                String ProfileChoice = Console.ReadLine();      // Take user input, halting program flow

                if (String.Compare(ProfileChoice,"exit",true) == 0)
                {
                    Console.WriteLine("Aborting installation.");
                    return;
                }
                Installations.Add(RetrieveProfile(ProfileChoice));
            }
            else
            {
                // We have passed parameters, 
            }
        }

        static Profile RetrieveProfile(String ProfileName)
        {
            Profile result = null;
            List<Profile> AvailableProfiles = GetProfileList();

            foreach (Profile Index in AvailableProfiles)
            {
                if (Index.ProfileName.Equals(ProfileName))
                {
                    result = Index;
                }
            }

            return result;
        }

        static List<Profile> GetProfileList()
        {
            List<Profile> Results = new List<Profile>();

            // Debug operations currently
            StreamReader FileReader = new StreamReader(ResolveFileLocation(ProfileDataFile));
            while (FileReader.Peek() != -1)
            {
                String DataLine = FileReader.ReadLine();
                if (DataLine.StartsWith("#"))
                {
                    continue;
                }
                Results.Add(ConstructProfileFromString(DataLine));
            }
            

            return Results;
        }

        static Profile ConstructProfileFromString(String Data)
        {
            String[] Tokens = Data.Split(';');
            HashSet<String> Programs = new HashSet<String>();

            for (int Index = 1; Index < Tokens.Length; Index++)
            {
                Programs.Add(Tokens[Index]);    // Add everything to the set
            }

            Profile Result = new Profile();
            Result.ProfileName = Tokens[0];
            Result.Programs = Programs;

            return Result;
        }

        static HashSet<Program> GetProgramSet()
        {
            HashSet<Program> Results = new HashSet<Program>();

            StreamReader FileReader = new StreamReader(ResolveFileLocation(ProgramDataFile));
            while (FileReader.Peek() != -1)
            {
                String DataLine = FileReader.ReadLine();
                if (DataLine.StartsWith("#"))
                {
                    continue;
                }
                Results.Add(ConstructProgramFromString(DataLine));
            }

            return Results;
        }

        static HashSet<Program> GetFilteredProgramSet(Profile Filter)
        {
            HashSet<Program> Results = new HashSet<Program>();

            StreamReader FileReader = new StreamReader(ResolveFileLocation(ProgramDataFile));
            while (FileReader.Peek() != -1)
            {
                String DataLine = FileReader.ReadLine();
                if (DataLine.StartsWith("#"))
                {
                    continue;
                }
                Program Construction = ConstructProgramFromString(DataLine);
                if (Filter.Programs.Contains(Construction.SoftwareName))
                {
                    Results.Add(Construction);
                }
            }

            return Results;
        }
        static Program ConstructProgramFromString(String Data)
        {
            String[] Tokens = Data.Split(';');
            Program Result = new Program();
            Result.SoftwareName = Tokens[0];
            Result.SoftwareLocation = Tokens[1];

            if (Tokens.Length > 2)
            {
                // We have arguments to include as well
                Result.SoftwareArguments = Tokens[2];
            }

            return Result;
        }

        static String ResolveFileLocation(String Path)
        {
            return RemoteProfileDirectory + Path;
        }
        static void InstallProfile(Profile ChosenProfile)
        {
            HashSet<Program> Programs = GetFilteredProgramSet(ChosenProfile);
            foreach (Program Software in Programs)
            {
                // We just need to install each program
                PerformInstallation(Software);
            }
        }

        static void PerformInstallation(Program Software)
        {
            Console.WriteLine("Software Name: " + Software.SoftwareName);
            Console.WriteLine("Software Location: " + Software.SoftwareLocation);
            Console.WriteLine("Software Arguments: " + Software.SoftwareArguments);
            Console.WriteLine("Press Enter To Install");
            Console.ReadLine();
            ProcessStartInfo StartInfo = new ProcessStartInfo(ResolveFileLocation(Software.SoftwareLocation), Software.SoftwareArguments);
            Process Installer = new Process();
            Installer.StartInfo = StartInfo;
            Installer.Start();
            Installer.WaitForExit();
        }
    }
}
