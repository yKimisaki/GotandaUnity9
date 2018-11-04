using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 日本語を出すためのおまじない
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            OutputEncoding = Encoding.GetEncoding("Shift_JIS");

            new[] { new int[] { 11, 12 }, new int[] { 21, 22 }, new int[] { 31, 32 } }
               .SelectMany((int[] x) => x)
               .Count();

            new[] { 1, 2, 3 }
               .SelectMany(_ => new[] { 1, 2 })
               .Count();

            Observable.Range(1, 3) // 1, 2, 3
               .SelectMany(_ => Observable.Range(1, 2)) // 1, 2
               .Subscribe(x => WriteLine("にゃんぱすー"));

            Observable.Range(1, 3) // 1, 2, 3
               .SelectMany(_ => Observable.Range(1, 2)) // 1, 2
               .Subscribe(
                    onNext: x => WriteLine("にゃんぱすー"),
                    onCompleted: () => WriteLine("今回はここまで"),
                    onError: e => WriteLine("スコー"));

            Observable.Range(-1, 3) // -1, 0, 1
                .Do(x => WriteLine(1 / x)) // 0で割ってみる
                                           // 例外を拾って0でやり直すが、これはこの後ろについてObservable.Range(-1, 3)の代わりに使うものになる。
                                           // ここまでのObservable.Range(-1, 3)はエラーが起きた段階で終了。
                .Catch((DivideByZeroException e) => Observable.Return(0))
                .Subscribe(
                    onNext: x => WriteLine("にゃんぱすー"),
                    onCompleted: () => WriteLine("今回はここまで"), // 復帰してたら呼ばれる
                    onError: e => WriteLine("スコー")); // 復帰してるので呼ばれない

            Observable.Empty<Unit>()
                .Subscribe(
                    onNext: x => WriteLine("にゃんぱすー"), // 呼ばれない
                    onCompleted: () => WriteLine("今回はここまで")); // 呼ばれる

            Observable.Range(1, 3)
                .SelectMany(_ => Observable.Empty<int>())
                .Subscribe(
                    onNext: x => WriteLine("にゃんぱすー"), // 呼ばれない
                    onCompleted: () => WriteLine("今回はここまで")); // 呼ばれる

            Observable.Never<Unit>()
                .Subscribe(
                    onNext: x => WriteLine("にゃんぱすー"), // 呼ばれない
                    onCompleted: () => WriteLine("今回はここまで")); // 呼ばれない

            // Task版
            await HogeAsync();
            await FugaAsync();
            WriteLine("にゃんぱすー"); // もちろん1回しか呼ばれない

            // IO<T>版
            HogeAsObservable()
                .SelectMany(_ => FugaAsObservable())
                .Subscribe(_ => WriteLine("にゃんぱすー")); // How many にゃんぱすー！s?

            OnClickAsObservable()
                .SelectMany(_ => HogeAsync().ToObservable()) // 長さが1である
                .SelectMany(_ => FugaAsync().ToObservable()) // 長さが1である
                .Subscribe(_ => WriteLine("にゃんぱすー")); // 押した数だけOnNextであるはずだ！！

            OnClickAsObservable()
                // Select時点ではIO<IO<T>>
                // つまりnew [] { IO<T>,  IO<T>, ...  } みたいな感じ
                .Select(_ => OnReceiveMessageAsObservable())
                // Switchを使うと、上でいう最後のIO<T>だけ抜き出す
                .Switch() 
                // 最後のClickに対するOnReceiveMessageAsObservableのみ有効
                .Subscribe(message => WriteLine(message)); 

            ReadKey();
        }

        static Task HogeAsync()
        {
            return Task.CompletedTask;
        }

        static Task FugaAsync()
        {
            return Task.CompletedTask;
        }

        static IObservable<Unit> HogeAsObservable()
        {
            return Observable.Return(Unit.Default);
        }

        static IObservable<Unit> FugaAsObservable()
        {
            return Observable.Return(Unit.Default);
        }

        static object image = null;
        static async Task SetImageAsync() // 非同期汚染される、さらに上もその上も
        {
            if (image == null)
            {
                image = await LoadAsync(); // この時だけ非同期がいいのに
            }
        }

        static Task<int> LoadAsync()
        {
            return Task.FromResult(0);
        }

        static IObservable<Unit> OnClickAsObservable()
        {
            return Observable.Return(Unit.Default);
        }

        static IObservable<Unit> OnReceiveMessageAsObservable()
        {
            return Observable.Return(Unit.Default);
        }
    }
}