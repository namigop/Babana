using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace PlaywrightTest.ViewModels; 

public class ViewModelBase : ReactiveObject {

    protected ViewModelBase(){}
    protected ICommand CreateCommand(Func<Task> doThis) {
        return ReactiveCommand.Create(async () => {
            try {
                await doThis();
            }
            catch (Exception exc) {
                Console.WriteLine(exc.ToString());
            }
        });
    }

    protected ICommand CreateCommand(Action doThis) {
        return ReactiveCommand.Create(() => {
            try {
                doThis();
            }
            catch (Exception exc) {
                Console.WriteLine(exc.ToString());
            }
        });
    }
}
