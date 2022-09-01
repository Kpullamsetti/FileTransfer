using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Security.Policy;

namespace Kamala_FileTransfer
{

    static class Program
    {

                
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
              
        static void Main(string[] args)
        {

                        

            ProcessInputFiles.InputFolder();
            



       }

        
}
}