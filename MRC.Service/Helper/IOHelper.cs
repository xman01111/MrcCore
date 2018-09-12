using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MRC.Service.Helper
{
    public class IOHelper
    {
        /// <summary>
        /// 写入内容到文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        /// <param name="isAppend">是否附加写入</param>
        /// <returns></returns>
        public static bool WriteFile(string path, string content, bool isAppend)
        {
            if (!CreateDirectory(path))
                return false;
            try
            {
                using (StreamWriter sw = new StreamWriter(path, isAppend, Encoding.UTF8))
                {
                    sw.WriteLine(content);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 写入内容到文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        /// <param name="isAppend">是否附加写入</param>
        /// <returns></returns>
        public static bool WriteFile(string path, string content, bool isAppend, Encoding encoding)
        {
            if (!CreateDirectory(path))
                return false;
            using (StreamWriter sw = new StreamWriter(path, isAppend, encoding))
            {
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
            }
            return true;
        }
        /// <summary>
        /// 从指定位置的文件中读取字符串。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            if (!File.Exists(path))
                return string.Empty;
            string rs = string.Empty;
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                rs = sr.ReadToEnd();
                sr.Close();
            }
            return rs;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool DeleteFile(string path)
        {
            if (!File.Exists(path))
                return true;
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 复制文件到指定位置
        /// </summary>
        /// <param name="src">源路径</param>
        /// <param name="des">目标路径</param>
        /// <returns></returns>
        public static bool CopyFile(string src, string des)
        {
            if (!File.Exists(src))
                return false;
            try
            {
                File.Copy(src, des, true);
                return true;
            }
            catch { }
            return false;
        }
        /// <summary>
        /// 创建目录，已存在目录不操作。
        /// </summary>
        /// <param name="strDirectoryName">路径</param>
        /// <returns></returns>
        public static bool CreateDirectory(string strDirectoryName)
        {
            if (strDirectoryName == null || strDirectoryName == string.Empty)
                return false;
            try
            {
                strDirectoryName = Path.GetDirectoryName(strDirectoryName);
                if (!Directory.Exists(strDirectoryName))
                {
                    Directory.CreateDirectory(strDirectoryName);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="strDirectoryName">路径</param>
        /// <returns></returns>
        public static bool DeleteDirectory(string strDirectoryName)
        {
            if (strDirectoryName == null || strDirectoryName == string.Empty)
                return false;
            try
            {
                if (Directory.Exists(strDirectoryName))
                    Directory.Delete(strDirectoryName, true);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 路径合并
        /// </summary>
        /// <param name="paths">多个路径</param>
        /// <returns></returns>
        public static string CombinePath(params string[] paths)
        {
            if (paths.Length == 0)
            {
                throw new ArgumentException("please input path");
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string spliter = "\\";
                string firstPath = paths[0];
                if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
                {
                    spliter = "/";
                }
                if (!firstPath.EndsWith(spliter))
                {
                    firstPath = firstPath + spliter;
                }
                builder.Append(firstPath);
                for (int i = 1; i < paths.Length; i++)
                {
                    string nextPath = paths[i];
                    if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                    {
                        nextPath = nextPath.Substring(1);
                    }
                    if (i != paths.Length - 1)//not the last one
                    {
                        if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                        {
                            nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                        }
                        else
                        {
                            nextPath = nextPath + spliter;
                        }
                    }
                    builder.Append(nextPath);
                }
                return builder.ToString();
            }
        }


        /// <summary>
        /// 返回指定目录下的所有文件信息
        /// </summary>
        /// <param name="strDirectory"></param>
        /// <returns></returns>
        public static List<FileInfo> GetAllFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            foreach (DirectoryInfo _directoryInfo in directoryArray)
            {
                DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
                FileInfo[] fileInfoArrayA = directoryA.GetFiles();
                if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
                GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
            }
            return listFiles;
        }

        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        public static void CopyFolderTo(string directorySource, string directoryTarget)
        {
            //检查是否存在目的目录  
            if (!Directory.Exists(directoryTarget))
            {
                Directory.CreateDirectory(directoryTarget);
            }
            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
            FileInfo[] files = directoryInfo.GetFiles();
            //复制所有文件  
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(directoryTarget, file.Name));
            }
            //最后复制目录  
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                CopyFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
            }
        }
    }

}
