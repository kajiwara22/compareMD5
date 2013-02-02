using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using RinWPFAPP.Models;
using System.IO;
using System.Security.Cryptography;

namespace RinWPFAPP.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        public void Initialize()
        {
            
        }

        #region CheckCommand
        private ListenerCommand<string> _CheckCommand;

        public ListenerCommand<string> CheckCommand
        {
            get
            {
                if (_CheckCommand == null)
                {
                    _CheckCommand = new ListenerCommand<string>(Check, CanCheck);
                }
                return _CheckCommand;
            }
        }

        public bool CanCheck()
        {
            return !(string.IsNullOrEmpty(SourceDir) || string.IsNullOrEmpty(TargetDir));
        }

        public void Check(string parameter)
        {
            var checkMd5 = new Model();
            checkMd5.sourceFolderPath = SourceDir;
            checkMd5.targetFolderPath = TargetDir;
            checkMd5.check();
            var tempans = new StringBuilder();
            foreach (var item in checkMd5.targetMD5List)
            {
                tempans.AppendLine(item.Path);
            }
            Result = tempans.ToString();
            if (String.IsNullOrEmpty(Result))
            {

                Result =  "差異はありませんでした。" ;
            }
        }
        #endregion




        #region SourceDir変更通知プロパティ
        private string _SourceDir;

        public string SourceDir
        {
            get
            { return _SourceDir; }
            set
            { 
                if (_SourceDir == value)
                    return;
                _SourceDir = value;
                RaisePropertyChanged("SourceDir");
                CheckCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion


        #region TargetDir変更通知プロパティ
        private string _TargetDir;

        public string TargetDir
        {
            get
            { return _TargetDir; }
            set
            { 
                if (_TargetDir == value)
                    return;
                _TargetDir = value;
                RaisePropertyChanged("TargetDir");
                CheckCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion



        #region Result変更通知プロパティ
        private string _Result;

        public string Result
        {
            get
            { return _Result; }
            set
            { 
                if (_Result == value)
                    return;
                _Result = value;
                RaisePropertyChanged("Result");
            }
        }
        #endregion




       

    }
}
