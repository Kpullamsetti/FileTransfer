using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace Kamala_FileTransfer
{
    public class ProcessInputFiles
    {

        public static string Input_Path = "";


        public static string Error_Path = "";

        public static string log_Path = "";

        public static string Output_Path = "";

        public static string log_file = "";


        public static void InputFolder()
        {

            var url = System.AppContext.BaseDirectory;

            Input_Path = url.ToString() + "Input";


            Error_Path = url.ToString() + "Errors";

            Output_Path = url.ToString()+"Output";

            log_Path= url.ToString()+"Log";


            Console.Write("Checking Input Folder: ");

            try
            {

               

                if (Directory.Exists(Input_Path)) // checking if the input folder exists
                {

                    DirectoryInfo di = new DirectoryInfo(Input_Path);


                FileInfo[] fileNames = di.GetFiles("*.dat");



                foreach (var strfileName in fileNames)
                {

                   

                    string strDatname = strfileName.Name;

                    string[] TempDatFileName = strDatname.Split('.');

                    string strMdbFileName = TempDatFileName[0];

                        // checking if the matching .md5 file exist or not

                        if (System.IO.File.Exists(Input_Path + "\\" + strMdbFileName + ".md5")) 

                    {


                        var info = new FileInfo(Input_Path + "\\" + strDatname);

                        if (info.Length != 0)


                     

                        {

                            StreamReader streamreader = new StreamReader(Input_Path + "\\" + strfileName.Name);


                            char[] delimiter = new char[] { '|' };


                           
                            string[] columnheaders = streamreader.ReadLine().Split(delimiter);

                                streamreader.Close();

                            // Checking if the header has the required columns

                            if (columnheaders.Contains("EventId") || columnheaders.Contains("Details") || columnheaders.Contains("Timestamp"))
                            {


                                int columnCount = -1; // this will be the number of columns per row

                                //Validate the data in the file 
                                var invalidLines = System.IO.File
                                     .ReadLines(Input_Path + "\\" + strfileName.Name) 
                                    .Skip(1).Select((line, index) => {
                                        int count = line.Split(delimiter).Length;

                                        if (columnCount < 0)
                                            columnCount = count;

                                        return new
                                        {
                                            line = line,
                                            count = count,
                                            index = index
                                        };
                                    })
                                     .Where(chunk => chunk.count != columnCount)
                                     .Select(chunk => String.Format("Line #{0} \"{1}\" has {2} items when {3} expected",
                                     chunk.index + 1, chunk.line, chunk.count, columnCount));






                        //Checking if there is any invalid data in the file
                                if (invalidLines.Any())

                                {

                                    string ErrPath = Path.Combine(Error_Path, strDatname + "Invalid Data in the file");

                                    System.IO.File.Move(strfileName.FullName, ErrPath);
                                       
                                        Console.WriteLine(" Error: File: " + strDatname + "; Invalid Data in the file. Moved it to the error folder");


                                    }

                                    else  // Move the good files (.dat and .md5)  to the output folder
                                    {

                                    string OutputPath = Path.Combine(Output_Path, strDatname);

                                    System.IO.File.Move(strfileName.FullName, OutputPath);

                                        Console.WriteLine(" Success: File: " + strDatname + "; Moved it to the output folder");


                                        string md5output = Path.Combine(Output_Path, strMdbFileName + ".md5");

                                        string md5input = Path.Combine(Input_Path, strMdbFileName + ".md5" );

                                    System.IO.File.Move(md5input, md5output);

                                        Console.WriteLine(" Success: File: " + strMdbFileName +".md5" + "; Moved it to the output folder");


                                    }



                            }
                            else  // Else condition for the invalid header. Move the file to error folder
                            {

                                string ErrPath = Path.Combine(Error_Path, strDatname + "Invalid columns in the header");

                                System.IO.File.Move(strfileName.FullName, ErrPath);

                                    Console.WriteLine(" Error: File: " + strDatname + "; Invalid columns in the header. Moved it to the error folder");

                                }


                        }


                        else // Else condition for the empty file. Move the empty file to the error folder
                        {

                            string ErrPath = Path.Combine(Error_Path, strDatname + " EmptyFile");

                            Console.WriteLine(" Error: File: " + strDatname + "; is Empty. Moved it to the error folder");



                            System.IO.File.Move(strfileName.FullName, ErrPath);

                            //System.IO.File.Copy(strfileName.FullName, Error_Path, true);

                        }


                    }

                    // No matching .mdb file exists. Moving the .dat file to error folder
                    else

                    {
                        string ErrPath = Path.Combine(Error_Path, strDatname + "No Matching MDB file");

                        System.IO.File.Move(strfileName.FullName, ErrPath);

                            Console.WriteLine(" Error: File: " + strDatname + "; No Matching MDB file. Moved it to the error folder");


                        }



                }


            }



            else

            {
                Console.Write("Input Folder does not exist");

            }

        }


         catch (Exception error)
            {
                
                error_handling("Error in Application " + error.Message, null, null);
            }


        }


        // Error Handling to add messages to logs
        public static void error_handling(string message, string fileident, string source)
        {


             log_file = Path.Combine(log_file + "\\copy_log" + DateTime.Now.ToString("mmdd_hhmm") + ".txt");


            if (!System.IO.File.Exists(log_file))
            {

                System.IO.File.Create(log_file);
                Console.WriteLine("Log file is Created!");

            }
            
            
            using (System.IO.StreamWriter myFile = new System.IO.StreamWriter(log_file, true))
            {
                string finalMessage = string.Format("{0}: {1} SOURCE: {3} - DEST: {2}", DateTime.Now, message, fileident, source, Environment.NewLine);
                myFile.WriteLine(finalMessage);
                myFile.Close();
            }
        }




    }

       


}
