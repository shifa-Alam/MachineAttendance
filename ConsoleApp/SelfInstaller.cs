﻿using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
//using System.Configuration.Install;
using System.ServiceProcess;
using System.Configuration.Install;

namespace ConsoleApp
{
    public class SelfInstaller
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;
        public static bool InstallMe()
        {
            bool result;
            try
            {
               
               InstallerClass.InstallHelper(new string[] {
                    SelfInstaller._exePath
                });
            }
            catch
            {
                result = false;
                return result;
            }
            result = true;
            return result;
        }

        public static bool UninstallMe()
        {
            bool result;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] {
                    "/u", SelfInstaller._exePath
                });
            }
            catch
            {
                result = false;
                return result;
            }
            result = true;
            return result;
        }

    }
}