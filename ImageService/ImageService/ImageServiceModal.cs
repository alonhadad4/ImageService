//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Configuration;
/// <summary>
///  path is 
/// </summary>
namespace ImageService
{
    /// <summary>
    /// ImageServiceModal clss implementing the IImageServiceModal  interface
    /// </summary>
    class ImageServiceModal : IImageServiceModal
    {
        /// <summary>
        /// a function that adds the file to a directory by monts and years
        /// </summary>
        /// <param name="args"> the Year,Month</param>
        /// <param name="result"> the result</param>
        /// <returns></returns>
        string IImageServiceModal.AddFile(string[] args, out bool result)
        {
            //creating the path of the image,first creating the outdir, then the year folder and then the month
            string destinationPath;
            string path = GetAppSettings().Get("OutputDir") + @"\OutputDir";
            destinationPath = GetAppSettings().Get("OutputDir")+ @"\OutputDir\" + args[1] + @"\" + args[2];
            try
            {
                //create outputdir folder
                if (!Directory.Exists(path))
                    CreateFolder(path);
                path = path + @"\" + args[1];
                //create the year folder
                if (!Directory.Exists(path))
                    CreateFolder(path);
                path = path + @"\" + args[2];
                //create the month folder
                if (!Directory.Exists(path))
                    CreateFolder(path);

            }
            catch (Exception e)
            {
                result = false;
                return e.ToString();
            }

            try
            {
                //move the file to the new folder
                File.Move(args[0], destinationPath + @"\" + Path.GetFileName(args[0]));
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                return e.ToString();
            }
            //now repeating the procces for the thumbnail folder
            string newPath = destinationPath + @"\" + Path.GetFileName(args[0]);
            path = GetAppSettings().Get("OutputDir") + @"\OutputDir\Thumbnails";
            destinationPath = GetAppSettings().Get("OutputDir") + @"\OutputDir\Thumbnails" + @"\" + args[1] + @"\" + args[2];
            try
            {
                if (!Directory.Exists(path))
                    CreateFolder(path);
                path = path + @"\" + args[1];
                if (!Directory.Exists(path))
                    CreateFolder(path);
                path = path + @"\" + args[2];
                if (!Directory.Exists(path))
                    CreateFolder(path);
            }
            catch (Exception e)
            {
                result = false;
                return e.ToString();
            }
        
            try
            {
                //creating the thumbnail image as requierd
                Image image = Image.FromFile(newPath);
                int ThumbnailSize = Convert.ToInt32(GetAppSettings().Get("ThumbnailSize"));
                Image thumb = image.GetThumbnailImage(ThumbnailSize, ThumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(destinationPath + @"\" + Path.GetFileName(newPath));
                result = true;
                image.Dispose();
            }
            catch (Exception e)
            {
                result = false;
                return e.ToString();
            }
            
            return destinationPath;
        }
        /// <summary>
        /// a function that allows pulling data from the app settings
        /// </summary>
        /// <returns></returns>
        private static System.Collections.Specialized.NameValueCollection GetAppSettings()
        {
            return ConfigurationSettings.AppSettings;
        }
        /// <summary>
        /// creating a folder if it doesnt exist
        /// </summary>
        /// <param name="path"> the folder path</param>
        void CreateFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return;
                }
                DirectoryInfo di = Directory.CreateDirectory(path);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            catch (Exception e){}
            return;

        }
    }
}
