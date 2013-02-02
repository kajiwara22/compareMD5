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
            var subFolders = System.IO.Directory.GetDirectories(
                targetFolderPath, "*", System.IO.SearchOption.AllDirectories);
            var ret = new ObservableSynchronizedCollection<FileMd5>();
            //フォルダ下をチェック
            var infiles = System.IO.Directory.GetFiles(
                    targetFolderPath, "*", System.IO.SearchOption.AllDirectories);

            foreach (var file in infiles)
            {
                ret.Add(MD5Sum(file));
            }

            //サブフォルダ下をチェック
            foreach (var folder in subFolders)
            {
                var files = System.IO.Directory.GetFiles(
                    folder, "*", System.IO.SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    ret.Add(MD5Sum(file));
                }
            }
            return ret;
        }

        public FileMd5 MD5Sum(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            string md5sum = BitConverter.ToString(MD5.Create().ComputeHash(fs)).ToLower().Replace("-", "");
            fs.Close();
            return new FileMd5
            {
                MD5 = md5sum,
                Path = path
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
            if(sourceMD5List.Count >= targetMD5List.Count)
            {
                var tempTarget = targetMD5List;
                targetMD5List = sourceMD5List;
                sourceMD5List = tempTarget;
            }
            foreach (var sourceMD5item in sourceMD5List)
            {
                foreach (var targetMD5item in targetMD5List)
                {
                    if (targetMD5item.MD5 == sourceMD5item.MD5)
                    {
                        targetMD5List.Remove(targetMD5item);
                        break;
                    }
                }
            }
        }
    }
}
