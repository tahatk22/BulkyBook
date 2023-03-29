using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Helper
{
    public static class FileUpload
    {
        public static string Upload(string LocalPath , IFormFile file)
        {
			try
			{
                string PathName = Directory.GetCurrentDirectory() + LocalPath;
                string FileName = Guid.NewGuid() + Path.GetFileName(file.FileName);
                string FinalPath = Path.Combine(PathName, FileName);
                using (var stream = new FileStream(FinalPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return FileName;
            }
			catch (Exception ex)
			{

                return ex.Message;
			}
        }
        public static string DeleteFile(string localpath , string ImageUrl)
        {
            try
            {
                if (System.IO.File.Exists(Directory.GetCurrentDirectory() + localpath + "/" + ImageUrl))
                {
                    System.IO.File.Delete(Directory.GetCurrentDirectory() + localpath + "/" + ImageUrl);
                }
                return "";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
    }
}
