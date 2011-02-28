/*
 * BPRename by Ilja Neumann - 2011
 * 
 *          - A automatic renamer for MP3s purchased at Beatport.com. Renames the files with information
 *            provided in the ID3 tags in a [ARTIST] - [NAME] ([Remix]).mp3 format.
 * 
 * 
 */

using System;
using System.Collections.Generic;
using ID3TagLib;
using System.IO;



namespace BPRename
{
    static class Program
    {
        const string FORMAT_ORIG = "{0} - {1}.mp3";
        const string FORMAT_RMX = "{0} - {1} ({2}).mp3";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Exit if no args
            if (args.Length==0||!File.Exists(args[0])||Path.GetExtension(args[0])!="mp3")
            {
                return;
            }

            

            string raw_artist;
            string raw_title;
            string src_name;
            string target_name;

            //Get filepath from first arg
            src_name = Path.GetFullPath(args[0]);

            //Read ID3v2 or ID3v1 tags 
            ReadTags(src_name, out raw_artist, out raw_title);

            //Splits between trackname and remix name and creates ne Filename
            string filename = Filter(raw_artist, raw_title);

            //Generate new "full path"
            target_name = Path.Combine(Path.GetDirectoryName(src_name),filename);

            //Rename File
            File.Move(src_name, target_name);

               
        }

        static void ReadTags(string path, out string artist, out string title)
        {
            //Load ID3File
            ID3File file = new ID3File(path);

            //Read V1 tag if V2 is not available
            if (file.ID3v2Tag == null)
            {

                artist = file.ID3v1Tag.Artist;
                title = file.ID3v1Tag.Title;
            }
            else
            {
                //Read V2


                TextFrame titleFrame = file.ID3v2Tag.Frames[0] as TextFrame;
                TextFrame artistFrame = file.ID3v2Tag.Frames[1] as TextFrame;
                title = titleFrame.Text;
                artist = artistFrame.Text;

            }

        }

        static string Filter(string artist, string title)
        {
            //Remove "Original Mix"
            title = title.Replace("- Original Mix", string.Empty);

            //Now try to seperate titlestring by " - " and save the result in splittedTitle[]
            string remixTitle=string.Empty;
            string[] remixSeperators = new string[1];
            remixSeperators[0] = " - ";
            string[] splittedTitle = title.Split(remixSeperators, StringSplitOptions.RemoveEmptyEntries);

            //Original Title must be here (first part)
            title = splittedTitle[0];

            //If splittedTitle.Length is >1 we have a Remix Title
            if (splittedTitle.Length > 1)
            {
                remixTitle = splittedTitle[1];
                return string.Format(FORMAT_RMX, artist, title, remixTitle);
            }
            else
            {
                //No Remix
                return string.Format(FORMAT_ORIG, artist, title);
            }   
        }
        
    }
}
