using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.IO;
using System.Security.Cryptography;

namespace RinWPFAPP.Models
{
    public class Model : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        public ObservableSynchronizedCollection<FileMd5> sourceMD5List { get; set; }
        public ObservableSynchronizedCollection<FileMd5> targetMD5List { get; set; }

        public String sourceFolderPath { get; set; }
        public String targetFolderPath { get; set; }

        public ObservableSynchronizedCollection<FileMd5> check(String targetFolderPath)
        {
            var fileList = new List<FileMd5>();
            String rootPath = targetFolderPath;

            // ドラッグ＆ドロップされたものがディレクトリの場合
            if (Directory.Exists(targetFolderPath))
            {
                //フォルダ下をチェック
                var infiles = System.IO.Directory.GetFiles(
                        targetFolderPath, "*", System.IO.SearchOption.AllDirectories);

                foreach (var file in infiles)
                {
                    fileList.Add(MD5Sum(file, targetFolderPath));
                }
            }
            // ドラッグ＆ドロップされたものがファイルの場合
            else
            {
                fileList.Add(MD5Sum(targetFolderPath, targetFolderPath));
            }

            // ファイル一覧を、Path名でソートし、ObservableSynchronizedCollection に詰替え
            SorterByPath sorter = new SorterByPath();
            fileList.Sort(sorter);
            var ret = new ObservableSynchronizedCollection<FileMd5>();
            foreach(var file in fileList){
                ret.Add(file);
            }
            return ret;
        }
        private class SorterByPath : IComparer<FileMd5>
        {
            public int Compare(FileMd5 x, FileMd5 y)
            {
                return x.Path.CompareTo(y.Path);
            }
        }

        public FileMd5 MD5Sum(String path, String rootPath)
        {
            if(File.Exists(path)){
                FileStream fs = new FileStream(path, FileMode.Open);
                string md5sum = BitConverter.ToString(MD5.Create().ComputeHash(fs)).ToLower().Replace("-", "");
                fs.Close();
                return new FileMd5
                {
                    MD5 = md5sum,
                    // 相対パス変換後格納
                    Path = Directory.Exists(rootPath) ? path.Replace(rootPath+"\\", "") : path.Replace(System.IO.Path.GetDirectoryName(rootPath)+"\\", "")
                };
            }
            return new FileMd5
            {
                MD5 = "",
                Path = "（フォルダ/ファイルがありません）" + path.Replace(System.IO.Path.GetDirectoryName(rootPath) + "\\", "")
            }; 
        }

        public void check()
        {
            if (String.IsNullOrEmpty(sourceFolderPath) || String.IsNullOrEmpty(targetFolderPath))
            {
                return;
            }
            sourceMD5List = check(this.sourceFolderPath);
            targetMD5List = check(this.targetFolderPath);

            foreach (var sourceMD5item in sourceMD5List)
            {
                foreach (var targetMD5item in targetMD5List)
                {
                    int iCompare = sourceMD5item.Path.CompareTo(targetMD5item.Path);
                    if (iCompare == 0)
                    {
                        if (targetMD5item.MD5 == sourceMD5item.MD5)
                        {
                            targetMD5List.Remove(targetMD5item);
                            sourceMD5List.Remove(sourceMD5item);
                        }
                        break;
                    }
                    else if (iCompare < 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}
